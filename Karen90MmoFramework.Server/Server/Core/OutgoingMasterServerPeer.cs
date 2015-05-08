using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Photon.SocketServer.Rpc.Reflection;
using Photon.SocketServer.ServerToServer;

using PhotonHostRuntimeInterfaces;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;

namespace Karen90MmoFramework.Server.Core
{
	public abstract class OutgoingMasterServerPeer : ServerPeerBase
	{
		#region Constants and Fields

		private const int InternalPingRequestIntervalMs = 2000; // 2s
		private const int InternalPingRequestCount = 11;

		/// <summary>
		/// logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();
		
		private readonly SubServerApplication application;

		/// <summary>
		/// only use this for pinging-purposes
		/// </summary>
		private readonly IPhotonPeer __unmanagedPeer__;

		private Timer pingTimer;
		private int masterTimeOffset;
		private int pingCount;
		private List<int> latencyList;

		/// <summary>
		/// holds all the custom operation response handlers
		/// </summary>
		private readonly Dictionary<long, EventHandler<OperationResponse>> callbackOperationHandlers = new Dictionary<long, EventHandler<OperationResponse>>();

		#endregion

		#region Properties

		/// <summary>
		/// Returns the current debug logger
		/// </summary>
		protected static ExitGames.Logging.ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Gets estimated master server time stamp
		/// </summary>
		protected int MasterTimeStamp
		{
			get { return Environment.TickCount + masterTimeOffset; }
		}

		/// <summary>
		/// Indicates whether this server is registered or not
		/// </summary>
		protected bool IsRegistered { get; set; }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new outgoing master server peer used to talk with the master
		/// </summary>
		protected OutgoingMasterServerPeer(InitResponse initResponse, SubServerApplication application)
			: base(initResponse.Protocol, initResponse.PhotonPeer)
		{
			this.application = application;
			this.__unmanagedPeer__ = initResponse.PhotonPeer;

			this.InitializeEncryption();

			this.IsRegistered = false;
		}

		#endregion

		#region Client Communication Methods

		/// <summary>
		/// Sends a(n) <see cref="GameOperationResponse"/> to a session
		/// </summary>
		public void SendOperationResponse(int sessionId, GameOperationResponse gameResponse, MessageParameters messageParameters)
		{
			var parameters = ObjectDataMemberMapper.GetValues<DataMemberAttribute>(gameResponse);
			parameters.Add(0, sessionId);

			var operationResponse = new OperationResponse(gameResponse.OperationCode, parameters)
				{
					ReturnCode = gameResponse.ReturnCode,
					DebugMessage = gameResponse.DebugMessage
				};

			this.SendOperationResponse(operationResponse,
			                           new SendParameters
				                           {
					                           ChannelId = messageParameters.ChannelId,
					                           Encrypted = messageParameters.Encrypted,
				                           });
		}

