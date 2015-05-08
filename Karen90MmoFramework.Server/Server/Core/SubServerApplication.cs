using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.IO;

using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;

using log4net;
using log4net.Config;

using ExitGames.Logging.Log4Net;

using Karen90MmoFramework.Server.Config;

using LogManager = ExitGames.Logging.LogManager;

namespace Karen90MmoFramework.Server.Core
{
	public abstract class SubServerApplication : ApplicationBase
	{
		#region Constants and Fields

		/// <summary>
		/// logger
		/// </summary>
		protected static readonly ExitGames.Logging.ILogger Logger = LogManager.GetCurrentClassLogger();
		
		/// <summary>
		/// server id
		/// </summary>
		private readonly int serverId;

		/// <summary>
		/// server id
		/// </summary>
		private readonly byte subServerId;

		/// <summary>
		/// instance of the current sub server
		/// </summary>
		private static SubServerApplication _instance;

		/// <summary>
		/// peer used to send outgoing message to the master
		/// </summary>
		private OutgoingMasterServerPeer masterPeer;

		/// <summary>
		/// holds all pending operation requests to be sent to the master once the master server peer becomes available
		/// </summary>
		private readonly List<PendingOperationRequest> pendingOperationRequests = new List<PendingOperationRequest>(); 

		/// <summary>
		/// holds all pending external server connections
		/// </summary>
		private readonly Dictionary<long, IOutgoingServerPeer> pendingExternalServerConnections = new Dictionary<long, IOutgoingServerPeer>();

		#endregion

		#region Properties

		/// <summary>
		/// Gets the server configuration
		/// </summary>
		public ServerConfig ServerConfig { get; private set; }

		/// <summary>
		/// Gets the server id
		/// </summary>
		public int ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		/// <summary>
		/// Gets the sub server id
		/// </summary>
		public byte SubServerId
		{
			get
			{
				return this.subServerId;
			}
		}

		/// <summary>
		/// Gets the singleton instance
		/// </summary>
		public new static SubServerApplication Instance
		{
			get
			{
				return _instance;
			}

			protected set
			{
				Interlocked.Exchange(ref _instance, value);
			}
		}

		/// <summary>
		/// Gets the outgoing master server peer
		/// </summary>
		public OutgoingMasterServerPeer MasterPeer
		{
			get
			{
				return this.masterPeer;
			}

			protected set
			{
				Interlocked.Exchange(ref masterPeer, value);
			}
		}

		/// <summary>
		/// Gets the sub server type
		/// </summary>
		public SubServerType ServerType { get; private set; }

		/// <summary>
		/// Gets the ip address of the master
		/// </summary>
		public IPEndPoint MasterIPEndPoint { get; private set; }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new sub server
		/// </summary>
		protected SubServerApplication(SubServerType subServerType)
		{
			this.serverId = Utils.NewGuidInt32(GuidCreationCulture.SplitGuid);

			this.ServerConfig = ServerConfig.Initialize(BinaryPath);

			this.ServerType = subServerType;
			this.subServerId = ServerConfig.SubServer.SubServerId;

			var ip = IPAddress.Parse(ServerConfig.MasterServer.IP);
			var port = ServerConfig.MasterServer.ServerPort;
			
			this.MasterIPEndPoint = new IPEndPoint(ip, port);
		}

		#endregion

		#region ApplicationBase Overrides

		/// <summary>
		/// Sets up logging and connects to the master server
		/// </summary>
		protected override void Setup()
		{
			Instance = this;

			TypeSerializer.RegisterType(CustomTypeCode.Guid);

			this.SetupLogging();
			this.ConnectToMaster(this.MasterIPEndPoint);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		}

		protected override PeerBase CreatePeer(InitRequest initRequest)
		{
			return null;
		}

		protected override void TearDown()
		{
		}

		protected override sealed void OnStopRequested()
		{
			if (MasterPeer == null)
				return;

			this.MasterPeer.CleanupBeforeDisconnect();
			this.MasterPeer.Disconnect();
		}

		#endregion

		#region Connection and Create Peer Methods

		/// <summary>
		/// Connects to the master
		/// </summary>
		public bool ConnectToMaster(IPEndPoint masterIPEndPoint)
		{
			return ConnectToServerTcp(masterIPEndPoint, ServerConfig.MasterServer.MasterAppName, 0);
		}
		
