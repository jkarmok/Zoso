using System;
using System.Collections.Generic;

using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;

namespace Karen90MmoFramework.Server.Master
{
	public class IncomingSubServerPeer : ServerPeerBase
	{
		#region Constants and Fields

		/// <summary>
		/// logger
		/// </summary>
		protected static readonly ExitGames.Logging.ILogger Logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// master app
		/// </summary>
		private readonly MasterApplication application;

		/// <summary>
		/// only use this for pinging-purposes
		/// </summary>
		private readonly IPhotonPeer __unmanagedPeer__;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the application
		/// </summary>
		protected MasterApplication Application
		{
			get
			{
				return this.application;
			}
		}

		/// <summary>
		/// Gets the server id
		/// </summary>
		public int? ServerId { get; protected set; }

		/// <summary>
		/// Gets the sub server type
		/// </summary>
		public SubServerType ServerType { get; protected set; }

		/// <summary>
		/// Gets the sub server id
		/// </summary>
		public byte SubServerId { get; protected set; }

		/// <summary>
		/// Gets the server address
		/// </summary>
		public string ServerAddress { get; protected set; }

		/// <summary>
		/// Gets the server udp port
		/// </summary>
		public int UdpPort { get; protected set; }

		/// <summary>
		/// Gets the server tcp port
		/// </summary>
		public int TcpPort { get; protected set; }

		#endregion

		#region Constructors and Destructors

		public IncomingSubServerPeer(InitRequest initRequest, MasterApplication application)
			: base(initRequest.Protocol, initRequest.PhotonPeer)
		{
			this.application = application;
			__unmanagedPeer__ = initRequest.PhotonPeer;

			this.ServerId = null;
		}

		#endregion

		#region Client Methods

		protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
		{
			// Events from a Sub-Servers are always sent to a client

			object clientId;
			if (eventData.Parameters.TryGetValue(0, out clientId))
			{
				MasterClientPeer client;
				if (Application.MasterLobby.Clients.TryGetClient((int) clientId, out client))
				{
					eventData.Parameters.Remove(0);
					client.RequestFiber.Enqueue(() => client.SendEvent(eventData, sendParameters));
				}
			}
			else
			{
				Logger.ErrorFormat("[OnEvent]: Server (Id={0}) did not send a clientId with Event (EvCode={1})", this.ServerId, eventData.Code);
			}
		}

		protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
		{
			// Operation Response from a Sub-Servers are always sent to a client

			object clientId;
			if (operationResponse.Parameters.TryGetValue(0, out clientId))
			{
				MasterClientPeer client;
				if (Application.MasterLobby.Clients.TryGetClient((int) clientId, out client))
				{
					operationResponse.Parameters.Remove(0);
					client.RequestFiber.Enqueue(() => client.SendOperationResponse(operationResponse, sendParameters));
				}
			}
			else
			{
				Logger.ErrorFormat("[OnOperationResponse]: Server (Id={0}) did not send a clientId with OperationResponse (OpCode={1})", this.ServerId,
				                   operationResponse.OperationCode);
			}
		}

		#endregion

		#region Server Methods

		protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
		{
			OperationResponse response = null;
			switch ((ServerOperationCode) operationRequest.OperationCode)
			{
					// ping request
				case 0:
					this.HandlePingRequest(operationRequest);
					break;

				case ServerOperationCode.RegisterServer:
					response = ServerId != null
						           ? new OperationResponse((byte) ServerOperationCode.RegisterServer) {ReturnCode = (short) ResultCode.AlreadyRegistered}
						           : this.HandleOperationRegisterServer(operationRequest);
					break;

				case ServerOperationCode.AcquireServerAddress:
					response = this.HandleOperationAcquireServerAddress(operationRequest);
					break;

				case ServerOperationCode.AckClientUserLogin:
					response = this.HandleOperationAckClientUserLogin(operationRequest);
					break;

				case ServerOperationCode.AckClientCharacterLogin:
					response = this.HandleOperationAckClientCharacterLogin(operationRequest);
					break;

				case ServerOperationCode.KillClient:
					response = this.HandleOperationKillClient(operationRequest);
					break;

				case ServerOperationCode.KillSession:
					response = this.HandleOperationKillSession(operationRequest);
					break;

				case ServerOperationCode.TransferSession:
					response = this.HandleOperationTransferSession(operationRequest);
					break;

				case ServerOperationCode.AckClientPlayerTransferWorld:
					response = this.HandleOperationAckClientPlayerTransferWorld(operationRequest);
					break;

				default:
					response = new OperationResponse(operationRequest.OperationCode) {ReturnCode = (short) ResultCode.OperationNotAvailable};
					break;
			}

			if (response != null)
			{
				// if the state (paramCode = 2) was provided with the request send it back
				if (response.Parameters != null)
					response.Parameters.Remove(2);

				object state;
				if (operationRequest.Parameters.TryGetValue(2, out state))
				{
					if (response.Parameters == null)
						response.Parameters = new Dictionary<byte, object>();
					response.Parameters.Add( /*State*/2, state);
				}

				this.SendOperationResponse(response, sendParameters);
			}
		}

		protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
		{
			this.application.MasterLobby.OnServerDisconnected(this);
#if MMO_DEBUG
			Logger.DebugFormat("Server (Id={0}:{1}) Disconnected.", this.ServerId, this.ServerType);
#endif
		}

		#endregion

		#region Operation Request Handlers

