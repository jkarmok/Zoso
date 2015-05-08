using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using Karen90MmoFramework.Server.Game;
using Karen90MmoFramework.Server.Game.Systems;
using Karen90MmoFramework.Server.ServerToServer.Events;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Server.Core;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;

namespace Karen90MmoFramework.Server.World
{
	public sealed class OutgoingSocialListenerPeer : ISocialManager, IOutgoingServerPeer
	{
		#region Constants and Fields

		/// <summary>
		/// the instance
		/// </summary>
		private static readonly OutgoingSocialListenerPeer _instance = new OutgoingSocialListenerPeer();

		/// <summary>
		/// the logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// pending operations
		/// </summary>
		private readonly List<OperationRequest> pendingOperations;

		/// <summary>
		/// the chat server peer
		/// </summary>
		private ServerPeerBase outgoingServerPeer;

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
		public static OutgoingSocialListenerPeer Instance
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
		/// Gets or sets the value which tells whether the <see cref="OutgoingSocialListenerPeer"/> is shutting down or not
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
		/// Creates a new instance of the <see cref="OutgoingSocialListenerPeer"/> class.
		/// </summary>
		private OutgoingSocialListenerPeer()
		{
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
				this.AcquireSocialServerConnection();
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
		/// Acquires the social server connection address
		/// </summary>
		/// <param name="messageDelay"></param>
		void AcquireSocialServerConnection(long messageDelay = 0)
		{
			SubServerApplication.Instance.QueueOutgoingMasterServerOperationRequest(
				new OperationRequest((byte) ServerOperationCode.AcquireServerAddress, new AcquireServerAddress
					{
						ServerType = (byte) ServerType.Social,
					}),
				(o, r) => this.HandleOperationResponseAcquireServerResponse(WorldApplication.Instance.MasterPeer.Protocol, r), messageDelay);
		}

		/// <summary>
		/// Handles a <see cref="AcquireServerResponse"/>.
		/// </summary>
		void HandleOperationResponseAcquireServerResponse(IRpcProtocol protocol, OperationResponse operationResponse)
		{
			var response = new AcquireServerResponse(protocol, operationResponse);
			if (!response.IsValid || operationResponse.ReturnCode != (short)ResultCode.Ok)
			{
#if MMO_DEBUG
				_logger.DebugFormat("Re-acquiring social server connection info. ResponseValid?={0}. ReturnCode={1}.", response.IsValid, operationResponse.ReturnCode);
#endif
				this.AcquireSocialServerConnection(ServerSettings.SERVER_RECONNECT_INTERVAL);
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
			WorldApplication.Instance.ConnectToExternalServer(new IPEndPoint(ipAddress, port), "Social", this);
		}

		#endregion

		#region Implementation of ISocialManager

		/// <summary>
		/// Updates the profile of a session
		/// </summary>
		public void UpdateProfile(int sessionId, Hashtable properties)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.UpdateSocialProfile,
			                                            new UpdateSocialProfile
				                                            {
					                                            SessionId = sessionId,
					                                            Properties = properties
				                                            });
			this.DispatchOperationRequest(operationRequest);
		}

