using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Photon.SocketServer.Concurrency;

using Raven.Json.Linq;
using Raven.Abstractions.Data;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Concurrency;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Social.Systems;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToClient.Events;
using Karen90MmoFramework.Server.ServerToClient.Operations;

namespace Karen90MmoFramework.Server.Social
{
	public class SocialSession : IDisposable, IPeer, ISession, IFriend, IGroupMember
	{
		#region Constants and Fields

		/// <summary>
		/// logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		private readonly int sessionId;
		private readonly string name;
		private int guid;

		private byte level;
		private short currentWorldId;

		private readonly MmoSocial social;
		private readonly ISocialServer server;
		private readonly ISerialFiber fiber;

		private readonly HashSet<string> friends;
		private readonly HashSet<string> ignores;

		private SocialStatus socialStatus;
		private readonly AsyncEvent<SocialStatus> onStatusChange;
		private readonly AsyncEvent<ProfilePropertyUpdateFlags> onProfileUpdate;
		private readonly AsyncEvent onDisconnect;
		private readonly Dictionary<IFriend, IDisposable> friendSubscriptions;
		private readonly List<IFriend> sentFriendRequests;
		private readonly List<IFriend> receivedFriendRequests;

		private int sessionState;
		private bool disposed;

		private Group group;
		private Group invitingGroup;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the session state
		/// </summary>
		public SocialSessionState SessionState
		{
			get
			{
				return (SocialSessionState) this.sessionState;
			}

			private set
			{
				Interlocked.Exchange(ref this.sessionState, (int) value);
			}
		}

		/// <summary>
		/// Gets the social
		/// </summary>
		public MmoSocial Social
		{
			get
			{
				return this.social;
			}
		}

		/// <summary>
		/// Gets the current world id
		/// </summary>
		public short CurrentWorldId
		{
			get
			{
				return this.currentWorldId;
			}
		}

		/// <summary>
		/// Gets the async event to listen for disconnects
		/// </summary>
		public AsyncEvent OnDisconnect
		{
			get
			{
				return this.onDisconnect;
			}
		}

		/// <summary>
		/// Gets the async event to listen for status changes
		/// </summary>
		public AsyncEvent<SocialStatus> OnStatusChange
		{
			get
			{
				return this.onStatusChange;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="SocialSession"/> class,
		/// </summary>
		public SocialSession(int sessionId, string name, ISocialServer server, MmoSocial social)
		{
			this.server = server;
			this.social = social;
			this.sessionId = sessionId;
			this.name = name;

			this.fiber = new SerialPoolFiber();
			this.fiber.Start();

			this.friends = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
			this.ignores = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

			this.onStatusChange = new AsyncEvent<SocialStatus>();
			this.onProfileUpdate = new AsyncEvent<ProfilePropertyUpdateFlags>();
			this.onDisconnect = new AsyncEvent();
			this.friendSubscriptions = new Dictionary<IFriend, IDisposable>();
			this.sentFriendRequests = new List<IFriend>();
			this.receivedFriendRequests = new List<IFriend>();

			this.socialStatus = SocialStatus.Online;
			this.SessionState = SocialSessionState.Login;
			this.currentWorldId = Game.MmoWorld.INVALID_WORLD_ID;

			this.fiber.Enqueue(DoAddSession);
		}

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		/// Disposes the <see cref="SocialSession"/>.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
				if (disposing)
					this.fiber.Dispose();

			this.disposed = true;
		}

		#endregion

		#region Implementation of IPeer

		/// <summary>
		/// Sends an operation response to the client
		/// </summary>
		public void SendOperationResponse(GameOperationResponse response, MessageParameters parameters)
		{
			this.server.SendOperationResponse(this.SessionId, response, parameters);
		}

		/// <summary>
		/// Sends an operation response to the client
		/// </summary>
		public void SendEvent(GameEvent eventData, MessageParameters parameters)
		{
			this.server.SendEvent(this.SessionId, eventData, parameters);
		}

		/// <summary>
		/// Disconnects the client
		/// </summary>
		public void Disconnect()
		{
			this.server.KillClient(this.sessionId);
		}

		#endregion

		#region Implementation of ISession

		/// <summary>
		/// Gets the session Id
		/// </summary>
		public int SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		/// <summary>
		/// Gets the character name
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Queues an <see cref="GameOperationRequest"/> to be processed
		/// </summary>
		public void ReceiveOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters)
		{
			this.fiber.Enqueue(() => this.OnOperationRequest(operationRequest, messageParameters));
		}