		private void HandlePingRequest(OperationRequest operationRequest)
		{
			var operation = new PingRequest(this.Protocol, operationRequest);
			if (!operation.IsValid)
			{
				Logger.ErrorFormat("[HandlePingRequest]: Received an invalid ping request. ErrorMessage={0}", operation.GetErrorMessage());
				return;
			}

			var response = new OperationResponse(operationRequest.OperationCode,
			                                     new Dictionary<byte, object>
				                                     {
					                                     {/*MasterTimeStamp*/4, (int) (DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond)},
					                                     {/*ServerTimeStamp*/5, operation.ServerTimeStamp}
				                                     });

			// temporarily using the un-managed peer to suppress logging
			if (Connected)
				__unmanagedPeer__.Send(Protocol.SerializeOperationResponse(response), MessageReliablity.Reliable, 0, MessageContentType.Binary);
		}

		protected OperationResponse HandleOperationRegisterServer(OperationRequest operationRequest)
		{
			var operation = new RegisterServer(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			this.ServerId = operation.ServerId;
			this.ServerType = (SubServerType) operation.ServerType;
			this.SubServerId = operation.SubServerId;
			this.ServerAddress = operation.Address;
			this.TcpPort = operation.TcpPort;
			this.UdpPort = operation.UdpPort;

			this.Application.MasterLobby.OnServerConnected(this);

			return operation.GetResponse((short) ResultCode.Ok);
		}

		protected OperationResponse HandleOperationAcquireServerAddress(OperationRequest operationRequest)
		{
			var operation = new AcquireServerAddress(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			IncomingSubServerPeer peer = null;
			switch ((ServerType)operation.ServerType)
			{
				case Server.ServerType.Chat:
					{
						peer = this.application.MasterLobby.SubServers.ChatServer;
						if (peer == null)
							this.application.MasterLobby.SubServers.TryGetSubserverByTypeAny(SubServerType.Chat, out peer);
					}
					break;

				case Server.ServerType.Social:
					{
						peer = this.application.MasterLobby.SubServers.SocialServer;
						if (peer == null)
							this.application.MasterLobby.SubServers.TryGetSubserverByTypeAny(SubServerType.Social, out peer);
					}
					break;
			}

			if (peer == null || !peer.Connected)
				return operation.GetResponse((short) ResultCode.ServerNotFound, null);

			var serverInfo = new AcquireServerResponse
				{
					ServerType = operation.ServerType,
					Address = peer.ServerAddress,
					TcpPort = peer.TcpPort,
					UdpPort = peer.UdpPort,
				};

			return new OperationResponse(operationRequest.OperationCode, serverInfo) {ReturnCode = (short) ResultCode.Ok};
		}

		protected virtual OperationResponse HandleOperationAckClientUserLogin(OperationRequest operationRequest)
		{
			var operation = new AckClientUserLogin(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			MasterClientPeer clientPeer;
			if (!Application.MasterLobby.Clients.TryGetClient(operation.SessionId, out clientPeer))
				return operation.GetResponse((short) ResultCode.ClientNotFound, operation.GetErrorMessage());

			// logins in the user
			clientPeer.RequestFiber.Enqueue(() => clientPeer.OnUserLogin(operation.Username));
			return null;
		}

		protected virtual OperationResponse HandleOperationAckClientCharacterLogin(OperationRequest operationRequest)
		{
			var operation = new AckClientCharacterLogin(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			MasterClientPeer clientPeer;
			if (!Application.MasterLobby.Clients.TryGetClient(operation.SessionId, out clientPeer))
				return operation.GetResponse((short) ResultCode.ClientNotFound, operation.GetErrorMessage());

			// logins in the user
			clientPeer.RequestFiber.Enqueue(() => clientPeer.OnCharacterLogin(operation.Guid, operation.CharacterName, operation.ZoneId));
			return null;
		}

		protected virtual OperationResponse HandleOperationKillClient(OperationRequest operationRequest)
		{
			var operation = new KillClient(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short)ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			MasterClientPeer clientPeer;
			if (Application.MasterLobby.Clients.TryGetClient(operation.ClientId, out clientPeer))
			{
				// this will make sure the client will finish its current request
				clientPeer.RequestFiber.Enqueue(() => clientPeer.OnUserLogout(this));
			}

			return null;
		}

		protected virtual OperationResponse HandleOperationKillSession(OperationRequest operationRequest)
		{
			var operation = new KillSession(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			MasterClientPeer clientPeer;
			if (Application.MasterLobby.Clients.TryGetClient(operation.SessionId, out clientPeer))
			{
				// this will make sure the client will finish its current request
				clientPeer.RequestFiber.Enqueue(() => clientPeer.OnCharacterLogout(this));
			}

			return null;
		}

		protected virtual OperationResponse HandleOperationTransferSession(OperationRequest operationRequest)
		{
			var operation = new TransferSession(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			MasterClientPeer clientPeer;
			if (Application.MasterLobby.Clients.TryGetClient(operation.SessionId, out clientPeer))
			{
				// this will make sure the client will finish its current request
				clientPeer.RequestFiber.Enqueue(() => clientPeer.TransferWorld(operation.WorldId));
			}

			return null;
		}

		protected virtual OperationResponse HandleOperationAckClientPlayerTransferWorld(OperationRequest operationRequest)
		{
			var operation = new KillSession(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			MasterClientPeer clientPeer;
			if (Application.MasterLobby.Clients.TryGetClient(operation.SessionId, out clientPeer))
			{
				// this will make sure the client will finish its current request
				clientPeer.RequestFiber.Enqueue(() => clientPeer.OnPlayerTransferWorld(this));
			}

			return null;
		}

		#endregion
	}
}
