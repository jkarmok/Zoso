using System.Collections.Generic;
using System.Threading;
using System.Net;

using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Core;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;

namespace Karen90MmoFramework.Server.World
{
	public sealed class OutgoingChatListenerPeer : IChatManager, IOutgoingServerPeer
	{
		#region Constants and Fields

		/// <summary>
		/// the instance
		/// </summary>
		private static readonly OutgoingChatListenerPeer _instance = new OutgoingChatListenerPeer();

		/// <summary>
		/// the logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// holds all the callbacks which will be invoked on successful channel creation
		/// </summary>
		private readonly Dictionary<long, ChannelCreatedCallback> channelCreatedCallbacks;

		/// <summary>
		/// pending operations
		/// </summary>
		private readonly List<OperationRequest> pendingOperations;

		/// <summary>
		/// the chat server peer
		/// </summary>
		private S2SPeerBase outgoingServerPeer;

		/// <summary>
		/// the listener state
		/// </summary>
		private int listenerState;

		/// <summary>
		/// shutdown state
		/// </summary>
		private int shuttingDown;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the singleton instance
		/// </summary>
		public static OutgoingChatListenerPeer Instance
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Gets the listener state
		/// </summary>
		public ListenerState ListenerState
		{
			get
			{
				return (ListenerState) listenerState;
			}

			private set
			{
				Interlocked.Exchange(ref listenerState, (int) value);
			}
		}

		/// <summary>
		/// Gets or sets the value which tells whether the <see cref="OutgoingChatListenerPeer"/> is shutting down or not
		/// </summary>
		private bool IsShuttingDown
		{
			get
			{
				return shuttingDown > 0;
			}

			set
			{
				Interlocked.Exchange(ref shuttingDown, value ? 1 : 0);
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="OutgoingChatListenerPeer"/> class.
		/// </summary>
		private OutgoingChatListenerPeer()
		{
			this.channelCreatedCallbacks = new Dictionary<long, ChannelCreatedCallback>();
			this.pendingOperations = new List<OperationRequest>();

			ListenerState = ListenerState.Disconnected;
			IsShuttingDown = false;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Begins the connection to the server
		/// </summary>
		public void Connect()
		{
			if (ListenerState == ListenerState.Disconnected)
				this.AcquireChatServerConnection();
		}

		/// <summary>
		/// Disconnects the peer
		/// </summary>
		public void Disconnect()
		{
			IsShuttingDown = true;
			if (outgoingServerPeer != null)
				this.outgoingServerPeer.Disconnect();
		}

		/// <summary>
		/// Acquires the chat server connection address
		/// </summary>
		/// <param name="messageDelay"></param>
		private void AcquireChatServerConnection(long messageDelay = 0)
		{
			SubServerApplication.Instance.QueueOutgoingMasterServerOperationRequest(
				new OperationRequest((byte) ServerOperationCode.AcquireServerAddress, new AcquireServerAddress
					{
						ServerType = (byte) ServerType.Chat,
					}),
				(o, r) => this.HandleOperationResponseAcquireServerResponse(WorldApplication.Instance.MasterPeer.Protocol, r), messageDelay);
		}

		/// <summary>
		/// Handles a <see cref="AcquireServerResponse"/>.
		/// </summary>
		private void HandleOperationResponseAcquireServerResponse(IRpcProtocol protocol, OperationResponse operationResponse)
		{
			var response = new AcquireServerResponse(protocol, operationResponse);
			if (!response.IsValid || operationResponse.ReturnCode != (short) ResultCode.Ok)
			{
#if MMO_DEBUG
				_logger.DebugFormat("Re-acquiring chat server connection info. ResponseValid?={0}. ReturnCode={1}.", response.IsValid, operationResponse.ReturnCode);
#endif
				this.AcquireChatServerConnection(ServerSettings.SERVER_RECONNECT_INTERVAL);
				return;
			}

			Interlocked.CompareExchange(ref listenerState, (int) ListenerState.Connecting, (int) ListenerState.Disconnected);

			var newListenerState = ListenerState;
			if (newListenerState != ListenerState.Connecting)
			{
				_logger.ErrorFormat("[HandleOperationResponseAcquireServerResponse]: Invalid listener state (State={0})", newListenerState);
				return;
			}

			var ipAddress = IPAddress.Parse(response.Address);
			var port = response.TcpPort;
			WorldApplication.Instance.ConnectToExternalServer(new IPEndPoint(ipAddress, port), "Chat", this);
		}

		#endregion

		#region Implementation of IChatManager

		/// <summary>
		/// Creates a chat channel
		/// </summary>
		public void CreateChannel(string name, ChannelType channelType, ChannelCreatedCallback callback)
		{
			var callbackId = Utils.NewGuidInt64(GuidCreationCulture.Utc);
			lock (channelCreatedCallbacks)
				this.channelCreatedCallbacks.Add(callbackId, callback);

			var operation = new CreateChannel
				{
					ChannelType = (byte) channelType,
					CallbackId = callbackId
				};

			if (!string.IsNullOrEmpty(name))
				operation.ChannelName = name;

			var operationRequest = new OperationRequest((byte) ServerOperationCode.CreateChannel, operation);
			this.DispatchOperationRequest(operationRequest);
		}

		/// <summary>
		/// Removes a chat channel
		/// </summary>
		public void RemoveChannel(int channelId)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.RemoveChannel, new RemoveChannel {ChannelId = channelId});
			this.DispatchOperationRequest(operationRequest);
		}

		/// <summary>
		/// Joins the session on a specific chat channel
		/// </summary>
		public void JoinChannel(int sessionId, int channelId)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.JoinChannel,
			                                            new JoinChannel
				                                            {
					                                            SessionId = sessionId,
					                                            ChannelId = channelId,
				                                            });
			this.DispatchOperationRequest(operationRequest);
		}