		/// <summary>
		/// Connects to a server with a custom <see cref="IOutgoingServerPeer"/>.
		/// </summary>
		/// <param name="ipEndPoint"></param>
		/// <param name="applicationName"></param>
		/// <param name="outgoingServerPeerHandler"></param>
		/// <returns></returns>
		public bool ConnectToExternalServer(IPEndPoint ipEndPoint, string applicationName, IOutgoingServerPeer outgoingServerPeerHandler)
		{
			var callbackId = Utils.NewGuidInt64(GuidCreationCulture.Utc);
			if(ConnectToServerTcp(ipEndPoint, applicationName, callbackId))
			{
				lock (pendingExternalServerConnections)
					this.pendingExternalServerConnections.Add(callbackId, outgoingServerPeerHandler);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Queues an <see cref="OperationRequest"/> to be sent to the master once the <see cref="OutgoingMasterServerPeer"/> becomes available if its not already.
		/// </summary>
		public void QueueOutgoingMasterServerOperationRequest(OperationRequest operationRequest, EventHandler<OperationResponse> operationResponseHandler, long messageDelay = 0)
		{
			var peer = MasterPeer;
			if (peer != null && !peer.Disposed)
			{
				if (messageDelay <= 0)
				{
					peer.SendCallbackOperationRequest(operationRequest, operationResponseHandler);
				}
				else
				{
					peer.RequestFiber.Schedule(() => peer.SendCallbackOperationRequest(operationRequest, operationResponseHandler), messageDelay);
				}
				return;
			}

			lock (pendingOperationRequests)
				this.pendingOperationRequests.Add(new PendingOperationRequest(operationRequest, operationResponseHandler));
		}

		/// <summary>
		/// Creates an outgoing master peer
		/// </summary>
		protected abstract OutgoingMasterServerPeer CreateOutgoingMasterPeer(InitResponse initResponse);

		protected override sealed ServerPeerBase CreateServerPeer(InitResponse initResponse, object state)
		{
			if (IsGameServer(initResponse))
			{
				IOutgoingServerPeer outgoingServerPeerHandler;
				lock (pendingExternalServerConnections)
					if (!pendingExternalServerConnections.TryGetValue((long)state, out outgoingServerPeerHandler))
						return null;
				return new OutgoingServerToServerPeer(initResponse.Protocol, initResponse.PhotonPeer, outgoingServerPeerHandler);
			}

			var peer = this.CreateOutgoingMasterPeer(initResponse);
			lock (pendingOperationRequests)
			{
				if (pendingOperationRequests.Count > 0)
					foreach (var pendingOperationRequest in pendingOperationRequests)
						peer.SendCallbackOperationRequest(pendingOperationRequest.OperationRequest, pendingOperationRequest.ResponseHandler);

				this.pendingOperationRequests.Clear();
			}
			return this.MasterPeer = peer;
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Tells whether the connection is a game server or not
		/// </summary>
		protected bool IsGameServer(InitRequest request)
		{
			return request.LocalPort == ServerConfig.SubServer.TcpPort;
		}

		/// <summary>
		/// Tells whether the connection is a game server or not
		/// </summary>
		protected bool IsGameServer(InitResponse response)
		{
			return response.RemotePort != ServerConfig.MasterServer.ServerPort;
		}

		private void SetupLogging()
		{
			var file = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
			if (file.Exists)
			{
				LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
				GlobalContext.Properties["LogFileName"] = "SS_" + this.ApplicationName;
				XmlConfigurator.ConfigureAndWatch(file);
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error(e.ExceptionObject);
		}

		#endregion

		private class PendingOperationRequest
		{
			private readonly OperationRequest operationRequest;
			private readonly EventHandler<OperationResponse> responseHandler;

			public OperationRequest OperationRequest
			{
				get
				{
					return this.operationRequest;
				}
			}

			public EventHandler<OperationResponse> ResponseHandler
			{
				get
				{
					return this.responseHandler;
				}
			}

			public PendingOperationRequest(OperationRequest operationRequest, EventHandler<OperationResponse> responseHandler)
			{
				this.operationRequest = operationRequest;
				this.responseHandler = responseHandler;
			}
		}
	}
}