		/// <summary>
		/// Destroys the session
		/// </summary>
		/// <param name="destroyReason"></param>
		public void Destroy(DestroySessionReason destroyReason)
		{
			this.fiber.Enqueue(() =>
			{
				// making a copy of our current state since this can change
				var currentState = this.SessionState;
				// setting the session state right before doing cleanup to update the value immediately
				this.SessionState = SocialSessionState.Destroyed;
				// do not move it outside the thread
				// because this will be ignored if there are two destroy calls queued at the same time
				if (currentState == SocialSessionState.Destroyed)
					return;

				// leaves the current social world we are in
				this.DoLeaveWorld(currentWorldId);
				// publishing offline message
				this.socialStatus = SocialStatus.Offline;
				this.onStatusChange.Invoke(socialStatus);
				this.onDisconnect.Invoke();
				// clearing all the sent requests
				if (sentFriendRequests.Count > 0)
				{
					foreach (var requested in sentFriendRequests)
						requested.ClearRequest(this);
					this.sentFriendRequests.Clear();
				}

				// clearing all the received requests
				if (receivedFriendRequests.Count > 0)
				{
					foreach (var requester in receivedFriendRequests)
						requester.ReceiveDeclinedRequest(this, DeclineRequestReason.Disconnecting);
					this.receivedFriendRequests.Clear();
				}

				if (friendSubscriptions.Count > 0)
				{
					foreach (var friendSubscription in friendSubscriptions.Values)
						friendSubscription.Dispose();
					this.friendSubscriptions.Clear();
				}

				if (!social.Disposed)
				{
					// remove us from the social
					while (!social.RemoveSession(this))
					{
						SocialSession existingSession;
						if (!this.social.SessionCache.TryGetSessionBySessionId(this.SessionId, out existingSession))
							return;
					}
				}
			});
		}

		#endregion

		#region Implementation of IFriend

		/// <summary>
		/// Gets the name
		/// </summary>
		string IFriend.Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the level
		/// </summary>
		byte IFriend.Level
		{
			get
			{
				return this.level;
			}
		}

		/// <summary>
		/// Gets the status
		/// </summary>
		SocialStatus IFriend.Status
		{
			get
			{
				return this.socialStatus;
			}
		}

		/// <summary>
		/// Gets the current world id
		/// </summary>
		short IFriend.CurrentWorldId
		{
			get
			{
				return this.currentWorldId;
			}
		}

		/// <summary>
		/// Gets the async event to listen for status changes
		/// </summary>
		AsyncEvent<SocialStatus> IFriend.OnStatusChange
		{
			get
			{
				return this.onStatusChange;
			}
		}

		/// <summary>
		/// Gets the async event to listen for profile updates
		/// </summary>
		AsyncEvent<ProfilePropertyUpdateFlags> IFriend.OnProfileUpdate
		{
			get
			{
				return this.onProfileUpdate;
			}
		}

		/// <summary>
		/// Receives a friend login notification of a(n) <see cref="IFriend"/>
		/// </summary>
		/// <param name="friend"></param>
		void IFriend.ReceiveFriendLoginNotification(IFriend friend)
		{
			this.fiber.Enqueue(() => this.DoReceiveFriendLogin(friend));
		}

		private void DoReceiveFriendLogin(IFriend friend)
		{
			// not in our friends list? unlikely
			if (!friends.Contains(friend.Name))
				return;

			// already has subscription just return quitly
			// this can happen when two friends login at the same time
			if(friendSubscriptions.ContainsKey(friend))
				return;

			// setting up status and properties listeners
			var statusSubscription = friend.OnStatusChange.Subscribe(fiber, s => this.OnFriendStatusChange(friend, s));
			var profileUpdateSubscription = friend.OnProfileUpdate.Subscribe(fiber, flags => this.OnFriendProfileUpdate(friend, flags));
			this.friendSubscriptions.Add(friend, new UnsubscriberCollection(statusSubscription, profileUpdateSubscription));
			// notifying our client
			this.SendEvent(new SocialProfileStatus {NameOfProfile = friend.Name, Status = (byte) friend.Status},
			               new MessageParameters {ChannelId = PeerSettings.SocialEventChannel});
		}

		/// <summary>
		/// Receives a friend request from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="requester"></param>
		void IFriend.ReceiveFriendRequest(IFriend requester)
		{
			this.fiber.Enqueue(() => this.DoReceiveFriendRequest(requester));
		}

		private void DoReceiveFriendRequest(IFriend requester)
		{
			// are we destroyed?
			if (SessionState == SocialSessionState.Destroyed)
			{
				requester.ReceiveDeclinedRequest(this, DeclineRequestReason.Disconnecting);
				return;
			}

			var nameOfRequester = requester.Name;
			// requester is ignored?
			if (ignores.Contains(nameOfRequester))
			{
				requester.ReceiveDeclinedRequest(this, DeclineRequestReason.RequesterIsIgnored);
				return;
			}

			// already friend? (wont happen but good ol' Murphy's law)
			if (friends.Contains(nameOfRequester))
			{
				requester.ReceiveDeclinedRequest(this, DeclineRequestReason.AlreadyFriend);
				return;
			}

			// already received request from the same person?
			if (receivedFriendRequests.Contains(requester))
			{
				requester.ReceiveDeclinedRequest(this, DeclineRequestReason.AlreadyReceivedRequest);
				return;
			}

			// already sent request to the same person?
			if (sentFriendRequests.Contains(requester))
			{
				requester.ReceiveDeclinedRequest(this, DeclineRequestReason.AlreadySentRequest);
				return;
			}

			this.receivedFriendRequests.Add(requester);
			// sending the request event
			this.SendEvent(new SocialFriendRequestReceived {Requester = nameOfRequester}, new MessageParameters {ChannelId = PeerSettings.SocialEventChannel});
		}