		/// <summary>
		/// Sends a(n) <see cref="GameEvent"/> to a session
		/// </summary>
		public void SendEvent(int sessionId, GameEvent gameEvent, MessageParameters messageParameters)
		{
			var parameters = ObjectDataMemberMapper.GetValues<DataMemberAttribute>(gameEvent);
			parameters.Add(0, sessionId);

			var eventData = new EventData(gameEvent.EventCode, parameters);
			this.SendEvent(eventData, new SendParameters
				{
					ChannelId = messageParameters.ChannelId,
					Encrypted = messageParameters.Encrypted,
				});
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Registeres the server on the master
		/// </summary>
		protected void Register()
		{
			var serverInfo = application.ServerConfig.SubServer;
			var operationData = new RegisterServer
				{
					ServerId = this.application.ServerId,
					SubServerId = this.application.SubServerId,
					ServerType = (byte) this.application.ServerType,
					Address = serverInfo.PublicIP,
					TcpPort = serverInfo.TcpPort,
					UdpPort = serverInfo.UdpPort,
				};

			var request = new OperationRequest((byte) ServerOperationCode.RegisterServer, operationData);
			this.SendOperationRequest(request, new SendParameters());
		}

		/// <summary>
		/// Sends a callback <see cref="OperationRequest"/> to the master server where the callback will be called once an <see cref="OperationResponse"/> has been received
		/// </summary>
		/// <param name="operationRequest"></param>
		/// <param name="operationResponseHandler"></param>
		public SendResult SendCallbackOperationRequest(OperationRequest operationRequest, EventHandler<OperationResponse> operationResponseHandler)
		{
			// TODO Only send the request after the peer is registered
			var callbackId = Utils.NewGuidInt64(GuidCreationCulture.Utc);
			operationRequest.Parameters.Remove(2);
			operationRequest.Parameters.Add(2, callbackId);

			lock (callbackOperationHandlers)
				callbackOperationHandlers.Add(callbackId, operationResponseHandler);
			
			return this.SendOperationRequest(operationRequest, new SendParameters());
		}

		internal void CleanupBeforeDisconnect()
		{
			this.OnApplicationStopping();
		}

		#endregion

		#region Callbacks

		/// <summary>
		/// Called when the server is registered on the master
		/// </summary>
		protected virtual void OnServerRegistered()
		{
		}

		/// <summary>
		/// Called when the server registeration is failed
		/// </summary>
		protected virtual void OnServerRegisterFailed()
		{
		}

		/// <summary>
		/// Called right before disconnecting the peer. A good place to notify and kick connected clients.
		/// </summary>
		protected virtual void OnApplicationStopping()
		{
		}

		#endregion

		#region ServerPeerBase Overrides

		/// <summary>
		/// Called when an event has been received from the master server
		/// </summary>
		protected override sealed void OnEvent(IEventData eventData, SendParameters sendParameters)
		{
			this.OnServerEvent(eventData, sendParameters);
		}

		/// <summary>
		/// Called when an operation response has been received from the master server
		/// </summary>
		protected override sealed void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
		{
			switch (operationResponse.OperationCode)
			{
				// ping response
				case 0:
					{
						this.HandlePingResponse(operationResponse);
					}
					break;

				case (byte) ServerOperationCode.RegisterServer:
					{
						this.HandleRegisterServerResponse(operationResponse, sendParameters);
					}
					break;

				default:
					{
						object state;
						if(operationResponse.Parameters.TryGetValue(2, out state))
						{
							operationResponse.Parameters.Remove(2);

							EventHandler<OperationResponse> responseHandler;
							lock (callbackOperationHandlers)
							{
								var callbackId = (long) state;
								if (!callbackOperationHandlers.TryGetValue(callbackId, out responseHandler))
								{
									Logger.ErrorFormat("Cannot find the callbackId for the OperationResponse. Code={0}{{1}) CallbackId={2}", operationResponse.OperationCode, (ServerOperationCode)operationResponse.OperationCode, callbackId);
									return;
								}
								callbackOperationHandlers.Remove(callbackId);
							}

							responseHandler(this, operationResponse);
							return;
						}
						
						this.OnServerOperationResponse(operationResponse, sendParameters);
					}
					break;
			}
		}

		/// <summary>
		/// Called when an operation response has been received from the master server
		/// </summary>
		protected abstract void OnServerOperationResponse(OperationResponse operationResponse, SendParameters sendParameters);

		/// <summary>
		/// Called when an event has been received from the master server
		/// </summary>
		protected abstract void OnServerEvent(IEventData eventData, SendParameters sendParameters);

		/// <summary>
		/// Called when the server is disconnected
		/// </summary>
		/// <param name="reasonCode"></param>
		/// <param name="reasonDetail"></param>
		protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
		{
			// TODO: Re-connect

			this.IsRegistered = false;
		}

		#endregion

		#region Operation Response Handler Methods

		protected virtual void HandleRegisterServerResponse(OperationResponse operationResponse, SendParameters sendParameters)
		{
			if (operationResponse.ReturnCode == (short) ResultCode.Ok)
			{
				this.IsRegistered = true;

				Logger.DebugFormat("(:> ********** <:) {0} Server Id={1} is Registered (:> ********** <:)", application.ServerType, application.ServerId);

				this.OnServerRegistered();

				// starts to sync master time
				pingTimer = new Timer(o => this.SendPing(), null, 0, InternalPingRequestIntervalMs);
				Logger.Debug("Time sync Began");
			}
			else
			{
				Logger.DebugFormat("(:> ********** <:) {0} Server Id={1} Failed to Register Error={2} (:> ********** <:)", application.ServerType, application.ServerId,
				                   (ResultCode) operationResponse.ReturnCode);

				this.OnServerRegisterFailed();
			}
		}

		#endregion

		#region Helper Methods

		void SendPing()
		{
			if (pingCount > InternalPingRequestCount)
			{
				pingTimer.Dispose();
				pingTimer = null;

				return;
			}

			var operationRequest = new OperationRequest(/*PingRequest*/0, new Dictionary<byte, object>
				{
					{/*ServerTimeStamp*/5, (int) (DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond)}
				});
			
			// temporarily using the un-managed peer to suppress logging
			if (Connected)
			{
				this.__unmanagedPeer__.Send(this.Protocol.SerializeOperationRequest(operationRequest), MessageReliablity.Reliable, 0,
											MessageContentType.Binary);
			}
			
			Interlocked.Increment(ref pingCount);
		}

		void HandlePingResponse(OperationResponse response)
		{
			var currTime = (int)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);

			var pingResponse = new Operations.PingResponse(this.Protocol, response);
			if (!pingResponse.IsValid)
			{
				if(_logger.IsErrorEnabled)
					_logger.Error("Received unknown ping response");

				return;
			}

			// creating new latency list
			if(latencyList == null)
				latencyList = new List<int>();

			// adding the latency for the last packet received
			var latency = (currTime - pingResponse.ServerTimeStamp) >> 1;
			latencyList.Add(latency);

			// initial (temporary) time sync
			var mto = pingResponse.MasterTimeStamp - currTime + latency;
			Interlocked.Exchange(ref masterTimeOffset, mto);

			if (pingCount > InternalPingRequestCount)
			{
				// sorting low to high latency
				latencyList.Sort();

				var median = this.latencyList[(latencyList.Count >> 1) + 1];
				var mean = this.latencyList.Average();
				
				// calculating the diff mean squares
				var meanSquaresList = new List<float>();
				this.latencyList.ForEach(n => meanSquaresList.Add((float)Math.Pow(n - mean, 2)));
				
				// calculating the standard deviation
				var deviation = (float) Math.Sqrt(meanSquaresList.Average()/(meanSquaresList.Count - 1));
				
				// omiting higher latencies due to ack delay
				var newLatencyList = new List<int>();
				this.latencyList.ForEach(n =>
					{
						if (n <= deviation + median)
							newLatencyList.Add(n);
					});

				// calculating the average latency (estimate)
				var meanLatency = newLatencyList.Average();
				var nMto = (int)Math.Round(pingResponse.MasterTimeStamp - currTime + meanLatency);

				Interlocked.Exchange(ref masterTimeOffset, nMto);
				this.latencyList = null;

				Logger.DebugFormat("Time sync completed. Master Time Offset = {0}. MTS={1} STS={2}. OSTS={3}", mto, pingResponse.MasterTimeStamp, currTime, pingResponse.ServerTimeStamp);
			}
		}

		#endregion
	}
}