		/// <summary>
		/// Registers a world
		/// </summary>
		/// <param name="worldId"></param>
		public void RegisterWorld(short worldId)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.RegisterWorld,
			                                            new RegisterWorld
				                                            {
					                                            WorldId = worldId
				                                            });
			this.DispatchOperationRequest(operationRequest);
		}

		/// <summary>
		/// Unregisters a world
		/// </summary>
		/// <param name="worldId"></param>
		public void UnregisterWorld(short worldId)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.UnregisterWorld,
			                                            new UnregisterWorld
				                                            {
					                                            WorldId = WorldApplication.Instance.SubServerId
				                                            });
			this.DispatchOperationRequest(operationRequest);
		}

		/// <summary>
		/// Joins the profile to a world
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="worldId"></param>
		public void JoinWorld(int sessionId, short worldId)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.JoinWorld,
			                                            new JoinWorld
				                                            {
					                                            SessionId = sessionId,
					                                            WorldId = worldId
				                                            });
			this.DispatchOperationRequest(operationRequest);
		}

		/// <summary>
		/// Leaves the profile from a world
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="worldId"></param>
		public void LeaveWorld(int sessionId, short worldId)
		{
			var operationRequest = new OperationRequest((byte) ServerOperationCode.LeaveWorld,
			                                            new LeaveWorld
				                                            {
					                                            SessionId = sessionId,
					                                            WorldId = worldId
				                                            });
			this.DispatchOperationRequest(operationRequest);
		}

		#endregion

		#region Implementation of IOutgoingServerPeerHandler

		void IOutgoingServerPeer.OnConnect(ServerPeerBase connectedServerPeer)
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

		void IOutgoingServerPeer.OnDisconnect(ServerPeerBase disconnectedServerPeer)
		{
			this.ListenerState = ListenerState.Disconnected;
			Interlocked.CompareExchange(ref outgoingServerPeer, null, disconnectedServerPeer);

			if (!IsShuttingDown)
				this.AcquireSocialServerConnection();
		}

		void IOutgoingServerPeer.OnOperationResponse(IRpcProtocol protocol, OperationResponse operationResponse, SendParameters sendParameters)
		{
#if MMO_DEBUG
			_logger.DebugFormat("[OnServerOperationResponse]: OperationResponse (OpCode={0}) is not handled", operationResponse.OperationCode);
#endif
		}

		void IOutgoingServerPeer.OnEvent(IRpcProtocol protocol, IEventData eventData, SendParameters sendParameters)
		{
			switch ((ServerEventCode) eventData.Code)
			{
				case ServerEventCode.GroupFormed:
					HandleEventGroupFormed(protocol, eventData);
					break;

				case ServerEventCode.GroupMemberAdded:
					HandleEventGroupMemberAdded(protocol, eventData);
					break;

				case ServerEventCode.GroupMemberAddedSession:
					HandleEventGroupMemberAddedSession(protocol, eventData);
					break;

				case ServerEventCode.GroupMemberOnline:
					this.HandleEventGroupMemberOnline(protocol, eventData);
					break;

				case ServerEventCode.GroupMemberOffline:
					this.HandleEventGroupMemberOffline(protocol, eventData);
					break;

				case ServerEventCode.GroupMemberRemoved:
					HandleEventGroupMemberRemoved(protocol, eventData);
					break;

				case ServerEventCode.GroupDisbanded:
					HandleEventGroupDisbanded(protocol, eventData);
					break;
			}
		}

		#endregion

		#region Event Handlers

		private static void HandleEventGroupFormed(IRpcProtocol protocol, IEventData eventData)
		{
			var groupFormed = new GroupFormed(protocol, eventData);
			if(!groupFormed.IsValid)
			{
				_logger.ErrorFormat("[HandleEventGroupFormed]: Received an invalid {0}", eventData.GetType());
				return;
			}

			MmoWorld.Instance.PrimaryFiber.Enqueue(() => DoGroupFormed(groupFormed));
		}

		private static void DoGroupFormed(GroupFormed groupFormed)
		{
			Group existingGroup;
			if (GroupManager.Instance.TryGetGroup(groupFormed.GroupGuid, out existingGroup))
			{
				_logger.WarnFormat("[DoGroupFormed]: Existing group found (Guid={0}). Deforming the old group.", existingGroup.Guid);
				existingGroup.Deform();
			}

			var group = new Group(MmoWorld.Instance);
			group.FormExistingGroup(groupFormed.GroupGuid, groupFormed.LeaderInfo);
		}

		private static void HandleEventGroupMemberAdded(IRpcProtocol protocol, IEventData eventData)
		{
			var groupMemberAdded = new GroupMemberAdded(protocol, eventData);
			if (!groupMemberAdded.IsValid)
			{
				_logger.ErrorFormat("[HandleEventGroupMemberAdded]: Received an invalid {0}", eventData.GetType());
				return;
			}

			MmoWorld.Instance.PrimaryFiber.Enqueue(() => DoGroupMemberAdded(groupMemberAdded));
		}

		private static void DoGroupMemberAdded(GroupMemberAdded groupMemberAdded)
		{
			Group group;
			if (!GroupManager.Instance.TryGetGroup(groupMemberAdded.GroupGuid, out group))
			{
				_logger.WarnFormat("[DoGroupMemberAdded]: Group (Guid={0}) not found.", groupMemberAdded.GroupGuid);
				return;
			}

			group.AddMember(groupMemberAdded.MemberInfo);
		}

		private static void HandleEventGroupMemberAddedSession(IRpcProtocol protocol, IEventData eventData)
		{
			var groupMemberAddedSession = new GroupMemberAddedSession(protocol, eventData);
			if (!groupMemberAddedSession.IsValid)
			{
				_logger.ErrorFormat("[HandleEventGroupMemberAddedSession]: Received an invalid {0}", eventData.GetType());
				return;
			}

			MmoWorld.Instance.PrimaryFiber.Enqueue(() => DoGroupMemberAddedSession(groupMemberAddedSession));
		}

		private static void DoGroupMemberAddedSession(GroupMemberAddedSession groupMemberAddedSession)
		{
			Group group;
			if (!GroupManager.Instance.TryGetGroup(groupMemberAddedSession.GroupGuid, out group))
			{
				_logger.WarnFormat("[DoGroupMemberAddedSession]: Group (Guid={0}) not found.", groupMemberAddedSession.GroupGuid);
				return;
			}

			WorldSession session;
			if(MmoWorld.Instance.SessionCache.TryGetSessionBySessionId(groupMemberAddedSession.SessionId, out session))
			{
				group.AddMember(session.Player);
			}
			else
			{
				var memberInfo = groupMemberAddedSession.MemberInfo;
				var memberReference = new GroupMemberReference(memberInfo.Guid, groupMemberAddedSession.SessionId, memberInfo.Name, MmoWorld.Instance.Server);
				group.AddMember(memberReference);
			}
		}

		private void HandleEventGroupMemberOnline(IRpcProtocol protocol, IEventData eventData)
		{
			throw new System.NotImplementedException();
		}

		private void HandleEventGroupMemberOffline(IRpcProtocol protocol, IEventData eventData)
		{
			throw new System.NotImplementedException();
		}

		private static void HandleEventGroupMemberRemoved(IRpcProtocol protocol, IEventData eventData)
		{
			var groupMemberRemoved = new GroupMemberRemoved(protocol, eventData);
			if (!groupMemberRemoved.IsValid)
			{
				_logger.ErrorFormat("[HandleEventGroupMemberRemoved]: Received an invalid {0}", eventData.GetType());
				return;
			}

			MmoWorld.Instance.PrimaryFiber.Enqueue(() => DoGroupMemberRemoved(groupMemberRemoved));
		}

		private static void DoGroupMemberRemoved(GroupMemberRemoved groupMemberRemoved)
		{
			Group group;
			if (!GroupManager.Instance.TryGetGroup(groupMemberRemoved.GroupGuid, out group))
			{
				_logger.WarnFormat("[DoGroupMemberRemoved]: Group (Guid={0}) not found.", groupMemberRemoved.GroupGuid);
				return;
			}

			group.RemoveMember(groupMemberRemoved.MemberGuid);
		}

		private static void HandleEventGroupDisbanded(IRpcProtocol protocol, IEventData eventData)
		{
			var groupDisbanded = new GroupDisbanded(protocol, eventData);
			if (!groupDisbanded.IsValid)
			{
				_logger.ErrorFormat("[HandleEventGroupDisbanded]: Received an invalid {0}", eventData.GetType());
				return;
			}

			MmoWorld.Instance.PrimaryFiber.Enqueue(() => DoGroupDisbanded(groupDisbanded));
		}

		private static void DoGroupDisbanded(GroupDisbanded groupDisbanded)
		{
			Group group;
			if (!GroupManager.Instance.TryGetGroup(groupDisbanded.GroupGuid, out group))
			{
				_logger.WarnFormat("[DoGroupDisbanded]: Group (Guid={0}) not found.", groupDisbanded.GroupGuid);
				return;
			}

			group.Deform();
		}

		#endregion

		#region Helper Methods

		void DispatchOperationRequest(OperationRequest operationRequest)
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
