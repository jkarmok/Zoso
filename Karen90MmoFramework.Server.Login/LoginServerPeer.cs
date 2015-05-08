using Photon.SocketServer;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Server.Core;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToClient.Operations;
using Karen90MmoFramework.Server.Login.Handlers;

namespace Karen90MmoFramework.Server.Login
{
	public class LoginServerPeer : OutgoingMasterServerPeer
	{
		#region Constants and Fields

		private readonly IGameOperationHandler loginOperationHandler;
		private readonly IGameOperationHandler characterOperationHandler;

		#endregion

		#region Constructors and Destructors

		public LoginServerPeer(InitResponse initResponse, LoginApplication application)
			: base(initResponse, application)
		{
			this.loginOperationHandler = new LoginOperationHandler(this, application);
			this.characterOperationHandler = new CharacterOperationHandler(this, application);

			this.RequestFiber.Enqueue(this.Register);
		}

		#endregion

		#region ServerPeerBase Methods

		protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
		{
			var gameOperationRequest = new GameOperationRequest(operationRequest.Parameters);
			var parameters = new MessageParameters {ChannelId = sendParameters.ChannelId, Encrypted = sendParameters.Encrypted};

			switch (operationRequest.OperationCode)
			{
				case (byte) ClientOperationCode.Login:
					{
						this.RequestFiber.Enqueue(() => this.loginOperationHandler.OnOperationRequest(gameOperationRequest, parameters));
					}
					break;

				case (byte) ClientOperationCode.Character:
					{
						this.RequestFiber.Enqueue(() => this.characterOperationHandler.OnOperationRequest(gameOperationRequest, parameters));
					}
					break;

				default:
					{
						this.SendOperationResponse(gameOperationRequest.ClientId,
						                                 new GameErrorResponse(operationRequest.OperationCode)
							                                 {
								                                 ReturnCode = (short) ResultCode.OperationNotAvailable,
							                                 },
						                                 new MessageParameters());
					}
					break;
			}
		}

		protected override void OnServerEvent(IEventData eventData, SendParameters sendParameters)
		{
			Logger.WarnFormat("[OnServerEvent]: Event (EvtCode={0} ({1})) is not handled", eventData.Code, (ServerEventCode) eventData.Code);
		}

		protected override void OnServerOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
		{
			Logger.WarnFormat("[OnServerOperationResponse]: OperationResponse (OpCode={0} ({1})) is not handled", operationResponse.OperationCode,
			                  (ServerOperationCode) operationResponse.OperationCode);
		}

		#endregion
	}
}
