using System;
using System.Linq;
using System.Threading;

using Photon.SocketServer;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Security;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;
using Karen90MmoFramework.Server.ServerToClient.Operations;

namespace Karen90MmoFramework.Server.Login.Handlers
{
	public class LoginOperationHandler : IGameOperationHandler
	{
		#region Constants and Fields

		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		private readonly LoginServerPeer peer;
		private readonly LoginApplication application;

		#endregion

		#region Constructors and Destructors

		public LoginOperationHandler(LoginServerPeer peer, LoginApplication application)
		{
			this.application = application;
			this.peer = peer;
		}

		#endregion

		#region IClientOperationHandler Implementation

		public void OnOperationRequest(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			switch ((GameOperationCode) operationRequest.OperationCode)
			{
				case GameOperationCode.LoginUser:
					{
						this.ExecUserOperation(() => this.HandleOperationLogin(operationRequest, parameters), operationRequest.ClientId, parameters);
					}
					break;

				case GameOperationCode.CreateUser:
					{
						this.ExecUserOperation(() => this.HandleOperationCreateNewUser(operationRequest, parameters), operationRequest.ClientId, parameters);
					}
					break;

				default:
					{
						this.peer.SendOperationResponse(operationRequest.ClientId,
						                                      new GameErrorResponse(operationRequest.OperationCode)
							                                      {
								                                      ReturnCode = (short) ResultCode.OperationNotAvailable,
							                                      },
						                                      new MessageParameters());
					}
					break;
			}
		}

		#endregion

		#region Operation Handler Methods

		protected virtual GameOperationResponse HandleOperationLogin(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new LoginUser(peer.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			ThreadPool.QueueUserWorkItem(
				o => this.ExecUserOperation(() => this.HandleLoginUser(operationRequest.ClientId, operation), operationRequest.ClientId, parameters));

			return null;
		}

		protected virtual GameOperationResponse HandleOperationCreateNewUser(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new CreateUser(peer.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetErrorResponse((short)ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			ThreadPool.QueueUserWorkItem(
				o => this.ExecUserOperation(() => this.HandleCreateNewUser(operationRequest.ClientId, operation), operationRequest.ClientId, parameters));

			return null;
		}

		#endregion

		#region Helper Methods

		private GameOperationResponse HandleCreateNewUser(int sessionId, CreateUser operation)
		{
			try
			{
				var username = operation.Username.ToUpper();
				var existingUserData = this.application.UserDatabase.Query<UserData>("UserData/ByUsername")
					//.Customize(x => x.WaitForNonStaleResultsAsOfNow())
					.Select(user => new {user.Username})
					.FirstOrDefault(user => user.Username.Equals(operation.Username, StringComparison.CurrentCultureIgnoreCase));

				if (existingUserData != null)
					return operation.GetErrorResponse((short) ResultCode.UsernameAlreadyExists);

				ResultCode resultCode;
				if (LoginHelper.IsValidUsername(username, out resultCode) == false)
					return operation.GetErrorResponse((short) resultCode);

				if (LoginHelper.IsValidPassword(operation.Password, out resultCode) == false)
					return operation.GetErrorResponse((short) resultCode);

				var passwordHash = SaltedHash.Create(operation.Password);
				var newUserInfo = new UserData
					{
						Id = UserData.GenerateId(username),
						Username = username,
						Salt = passwordHash.Salt,
						Password = passwordHash.Hash,
						IsBanned = false,
						CreatedOn = DateTime.Now,
						LastLogin = null,
					};

				this.application.UserDatabase.Store(newUserInfo);
				return operation.GetErrorResponse((short) ResultCode.Ok);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				return operation.GetErrorResponse((short) ResultCode.Fail);
			}
		}

		private GameOperationResponse HandleLoginUser(int sessionId, LoginUser operation)
		{
			try
			{
				var userData = this.application.UserDatabase.Query<UserData>("UserData/ByUsername")
					//.Customize(x => x.WaitForNonStaleResultsAsOfNow())
					.Select(user => new {user.Username, user.Salt, user.Password})
					.FirstOrDefault(user => user.Username.Equals(operation.Username, StringComparison.CurrentCultureIgnoreCase));
				if (userData == null)
					return operation.GetErrorResponse((short) ResultCode.IncorrectUsernameOrPassword);

				var passwordHash = SaltedHash.Create(userData.Salt, userData.Password);
				if (passwordHash.Verify(operation.Password) == false)
					return operation.GetErrorResponse((short) ResultCode.IncorrectUsernameOrPassword);

				// requesting master to authorize client
				this.peer.SendOperationRequest(new OperationRequest((byte) ServerOperationCode.AckClientUserLogin,
				                                                    new AckClientUserLogin
					                                                    {
						                                                    SessionId = sessionId,
						                                                    Username = userData.Username
					                                                    }),
				                               new SendParameters());
				return null;
			}
			catch (Exception e)
			{
				_logger.Error(e);
				return operation.GetErrorResponse((short) ResultCode.Fail);
			}
		}

		private void ExecUserOperation(Func<GameOperationResponse> operation, int sessionId, MessageParameters parameters)
		{
			var response = operation();
			if (response != null)
				this.peer.SendOperationResponse(sessionId, response, parameters);
		}

		#endregion
	}
}
