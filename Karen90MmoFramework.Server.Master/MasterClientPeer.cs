using System;
using System.Threading;
using System.Collections.Generic;

using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Server.Master.Data;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Events;

namespace Karen90MmoFramework.Server.Master
{
	public class MasterClientPeer : ClientPeer
	{
		#region Constants and Fields

		/// <summary>
		/// the logger.
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		private readonly MasterApplication application;
		private readonly int clientId;

		private IncomingSubServerPeer currentWorldServer;
		private int currentState;

		private ClientLoginData loginData;

		private int i32SafelyDisconnected;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the master application
		/// </summary>
		protected MasterApplication Application
		{
			get
			{
				return this.application;
			}
		}

		/// <summary>
		/// Gets the client id
		/// </summary>
		public int ClientId
		{
			get
			{
				return this.clientId;
			}
		}

		/// <summary>
		/// Gets or sets the current world server
		/// </summary>
		public IncomingSubServerPeer WorldServer
		{
			get
			{
				return this.currentWorldServer;
			}
			set
			{
				Interlocked.Exchange(ref this.currentWorldServer, value);
			}
		}

		/// <summary>
		/// Gets the current client state
		/// </summary>
		public ClientPeerState CurrentState
		{
			get
			{
				return (ClientPeerState) this.currentState;
			}
			protected set
			{
				Interlocked.Exchange(ref this.currentState, (int) value);
			}
		}

		/// <summary>
		/// Gets the login data
		/// </summary>
		protected ClientLoginData LoginData
		{
			get
			{
				return this.loginData;
			}
			private set
			{
				Interlocked.Exchange(ref this.loginData, value);
			}
		}

		/// <summary>
		/// Gets or sets the safely disconnected state
		/// </summary>
		protected bool IsSafelyDisconnected
		{
			get
			{
				return this.i32SafelyDisconnected > 0;
			}
			set
			{
				Interlocked.Exchange(ref this.i32SafelyDisconnected, value ? 1 : 0);
			}
		}

		#endregion

		#region Constructors and Destructors

		public MasterClientPeer(InitRequest initRequest, MasterApplication application)
			: base(initRequest)
		{
			this.application = application;
			this.clientId = Utils.NewGuidInt32();

			this.LoginData = null;
			this.WorldServer = null;
			this.CurrentState = ClientPeerState.Connect;
			this.IsSafelyDisconnected = false;

			this.Application.MasterLobby.OnClientConnected(this);
#if MMO_DEBUG
			if (_logger.IsDebugEnabled)
				_logger.DebugFormat("Client (Id={0}) Connected", clientId);
#endif
		}

		#endregion

		#region Implementation of PeerBase

		protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
		{
			IncomingSubServerPeer receiver = null;
			OperationResponse response = null;

			switch (CurrentState)
			{
				case ClientPeerState.Connect:
					{
						if (operationRequest.OperationCode == (byte) ClientOperationCode.Login)
							receiver = Application.MasterLobby.SubServers.LoginServer;
					}
					break;

				case ClientPeerState.Login:
					{
						if (operationRequest.OperationCode != (byte) ClientOperationCode.Character)
							break;

						operationRequest.Parameters.Remove((byte) ParameterCode.Username);
						operationRequest.Parameters.Add((byte) ParameterCode.Username, this.LoginData.Username);

						receiver = Application.MasterLobby.SubServers.LoginServer;
					}
					break;

				case ClientPeerState.WorldEnter:
					{
						switch ((ClientOperationCode)operationRequest.OperationCode)
						{
							case ClientOperationCode.Chat:
								receiver = Application.MasterLobby.SubServers.ChatServer;
								break;

							case ClientOperationCode.Group:
							case ClientOperationCode.Social:
								receiver = Application.MasterLobby.SubServers.SocialServer;
								break;

							case ClientOperationCode.World:
								receiver = WorldServer;
								break;
						}
					}
					break;
			}

			if (receiver != null)
			{
				operationRequest.Parameters.Remove(0);
				operationRequest.Parameters.Add(0, ClientId);

				receiver.SendOperationRequest(operationRequest, sendParameters);
			}
			else
			{
				response = new OperationResponse(operationRequest.OperationCode)
					{
						ReturnCode = (short) ResultCode.OperationNotAvailable
					};
				_logger.ErrorFormat("State={0} SessionId={1} OpCode={2}", CurrentState, ClientId, (ClientOperationCode) operationRequest.OperationCode);
			}

			if (response != null)
				this.SendOperationResponse(response, new SendParameters{Encrypted = false});
		}

		protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
		{
			var oldState = this.CurrentState;
			this.CurrentState = ClientPeerState.Disconnect;
			this.application.MasterLobby.OnClientDisconnected(this);
#if MMO_DEBUG
			if(_logger.IsDebugEnabled)
				_logger.DebugFormat("Client (Id={0}) Disconnected", this.ClientId);
#endif
			if (IsSafelyDisconnected)
				return;

			this.IsSafelyDisconnected = true;
			this.NotifyDisconnection(oldState, null);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Call this when the user logs in.
		/// Sends client a <see cref="ClientEventCode.UserLoggedIn"/> message
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public void OnUserLogin(string username)
		{
			// client trying to login twice?
			if (LoginData != null)
			{
				_logger.ErrorFormat("[OnUserLogin]: LoginData already found for User (Username={0})", username);
				return;
			}

			MasterClientPeer existingPeer;
			if (application.MasterLobby.Users.TryGetValue(username, out existingPeer))
			{
				// disconnecting the old user's client
#if MMO_DEBUG
				if (_logger.IsDebugEnabled)
					_logger.DebugFormat("[OnUserLogin]: Disconnecting exisiting user (Username={0})", username);
#endif
				existingPeer.Disconnect();
				this.application.MasterLobby.Users.Remove(username);
			}

			this.LoginData = new ClientLoginData
				{
					ClientId = this.ClientId,
					Username = username,
				};

			// sending the user logged in message to the client
			this.SendEvent(
				new EventData((byte) ClientEventCode.UserLoggedIn,
				              new Dictionary<byte, object>
					              {
						              {(byte) ParameterCode.Username, username}
					              }),
				new SendParameters());

			this.application.MasterLobby.Users.Add(username, this);
			this.CurrentState = ClientPeerState.Login;
		}

		/// <summary>
		/// Call this when the user logs out.
		/// Disconnects the client.
		/// </summary>
		/// <param name="logoutServerPeer"></param>
		public void OnUserLogout(IncomingSubServerPeer logoutServerPeer)
		{
			var oldState = this.CurrentState;
			this.CurrentState = ClientPeerState.Disconnecting;
			this.Disconnect();
#if MMO_DEBUG
			if (_logger.IsDebugEnabled)
			{
				if(logoutServerPeer != null)
					_logger.DebugFormat("Client (Id={0}) Disconnected by GameServer (Type={1})", this.ClientId, logoutServerPeer.ServerType);
				else
					_logger.DebugFormat("Client (Id={0}) Disconnected", this.ClientId);
			}
#endif
			if (IsSafelyDisconnected)
				return;

			// no need to send a(n) UserLoggedOut event
			// unless the intend is NOT to disconnect the client when the user logs out
			// our game disconnects the client when the user logs out

			this.IsSafelyDisconnected = true;
			this.NotifyDisconnection(oldState, logoutServerPeer != null ? new List<IncomingSubServerPeer> { logoutServerPeer } : null);
		}

		/// <summary>
		/// Call this when the character logs in.
		/// </summary>
		/// <param name="guid"> </param>
		/// <param name="characterName"></param>
		/// <param name="worldServerId"></param>
		/// <returns></returns>
		public void OnCharacterLogin(int guid, string characterName, int worldServerId)
		{
			// client trying to play without logging?
			// this cannot happen
			if (LoginData == null)
			{
				_logger.ErrorFormat("[OnCharacterLogin]: LoginData not found for Character (CharName={0})", characterName);
				return;
			}

			if (CurrentState == ClientPeerState.WorldEnter || CurrentState == ClientPeerState.WorldTransfer)
			{
				_logger.ErrorFormat("[OnCharacterLogin]: Character (CharName={0}) is already playing", characterName);
				return;
			}

			IncomingSubServerPeer worldServerPeer;
			if (!application.MasterLobby.SubServers.TryGetSubserverByType(SubServerType.World, worldServerId, out worldServerPeer))
			{
				_logger.ErrorFormat("[OnCharacterLogin]: World (Id={0}) is down", worldServerId);
				// TODO: Queue player until world gets up or add to a temp world
				return;
			}

			this.LoginData.Guid = guid;
			this.LoginData.CharacterName = characterName;

			// sending the character logged in message to the client
			this.SendEvent(
				new EventData((byte) ClientEventCode.CharacterLoggedIn,
				              new Dictionary<byte, object>
					              {
						              {(byte) ParameterCode.CharacterName, this.LoginData.CharacterName}
					              }),
				new SendParameters());

			var addSession = new AddSession
				{
					SessionId = this.ClientId,
					CharacterName = this.LoginData.CharacterName,
					IsTransfer = false,
				};
			// adding the client session in chat, social and world servers
			new EventData((byte) ServerEventCode.AddSession, addSession).SendTo(
				new[]
					{
						application.MasterLobby.SubServers.ChatServer,
						application.MasterLobby.SubServers.SocialServer,
						worldServerPeer
					},
				new SendParameters());

			// sending the player transferring world message to the client
			this.SendEvent(
				new EventData((byte)ClientEventCode.PlayerTransferring,
							  new Dictionary<byte, object>
					              {
						              {(byte) ParameterCode.WorldId, worldServerId}
					              }),
				new SendParameters());

			// setting the current zone, this will make sure the server will notify the zone when the client disconnects
			this.WorldServer = worldServerPeer;
			// setting the transfer state prevents the client from sending any messages until the world accepts the player
			this.CurrentState = ClientPeerState.WorldTransfer;
		}

		/// <summary>
		/// Call this when the character logs out.
		/// Throws <see cref="NotImplementedException"/>.
		/// </summary>
		/// <param name="logoutServerPeer"></param>
		public void OnCharacterLogout(IncomingSubServerPeer logoutServerPeer)
		{
			throw new NotImplementedException();
			// sending the logged out character message to the client !important!
			var clientEventData = new EventData((byte)ClientEventCode.CharacterLoggedOut,
												new Dictionary<byte, object>
				                                    {
					                                    {(byte) ParameterCode.CharacterName, loginData.CharacterName}
				                                    });
			this.SendEvent(clientEventData, new SendParameters());
		}

		/// <summary>
		/// Call this when the player enters world.
		/// </summary>
		/// <param name="worldServerPeer"></param>
		public void OnPlayerTransferWorld(IncomingSubServerPeer worldServerPeer)
		{
			// client trying to play without logging?
			// this cannot happen
			if (LoginData == null)
			{
				_logger.ErrorFormat("[OnCharacterEnterWorld]: LoginData not found for Character");
				return;
			}

			if (CurrentState == ClientPeerState.WorldEnter)
			{
				_logger.ErrorFormat("[OnCharacterEnterWorld]: Character (CharName={0}) is already playing", loginData.CharacterName);
				return;
			}

			// resetting world server in case there is a world shutdown and the transfer was routed to another world
			if (WorldServer != worldServerPeer)
			{
#if MMO_DEBUG
				_logger.DebugFormat("[OnCharacterEnterWorld]: Updating the current world (NewId={0}. OldId={1})", worldServerPeer.SubServerId, WorldServer.SubServerId);
#endif
				this.WorldServer = worldServerPeer;
				this.LoginData.WorldId = WorldServer.SubServerId;
			}

			// updating the state world enter so the client can send messages to the world
			this.CurrentState = ClientPeerState.WorldEnter;

			// sending the player entered world message to the client
			this.SendEvent(
				new EventData((byte) ClientEventCode.PlayerTransferred,
				              new Dictionary<byte, object>
					              {
						              {(byte) ParameterCode.WorldId, WorldServer.SubServerId}
					              }),
				new SendParameters());
		}

		/// <summary>
		/// Call this when the player enters world.
		/// </summary>
		/// <param name="worldServerId"> </param>
		public void TransferWorld(int worldServerId)
		{
			// client trying to play without logging?
			// this cannot happen
			if (LoginData == null)
			{
				_logger.ErrorFormat("[OnCharacterEnterWorld]: LoginData not found for Character");
				return;
			}

			if (CurrentState != ClientPeerState.WorldEnter)
			{
				_logger.ErrorFormat("[OnCharacterEnterWorld]: Character (CharName={0}) is not playing", loginData.CharacterName);
				return;
			}

			IncomingSubServerPeer worldServerPeer;
			if (!application.MasterLobby.SubServers.TryGetSubserverByType(SubServerType.World, worldServerId, out worldServerPeer))
			{
				_logger.ErrorFormat("[TransferWorld]: World (Id={0}) is down", worldServerId);
				// TODO: Queue player until world gets up or add to a temp world
				return;
			}

			this.LoginData.WorldId = worldServerId;

			var addPlayer = new AddSession
				{
					SessionId = this.ClientId,
					CharacterName = this.LoginData.CharacterName,
					IsTransfer = true
				};
			// adding the client session in the new world to transfer to
			new EventData((byte) ServerEventCode.AddSession, addPlayer).SendTo(
				new[]
					{
						worldServerPeer
					},
				new SendParameters());

			// sending the player transferring world message to the client
			this.SendEvent(
				new EventData((byte)ClientEventCode.PlayerTransferring,
							  new Dictionary<byte, object>
					              {
						              {(byte) ParameterCode.WorldId, worldServerId}
					              }),
				new SendParameters());

			// setting the current zone, this will make sure the server will notify the zone when the client disconnects
			this.WorldServer = worldServerPeer;
			// setting the transfer state prevents the client from sending any messages until the world accepts the player
			this.CurrentState = ClientPeerState.WorldTransfer;
		}

		#endregion

		#region Local Methods

		/// <summary>
		/// Notifies a list of peers of the client disconnect based on the <see cref="oldState"/>.
		/// </summary>
		protected void NotifyDisconnection(ClientPeerState oldState, IEnumerable<IncomingSubServerPeer> excludeList)
		{
			var servers = this.Application.MasterLobby.SubServers;
			var receivingPeers = new List<IncomingSubServerPeer>();

			if (oldState == ClientPeerState.Login || oldState == ClientPeerState.Connect)
			{
				var loginServer = servers.LoginServer;
				if (loginServer != null)
					receivingPeers.Add(loginServer);
			}

			if (oldState == ClientPeerState.WorldEnter || oldState == ClientPeerState.WorldTransfer)
			{
				var chatServer = servers.ChatServer;
				if (chatServer != null)
					receivingPeers.Add(chatServer);

				var socialServer = servers.SocialServer;
				if(socialServer != null)
					receivingPeers.Add(socialServer);

				var worldServer = WorldServer;
				if (worldServer != null)
					receivingPeers.Add(worldServer);
			}

			if (excludeList != null)
				foreach (var peer in excludeList)
					receivingPeers.Remove(peer);
#if MMO_DEBUG
			if(_logger.IsDebugEnabled)
				_logger.DebugFormat("Removing client (Id={0}) from: {1}", this.ClientId, string.Join(", ", receivingPeers));
#endif
			if (receivingPeers.Count > 0)
			{
				var removePlayer = new RemoveSession
					{
						SessionId = this.ClientId
					};

				var eventData = new EventData((byte)ServerEventCode.RemoveSession, removePlayer);
				eventData.SendTo(receivingPeers, new SendParameters());
			}

			if (this.loginData != null)
			{
				MasterClientPeer exisitingPeer;
				if (this.application.MasterLobby.Users.TryGetValue(this.loginData.Username, out exisitingPeer))
				{
					// only remove the username if this client still owns the user
					if (exisitingPeer == this)
						this.application.MasterLobby.Users.Remove(this.loginData.Username);
				}
			}

			this.WorldServer = null;
		}

		#endregion
	}
}