		/// <summary>
		/// Leaves the session from a specific chat channel
		/// </summary>
		public void LeaveChannel(int sessionId, int channelId)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.LeaveChannel,
			                                            new LeaveChannel
				                                            {
					                                            SessionId = sessionId,
					                                            ChannelId = channelId,
				                                            });
			this.DispatchOperationRequest(operationRequest);
		}

		#endregion

		#region Implementation of IOutgoingServerPeerHandler

		void IOutgoingServerPeer.OnConnect(S2SPeerBase connectedServerPeer)
		{
			Interlocked.CompareExchange(ref outgoingServerPeer, connectedServerPeer, null);

			this.ListenerState = ListenerState.Connected;
			lock (pendingOperations)
			{
				foreach (var pendingOperation in pendingOperations)
					this.outgoingServerPeer.SendOperationRequest(pendingOperation, new SendParameters());

				this.pendingOperations.Clear();
			}
		}

		void IOutgoingServerPeer.OnDisconnect(S2SPeerBase disconnectedServerPeer)
		{
			this.ListenerState = ListenerState.Disconnected;
			Interlocked.CompareExchange(ref outgoingServerPeer, null, disconnectedServerPeer);

			if (!IsShuttingDown)
				this.AcquireChatServerConnection();
		}

		void IOutgoingServerPeer.OnOperationResponse(IRpcProtocol protocol, OperationResponse operationResponse, SendParameters sendParameters)
		{
			switch ((ServerOperationCode) operationResponse.OperationCode)
			{
				case ServerOperationCode.CreateChannel:
					{
						this.HandleOperationResponseCreateChatChannel(protocol, operationResponse);
					}
					break;

				default:
#if MMO_DEBUG
					_logger.DebugFormat("[OnServerOperationResponse]: OperationResponse (OpCode={0}) is not handled", operationResponse.OperationCode);
#endif
					break;
			}
		}

		void IOutgoingServerPeer.OnEvent(IRpcProtocol protocol, IEventData eventData, SendParameters sendParameters)
		{
#if MMO_DEBUG
			_logger.DebugFormat("[OnEvent]: Event (Code={0}) is not handled", eventData.Code);
#endif
		}

		#endregion

		#region Operation Response Handlers

		private void HandleOperationResponseCreateChatChannel(IRpcProtocol protocol, OperationResponse operationResponse)
		{
			var response = new CreateChannelResponse(protocol, operationResponse);
			if (!response.IsValid)
			{
				_logger.ErrorFormat("[HandleOperationResponseCreateChatChannel]: Received an invalid {0} (Error={1})", response.GetType(), response.GetErrorMessage());
				return;
			}

			ChannelCreatedCallback callback;
			lock (channelCreatedCallbacks)
				if (channelCreatedCallbacks.TryGetValue(response.CallbackId, out callback))
					channelCreatedCallbacks.Remove(response.CallbackId);

			if (callback == null)
			{
				_logger.DebugFormat("Received a(n) {0} which does not have an EventHandler", response.GetType());
				return;
			}
			
			callback(response.ChannelId);
		}

		#endregion

		#region Helper Methods

		private void DispatchOperationRequest(OperationRequest operationRequest)
		{
			if (ListenerState == ListenerState.Connected)
			{
				outgoingServerPeer.SendOperationRequest(operationRequest, new SendParameters());
			}
			else
			{
				lock (pendingOperations)
					this.pendingOperations.Add(operationRequest);
			}
		}

		#endregion
	}
}