		/// <summary>
		/// Receives an accepted request from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="receiver"></param>
		void IFriend.ReceiveAcceptedRequest(IFriend receiver)
		{
			this.fiber.Enqueue(() => this.DoReceiveAcceptedRequest(receiver));
		}

		private void DoReceiveAcceptedRequest(IFriend receiver)
		{
			// if we never received request in the first place simply return
			// this wont happen but Murphy's Law states otherwise (whml)
			if (!sentFriendRequests.Remove(receiver))
				return;

			// sending the accepted response
			this.SendOperationResponse(
				new SendFriendRequestResponse((byte) GameOperationCode.SendFriendRequest)
					{
						CharacterName = receiver.Name,
						ReturnCode = (short) ResultCode.Ok
					},
				new MessageParameters());

			// adding the player to our friend's list
			this.AddFriend(receiver, true);
		}

		/// <summary>
		/// Receives a declined request from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="declineRequestReason"></param>
		void IFriend.ReceiveDeclinedRequest(IFriend receiver, DeclineRequestReason declineRequestReason)
		{
			this.fiber.Enqueue(() => this.DoReceiveDeclinedRequest(receiver, declineRequestReason));
		}

		private void DoReceiveDeclinedRequest(IFriend receiver, DeclineRequestReason declineRequestReason)
		{
			// if we never received request in the first place simply return
			// this wont happen but Murphy's Law states otherwise (whml)
			if (!sentFriendRequests.Remove(receiver))
				return;

			var resultCode = ResultCode.Ok;
			switch (declineRequestReason)
			{
					// yup its opposite
				case DeclineRequestReason.AlreadyReceivedRequest:
					resultCode = ResultCode.AlreadySentRequest;
					break;
					// yup its opposite
				case DeclineRequestReason.AlreadySentRequest:
					resultCode = ResultCode.AlreadyReceivedRequest;
					break;
				case DeclineRequestReason.Disconnecting:
					resultCode = ResultCode.PlayerIsOffline;
					break;
				case DeclineRequestReason.RequesterIsIgnored:
					resultCode = ResultCode.PlayerIsIgnored;
					break;
				case DeclineRequestReason.AlreadyFriend:
					resultCode = ResultCode.FriendAlreadyAdded;
					break;
				case DeclineRequestReason.Declined:
					resultCode = ResultCode.DeclinedRequest;
					break;
			}

			if (resultCode != ResultCode.Ok)
			{
				// sending the declined response
				this.SendOperationResponse(
					new SendFriendRequestResponse((byte) GameOperationCode.SendFriendRequest)
						{
							CharacterName = receiver.Name,
							ReturnCode = (short) resultCode
						},
					new MessageParameters());
			}
		}

		/// <summary>
		/// Clears an invite from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="requester"></param>
		void IFriend.ClearRequest(IFriend requester)
		{
			this.fiber.Enqueue(() => this.DoClearRequest(requester));
		}

		private void DoClearRequest(IFriend requester)
		{
			this.SendEvent(new SocialFriendRequestCancelled {Requester = requester.Name}, new MessageParameters {ChannelId = PeerSettings.SocialEventChannel});
			this.receivedFriendRequests.Remove(requester);
		}

		/// <summary>
		/// Gets the profile info
		/// </summary>
		/// <returns></returns>
		ProfileStructure IFriend.GetProfile()
		{
			return new ProfileStructure(name, level, currentWorldId, (byte) socialStatus);
		}

		#endregion

		#region Implementation of IGroupMember

		/// <summary>
		/// Gets the guid
		/// </summary>
		public MmoGuid Guid
		{
			get
			{
				return new MmoGuid((byte) ObjectType.Player, guid);
			}
		}

		/// <summary>
		/// Gets the session
		/// </summary>
		IPeer IGroupMember.Peer
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Tells whether the member is in a group or not
		/// </summary>
		/// <returns></returns>
		bool IGroupMember.InGroup()
		{
			return group != null;
		}

		/// <summary>
		/// Gets or sets the current group
		/// </summary>
		Group IGroupMember.Group
		{
			get
			{
				return this.group;
			}

			set
			{
				this.group = value;
			}
		}

		/// <summary>
		/// Tells whether the member is invited or not
		/// </summary>
		/// <returns></returns>
		bool IGroupMember.IsInvited()
		{
			return invitingGroup != null;
		}

		/// <summary>
		/// Gets or sets the inviting group
		/// </summary>
		Group IGroupMember.InvitingGroup
		{
			get
			{
				return this.invitingGroup;
			}

			set
			{
				this.invitingGroup = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Joins a world
		/// </summary>
		/// <param name="worldId"></param>
		public void JoinWorld(short worldId)
		{
			this.fiber.Enqueue(() => this.DoJoinWorld(worldId));
		}

		/// <summary>
		/// Leaves a world
		/// </summary>
		/// <param name="worldId"></param>
		public void LeaveWorld(short worldId)
		{
			this.fiber.Enqueue(() => this.DoLeaveWorld(worldId));
		}

		/// <summary>
		/// Updates the profile
		/// </summary>
		/// <param name="properties"></param>
		public void UpdateProfile(Hashtable properties)
		{
			this.fiber.Enqueue(() => this.DoUpdateProfile(properties));
		}

		#endregion

		#region Operation Handlers

		// synchronous operation
		private GameOperationResponse HandleOperationSendGroupInvite(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SendGroupInvite(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			this.social.PrimaryFiber.Enqueue(() => this.ExecItemOperation(() => this.DoSendGroupInvite(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSendGroupInvite(SendGroupInvite operation)
		{
			if (string.IsNullOrEmpty(operation.CharacterName))
				return operation.GetErrorResponse((byte) ResultCode.CharacterNameIsEmpty);

			if (operation.CharacterName.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
				return operation.GetErrorResponse((byte) ResultCode.CannotInviteSelf);

			SocialSession session;
			if (!social.SessionCache.TryGetSessionBySessionName(operation.CharacterName, out session))
				return operation.GetErrorResponse((byte) ResultCode.PlayerNotFound);

			if (session == this)
				return operation.GetErrorResponse((byte) ResultCode.CannotInviteSelf);

			var receiver = (IGroupMember) session;
			if (receiver.InGroup())
				return operation.GetErrorResponse((byte) ResultCode.PlayerAlreadyInAGroup);
			
			if (!((IGroupMember) this).InGroup())
			{
				if (receiver.IsInvited())
					return operation.GetErrorResponse((byte) ResultCode.PlayerIsBusy);

				var newGroup = new Group(social);
				newGroup.FormNewGroup(this);

				if (!newGroup.SendInviteTo(receiver))
				{
					_logger.ErrorFormat("[DoSendGroupInvite]: GroupInvite to player (Name={0}) failed", receiver.Name);
					newGroup.Disband(false);
					return operation.GetErrorResponse((byte) ResultCode.PlayerIsBusy);
				}
			}
			else
			{
				if (group.Leader.Guid != this.Guid)
					return operation.GetErrorResponse((byte) ResultCode.YouAreNotTheLeader);

				if (receiver.IsInvited())
					return operation.GetErrorResponse((byte) ResultCode.PlayerIsBusy);

				if (group.IsFull())
					return operation.GetErrorResponse((byte) ResultCode.GroupIsFull);

				if (!group.SendInviteTo(receiver))
				{
					_logger.ErrorFormat("[DoSendGroupInvite]: GroupInvite to player (Name={0}) failed", receiver.Name);
					return operation.GetErrorResponse((byte) ResultCode.PlayerIsBusy);
				}
			}
			
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationAcceptGroupInvite(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new AcceptGroupInvite(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};
			this.social.PrimaryFiber.Enqueue(() => this.ExecItemOperation(() => this.DoAcceptGroupInvite(operation), parameters));
			return null;
		}

		private GameOperationResponse DoAcceptGroupInvite(AcceptGroupInvite operation)
		{
			if (!((IGroupMember) this).IsInvited())
				return operation.GetErrorResponse((short) ResultCode.NoInvitationFound);

			var acceptingGroup = this.invitingGroup;
			acceptingGroup.RemoveInvite(this, false);

			if (acceptingGroup.IsFull())
				return operation.GetErrorResponse((short) ResultCode.GroupIsFull);

			if (!acceptingGroup.AddMember(this))
			{
				if (acceptingGroup.Count < 2 && acceptingGroup.Invites == 0)
					acceptingGroup.Disband(true);

				return operation.GetErrorResponse((short) ResultCode.CannotJoinGroup);
			}

			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationDeclineGroupInvite(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new DeclineGroupInvite(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};
			this.social.PrimaryFiber.Enqueue(() => this.ExecItemOperation(() => this.DoDeclineGroupInvite(operation), parameters));
			return null;
		}

		private GameOperationResponse DoDeclineGroupInvite(DeclineGroupInvite operation)
		{
			if (!((IGroupMember)this).IsInvited())
				return operation.GetErrorResponse((short)ResultCode.NoInvitationFound);

			var decliningGroup = this.invitingGroup;
			decliningGroup.RemoveInvite(this, false);

			SocialSession session;
			if (social.SessionCache.TryGetSessionBySessionName(decliningGroup.Leader.Name, out session))
				session.SendEvent(new GroupInviteDeclined {Invited = this.Name}, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});

			if (decliningGroup.Count < 2 && decliningGroup.Invites == 0)
				decliningGroup.Disband(true);

			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationKickGroupMember(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new KickGroupMember(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};
			this.social.PrimaryFiber.Enqueue(() => this.ExecItemOperation(() => this.DoKickGroupMember(operation), parameters));
			return null;
		}

		private GameOperationResponse DoKickGroupMember(KickGroupMember operation)
		{
			if (!((IGroupMember)this).InGroup())
				return operation.GetErrorResponse((byte)ResultCode.YouAreNotInAGroup);

			if (!group.Leader.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
				return operation.GetErrorResponse((byte)ResultCode.YouAreNotTheLeader);

			if (!group.RemoveMember(operation.ObjectId))
				return operation.GetErrorResponse((byte) ResultCode.MemberNotFound);
			
			if (group.Count < 2 && group.Invites == 0)
				this.group.Disband(true);

			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationLeaveGroup(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new LeaveGroup(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};
			this.social.PrimaryFiber.Enqueue(() => this.ExecItemOperation(() => this.DoLeaveGroup(operation), parameters));
			return null;
		}

		private GameOperationResponse DoLeaveGroup(LeaveGroup operation)
		{
			if (!((IGroupMember)this).InGroup())
				return operation.GetErrorResponse((byte) ResultCode.YouAreNotInAGroup);

			if (group.Leader.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
			{
				// TODO: Promote the next available member
				this.group.Disband(true);
			}
			else
			{
				// wont happen
				var leavingGroup = this.group;
				if (!leavingGroup.RemoveMember(Guid))
					_logger.ErrorFormat("[DoLeaveGroup]: Cannot remove member (Name={0})", this.Name);

				if (leavingGroup.Count < 2 && leavingGroup.Invites == 0)
					leavingGroup.Disband(true);
			}

			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationDisbandGroup(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new DisbandGroup(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};
			this.social.PrimaryFiber.Enqueue(() => this.ExecItemOperation(() => this.DoDisbandGroup(operation), parameters));
			return null;
		}

		private GameOperationResponse DoDisbandGroup(DisbandGroup operation)
		{
			if (!((IGroupMember)this).InGroup())
				return operation.GetErrorResponse((byte)ResultCode.YouAreNotInAGroup);

			if (!group.Leader.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase))
				return operation.GetErrorResponse((byte) ResultCode.YouAreNotTheLeader);

			this.group.Disband(true);
			return null;
		}

		// immediate operation
		private GameOperationResponse HandleOperationSendFriendRequest(GameOperationRequest operationRequest)
		{
			var operation = new SendFriendRequest(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			var nameOfRequester = operation.CharacterName;
			// is the name empty?
			if (string.IsNullOrEmpty(nameOfRequester))
				return operation.GetErrorResponse((short) ResultCode.CharacterNameIsEmpty);

			// are we trying to friend ourselves?
			if (nameOfRequester.Equals(name, StringComparison.CurrentCultureIgnoreCase))
				return operation.GetErrorResponse((short) ResultCode.CannotFriendSelf);

			// is the person being ignored?
			if (ignores.Contains(nameOfRequester))
				return operation.GetErrorResponse((short) ResultCode.PlayerIsIgnored);

			// is the person already a friend?
			if (friends.Contains(nameOfRequester))
				return operation.GetErrorResponse((short) ResultCode.FriendAlreadyAdded);

			SocialSession session;
			// is the person online?
			if (!social.SessionCache.TryGetSessionBySessionName(nameOfRequester, out session))
				return operation.GetErrorResponse((short) ResultCode.PlayerNotFound);

			var requester = (IFriend) session;
			// already sent request?
			if (sentFriendRequests.Contains(requester))
				return operation.GetErrorResponse((short) ResultCode.AlreadySentRequest);

			// already received request?
			if (receivedFriendRequests.Contains(requester))
				return operation.GetErrorResponse((short) ResultCode.AlreadyReceivedRequest);

			this.sentFriendRequests.Add(requester);
			// sending the request
			requester.ReceiveFriendRequest(this);
			return null;
		}

		// immediate operation
		private GameOperationResponse HandleOperationAcceptFriendRequest(GameOperationRequest operationRequest)
		{
			var operation = new AcceptFriendRequest(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			var nameOfRequester = operation.CharacterName;
			// is the name empty?
			if (string.IsNullOrEmpty(nameOfRequester))
				return operation.GetErrorResponse((short) ResultCode.CharacterNameIsEmpty);

			// did we receive request?
			var requester = receivedFriendRequests.Find(person => person.Name.Equals(nameOfRequester));
			if (requester == null)
				return operation.GetErrorResponse((short) ResultCode.NoRequestFound);

			this.receivedFriendRequests.Remove(requester);
			// notifying the requester of the acceptance
			requester.ReceiveAcceptedRequest(this);
			// adding the player to our friend's list
			this.AddFriend(requester, true);
			return null;
		}

		// immediate operation
		private GameOperationResponse HandleOperationDeclineFriendRequest(GameOperationRequest operationRequest)
		{
			var operation = new DeclineFriendRequest(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			var nameOfRequester = operation.CharacterName;
			// is the name empty?
			if (string.IsNullOrEmpty(nameOfRequester))
				return operation.GetErrorResponse((short) ResultCode.CharacterNameIsEmpty);

			// did we receive request?
			var requester = receivedFriendRequests.Find(person => person.Name.Equals(nameOfRequester));
			if (requester == null)
				return operation.GetErrorResponse((short) ResultCode.NoRequestFound);

			this.receivedFriendRequests.Remove(requester);
			// notifying the requester of the decline
			requester.ReceiveDeclinedRequest(this, DeclineRequestReason.Declined);
			return null;
		}

		// immediate operation
		private GameOperationResponse HandleOperationRemoveFriend(GameOperationRequest operationRequest)
		{
			var operation = new RemoveFriend(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};
			return null;
		}

		/// <summary>
		/// Call this to handle client operation requests
		/// </summary>
		private void OnOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters)
		{
			// filters the operation operationRequest and updates the operation
			// this call is VERY IMPORTANT! since this ensures that operations are handled according
			// to the session state and permissions
			this.FilterOperationRequest(operationRequest);

			GameOperationResponse response;
			switch ((GameOperationCode) operationRequest.OperationCode)
			{
				case GameOperationCode.SendGroupInvite:
					response = this.HandleOperationSendGroupInvite(operationRequest, messageParameters);
					break;

				case GameOperationCode.AcceptGroupInvite:
					response = this.HandleOperationAcceptGroupInvite(operationRequest, messageParameters);
					break;

				case GameOperationCode.DeclineGroupInvite:
					response = this.HandleOperationDeclineGroupInvite(operationRequest, messageParameters);
					break;

				case GameOperationCode.KickGroupMember:
					response = this.HandleOperationKickGroupMember(operationRequest, messageParameters);
					break;

				case GameOperationCode.DisbandGroup:
					response = this.HandleOperationDisbandGroup(operationRequest, messageParameters);
					break;

				case GameOperationCode.LeaveGroup:
					response = this.HandleOperationLeaveGroup(operationRequest, messageParameters);
					break;

				case GameOperationCode.SendFriendRequest:
					response = this.HandleOperationSendFriendRequest(operationRequest);
					break;

				case GameOperationCode.AcceptFriendRequest:
					response = this.HandleOperationAcceptFriendRequest(operationRequest);
					break;

				case GameOperationCode.DeclineFriendRequest:
					response = this.HandleOperationDeclineFriendRequest(operationRequest);
					break;

				case GameOperationCode.RemoveFriend:
					response = this.HandleOperationRemoveFriend(operationRequest);
					break;

				case GameOperationCode.InvalidOperation:
					response = new GameErrorResponse(operationRequest.OperationCode) {ReturnCode = (short) ResultCode.OperationNotAvailable};
					break;

				case GameOperationCode.IgnoreOperation:
					response = null;
					break;

				default:
					response = new GameErrorResponse(operationRequest.OperationCode) {ReturnCode = (short) ResultCode.OperationNotAvailable};
					break;
			}

			if (response != null)
				this.SendOperationResponse(response, messageParameters);
		}

		/// <summary>
		/// Filters a(n) <see cref="GameOperationRequest"/> and updates the operationRequest
		/// </summary>
		/// <param name="operationRequest"></param>
		/// <returns></returns>
		private void FilterOperationRequest(GameOperationRequest operationRequest)
		{
			switch (SessionState)
			{
				case SocialSessionState.InWorld:
					break;

				case SocialSessionState.Profile:
					switch ((GameOperationCode) operationRequest.OperationCode)
					{
						case GameOperationCode.SendFriendRequest:
						case GameOperationCode.AcceptFriendRequest:
						case GameOperationCode.DeclineFriendRequest:
						case GameOperationCode.RemoveFriend:
							break;

						default:
							operationRequest.OperationCode = (byte) GameOperationCode.InvalidOperation;
							break;
					}
					break;

				default:
					operationRequest.OperationCode = (byte) GameOperationCode.InvalidOperation;
					break;
			}
		}

		#endregion

		#region Helper Methods

		private void LoadPlayer(string playerName)
		{
			var playerData = this.server.CharacterDatabase.Query<PlayerData>("PlayerData/ByName")
				.Select(player => new {player.Name, player.Guid, player.GroupGuid})
				.FirstOrDefault(player => player.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase));
			if(playerData == null)
			{
				// this can only happen if the character was deleted before we login
				// still the master should disconnect us before letting us enter the society
				// we are handling it to satisfy Murphy's Law
				_logger.ErrorFormat("[LoadPlayer]: PlayerData (Name={0}) cannot be found", this.Name);
				return;
			}

			this.guid = playerData.Guid;
			if (!playerData.GroupGuid.HasValue)
				return;

			Group playerGroup;
			if(GroupManager.Instance.TryGetGroup(playerData.GroupGuid.Value, out playerGroup))
				playerGroup.AddReference(this);
		}

		private void LoadProfile(string profileName)
		{
			var profileData = this.server.CharacterDatabase.Query<SocialProfileData>("SocialProfileData/ByName")
				//.Customize(x => x.WaitForNonStaleResultsAsOfNow())
				.FirstOrDefault(profile => profile.Name.Equals(profileName, StringComparison.CurrentCultureIgnoreCase));
			if (profileData == null)
			{
				// this can only happen if the character was deleted before we login
				// still the master should disconnect us before letting us enter the society
				// we are handling it to satisfy Murphy's Law
				_logger.ErrorFormat("[LoadProfile]: ProfileData (Name={0}) cannot be found", this.Name);
				this.Disconnect();
				this.Destroy(DestroySessionReason.KickedByServer);
				return;
			}

			this.level = profileData.Level;

			this.friends.Clear();
			this.ignores.Clear();

			// loading all friends
			var profileFriends = profileData.Friends;
			if (profileFriends != null)
			{
				if (profileFriends.Length > 0)
				{
					foreach (var friend in profileFriends)
						this.friends.Add(friend);

					// TODO: need to send in bulks
					if (profileFriends.Length > ServerSocialSettings.MAX_PROFILE_SEND_LIMIT)
						_logger.WarnFormat("[LoadProfile]: Friends list (Length={0}) exceeds the max send size", profileFriends.Length);

					// sending the friends list (names-only)
					this.SendEvent(new SocialFriendAddedNameMultiple { Names = profileFriends }, new MessageParameters { ChannelId = PeerSettings.SocialEventChannel });
				}
			}

			// loading all ignores
			var profileIgnores = profileData.Ignores;
			if (profileIgnores != null)
			{
				if (profileIgnores.Length > 0)
				{
					foreach (var ignore in profileIgnores)
						this.ignores.Add(ignore);

					// TODO: need to send in bulks
					if (profileIgnores.Length > ServerSocialSettings.MAX_PROFILE_SEND_LIMIT)
						_logger.WarnFormat("[LoadProfile]: Ignores list (Length={0}) exceeds the max send size", profileIgnores.Length);

					// sending the ignore list (names-only)
					this.SendEvent(new SocialIgnoreAddedNameMultiple { Names = profileIgnores }, new MessageParameters { ChannelId = PeerSettings.SocialEventChannel });
				}
			}

			if (friends.Count > 0)
			{
				FixedList<ProfileStructure> onlineFriends = null;
				foreach (var nameOfFriend in friends)
				{
					SocialSession session;
					if (social.SessionCache.TryGetSessionBySessionName(nameOfFriend, out session))
					{
						var friend = (IFriend)session;
						// setting up status and properties listeners
						var statusSubscription = friend.OnStatusChange.Subscribe(fiber, s => this.OnFriendStatusChange(friend, s));
						var profileUpdateSubscription = friend.OnProfileUpdate.Subscribe(fiber, flags => this.OnFriendProfileUpdate(friend, flags));
						this.friendSubscriptions.Add(friend, new UnsubscriberCollection(statusSubscription, profileUpdateSubscription));
						// notifying our friend of our login
						friend.ReceiveFriendLoginNotification(this);

						if (onlineFriends == null)
							onlineFriends = new FixedList<ProfileStructure>(ServerSocialSettings.MAX_PROFILE_SEND_LIMIT);

						onlineFriends.SetNext(friend.GetProfile());
						if (onlineFriends.IsFull())
						{
							// sending the friends list (profile)
							this.SendEvent(new SocialProfileUpdateMultiple { ProfileInfos = onlineFriends.ToArray() },
										   new MessageParameters { ChannelId = PeerSettings.SocialEventChannel });
							onlineFriends.Length = 0;
						}
					}
				}

				if (onlineFriends != null)
					if (onlineFriends.Length > 0)
						this.SendEvent(new SocialProfileUpdateMultiple { ProfileInfos = onlineFriends.ToArray() },
									   new MessageParameters { ChannelId = PeerSettings.SocialEventChannel });
			}

			this.SessionState = SocialSessionState.Profile;
		}

		private async void AddFriend(IFriend friend, bool notify)
		{
			this.friends.Add(friend.Name);
			// setting up status and properties listeners
			var statusSubscription = friend.OnStatusChange.Subscribe(fiber, s => this.OnFriendStatusChange(friend, s));
			var profileUpdateSubscription = friend.OnProfileUpdate.Subscribe(fiber, flags => this.OnFriendProfileUpdate(friend, flags));
			this.friendSubscriptions.Add(friend, new UnsubscriberCollection(statusSubscription, profileUpdateSubscription));
			// updating the database
			await this.server.CharacterDatabase.PatchAsync(SocialProfileData.GenerateId(guid),
														   new PatchRequest
														   {
															   Type = PatchCommandType.Add,
															   Name = "Friends",
															   Value = RavenJValue.FromObject(friend.Name)
														   });
			// updating the client
			if (notify)
				this.SendEvent(new SocialFriendAddedData { ProfileData = friend.GetProfile() }, new MessageParameters { ChannelId = PeerSettings.SocialEventChannel });
		}

		private void OnFriendStatusChange(IFriend friend, SocialStatus newStatus)
		{
			this.SendEvent(new SocialProfileStatus { NameOfProfile = friend.Name, Status = (byte)newStatus },
						   new MessageParameters { ChannelId = PeerSettings.SocialEventChannel });
			if (newStatus == SocialStatus.Offline)
			{
				IDisposable subscription;
				if (friendSubscriptions.TryGetValue(friend, out subscription))
				{
					subscription.Dispose();
					friendSubscriptions.Remove(friend);
				}
			}
		}

		private void OnFriendProfileUpdate(IFriend friend, ProfilePropertyUpdateFlags flags)
		{
			// since the profile holds only a small amount of data we will re-send the whole profile info instead of sending it as a hashtable
			// because the profileinfo has less size compared to a hashtable filled with one key + one value
			// for example to update the level:
			// Hashtable: size => 3 + size(key:2) + size(short:3) = 8
			// ProfileInfo: size => 1 + size(class:1) + size(level:2) + size(status:1) + size(worldId:2) = 7
			// and remember in both cases we need to send the name with the hashtable it is an additional 2 bytes for the key plus the size of the string
			// if the profile's size increases (more properties) consider changing it to a hashtable
			this.SendEvent(new SocialProfileUpdate { ProfileInfo = friend.GetProfile() }, new MessageParameters { ChannelId = PeerSettings.SocialEventChannel });
		}

		private void DoAddSession()
		{
			// adding session to the social
			while (!social.AddSession(this))
			{
				SocialSession exisitingSession;
				if (this.social.SessionCache.TryGetSessionBySessionId(this.sessionId, out exisitingSession))
				{
					// this shouldn't happen because the master won't let the same player login twice it will disconnect the exisitng player
					// we are handling it to satisfy Murphy's Law
#if MMO_DEBUG
					_logger.DebugFormat("[DoAddSession]: SocialSession (Name={0}) cannot be added because an existing SocialSession (Name={1}) is found",
					                    this.Name, exisitingSession.Name);
#endif
					exisitingSession.Disconnect();
					this.Disconnect();
					this.Destroy(DestroySessionReason.KickedByExistingSession);
					return;
				}
			}
			this.LoadPlayer(name);
			this.LoadProfile(name);
		}

		private void DoJoinWorld(short worldId)
		{
			if (currentWorldId == worldId)
			{
				_logger.ErrorFormat("[DoJoinWorld]: Session (Name={0}) trying to join the World (Id={1}) twice", name, worldId);
				return;
			}

			// this can happen in case the message was delayed
			if (currentWorldId != Game.MmoWorld.INVALID_WORLD_ID)
				_logger.WarnFormat("[DoJoinWorld]: Session (Name={0}) is already in World (Id={1})", name, currentWorldId);

			this.SessionState = SocialSessionState.InWorld;
			this.currentWorldId = worldId;
			// notifying friends
			this.onProfileUpdate.Invoke(ProfilePropertyUpdateFlags.WorldId);
		}

		private void DoLeaveWorld(short worldId)
		{
			if (currentWorldId != worldId)
			{
				_logger.ErrorFormat("[DoLeaveWorld]: Session (Name={0}) trying leave an un-joined World (Id={1})", name, worldId);
				return;
			}

			if (currentWorldId == Game.MmoWorld.INVALID_WORLD_ID)
			{
				_logger.ErrorFormat("[DoLeaveWorld]: Session (Name={0}) is not in any world", name);
				return;
			}

			this.SessionState = SocialSessionState.Profile;
			this.currentWorldId = Game.MmoWorld.INVALID_WORLD_ID;
			// notifying friends
			this.onProfileUpdate.Invoke(ProfilePropertyUpdateFlags.WorldId);
		}

		private void DoUpdateProfile(IDictionary properties)
		{
			var flags = ProfilePropertyUpdateFlags.None;
			foreach (DictionaryEntry entry in properties)
			{
				switch ((ProfileProperty) entry.Key)
				{
					case ProfileProperty.Level:
						{
							var newLevel = (byte) entry.Value;
							this.level = newLevel;
							flags |= ProfilePropertyUpdateFlags.Level;
							// updating the database
							this.server.CharacterDatabase.PatchAsync(SocialProfileData.GenerateId(guid),
							                                         new PatchRequest
								                                         {
									                                         Type = PatchCommandType.Set,
									                                         Name = "Level",
									                                         Value = RavenJValue.FromObject(newLevel)
								                                         });
						}
						break;

					default:
						properties.Remove(entry.Key);
						_logger.WarnFormat("[DoUpdateProfile]: ProfileProperty (Code={0}) is not handled", (ProfileProperty) entry.Key);
						break;
				}
			}

			if (flags != ProfilePropertyUpdateFlags.None)
				this.onProfileUpdate.Invoke(flags);
		}

		private void ExecItemOperation(Func<GameOperationResponse> operation, MessageParameters parameters)
		{
			var response = operation();
			if (response != null)
				this.SendOperationResponse(response, parameters);
		}

		#endregion
	}
}
