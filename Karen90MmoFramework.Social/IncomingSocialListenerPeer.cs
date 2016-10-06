using System.Collections.Generic;

using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;
using Karen90MmoFramework.Server.ServerToServer.Events;

namespace Karen90MmoFramework.Server.Social
{
	public sealed class IncomingSocialListenerPeer : ClientPeer, IWorld, IGroupManager
	{
		#region Constants and Fields

		/// <summary>
		/// the logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// the application
		/// </summary>
		private readonly SocialApplication application;

		/// <summary>
		/// the social
		/// </summary>
		private readonly MmoSocial social;

		/// <summary>
		/// contains all listening worlds
		/// </summary>
		private readonly List<short> listeningWorlds;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="IncomingSocialListenerPeer"/> class.
		/// </summary>
		/// <param name="initRequest"></param>
		/// <param name="application"> </param>
		public IncomingSocialListenerPeer(InitRequest initRequest, SocialApplication application)
			: base(initRequest)
		{
			this.application = application;
			this.social = MmoSocial.Instance;
			this.listeningWorlds = new List<short>();
		}

		#endregion

		#region PeerBase Implementation

		protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
		{
			lock (listeningWorlds)
				foreach (var listeningWorld in listeningWorlds)
					social.RemoveWorld(listeningWorld);
            
		}

		protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
		{
			OperationResponse response;
			switch ((ServerOperationCode)operationRequest.OperationCode)
			{
				case ServerOperationCode.RegisterWorld:
					response = this.HandleOperationRegisterWorld(operationRequest);
					break;

				case ServerOperationCode.UnregisterWorld:
					response = this.HandleOperationUnregisterWorld(operationRequest);
					break;

				case ServerOperationCode.UpdateSocialProfile:
					response = this.HandleOperationUpdateSocialProfile(operationRequest);
					break;

				case ServerOperationCode.JoinWorld:
					response = this.HandleOperationJoinWorld(operationRequest);
					break;

				case ServerOperationCode.LeaveWorld:
					response = this.HandleOperationLeaveWorld(operationRequest);
					break;

				default:
					response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.OperationNotAvailable };
					break;
			}

			if (response != null)
				this.SendOperationResponse(response, new SendParameters());
		}

		#endregion

		#region Operation Handlers

		private OperationResponse HandleOperationRegisterWorld(OperationRequest operationRequest)
		{
			var operation = new RegisterWorld(Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };

			if (!social.AddWorld(operation.WorldId, this))
				return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.AlreadyRegistered };
			
			lock (listeningWorlds)
				this.listeningWorlds.Add(operation.WorldId);
			return null;
		}

		private OperationResponse HandleOperationUnregisterWorld(OperationRequest operationRequest)
		{
			var operation = new UnregisterWorld(Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };

			lock (listeningWorlds)
				if (!listeningWorlds.Remove(operation.WorldId))
					return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.InvalidRegisterationCredentials };

			social.RemoveWorld(operation.WorldId);
			return null;
		}

		private OperationResponse HandleOperationUpdateSocialProfile(OperationRequest operationRequest)
		{
			var operation = new UpdateSocialProfile(Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };

			SocialSession session;
			if (social.SessionCache.TryGetSessionBySessionId(operation.SessionId, out session))
				session.UpdateProfile(operation.Properties);
			return null;
		}

		private OperationResponse HandleOperationJoinWorld(OperationRequest operationRequest)
		{
			var operation = new JoinWorld(Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };

			SocialSession session;
			if (social.SessionCache.TryGetSessionBySessionId(operation.SessionId, out session))
				session.JoinWorld(operation.WorldId);
			return null;
		}

		private OperationResponse HandleOperationLeaveWorld(OperationRequest operationRequest)
		{
			var operation = new LeaveWorld(Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };

			SocialSession session;
			if (social.SessionCache.TryGetSessionBySessionId(operation.SessionId, out session))
				session.LeaveWorld(operation.WorldId);
			return null;
		}

		#endregion

		#region Implementation of IWorld

		/// <summary>
		/// Gets the group manager
		/// </summary>
		IGroupManager IWorld.GroupManager
		{
			get
			{
				return this;
			}
		}

		#endregion

		#region Implementation of IGroupManager

		void IGroupManager.FormGroup(MmoGuid groupGuid, GroupMemberStructure leader)
		{
			var groupFormed = new GroupFormed
				{
					GroupGuid = groupGuid,
					LeaderInfo = leader,
				};
			this.SendEvent(new EventData((byte) ServerEventCode.GroupFormed, groupFormed), new SendParameters());
			this.DoAddMember(groupGuid, leader);
		}

		void IGroupManager.DisbandGroup(MmoGuid groupGuid)
		{
			var groupDisbanded = new GroupDisbanded
				{
					GroupGuid = groupGuid,
				};
			this.SendEvent(new EventData((byte) ServerEventCode.GroupDisbanded, groupDisbanded), new SendParameters());
		}

		void IGroupManager.AddMember(MmoGuid groupGuid, GroupMemberStructure member)
		{
			this.DoAddMember(groupGuid, member);
		}

		void DoAddMember(MmoGuid groupGuid, GroupMemberStructure member)
		{
			SocialSession session;
			if (social.SessionCache.TryGetSessionBySessionName(member.Name, out session))
			{
				var groupMemberAddedSession = new GroupMemberAddedSession
					{
						GroupGuid = groupGuid,
						MemberInfo = member,
						SessionId = session.SessionId
					};
				this.SendEvent(new EventData((byte) ServerEventCode.GroupMemberAddedSession, groupMemberAddedSession), new SendParameters());
			}
			else
			{
				var groupMemberAdded = new GroupMemberAdded
					{
						GroupGuid = groupGuid,
						MemberInfo = member,
					};
				this.SendEvent(new EventData((byte) ServerEventCode.GroupMemberAdded, groupMemberAdded), new SendParameters());
			}
		}

		void IGroupManager.RemoveMember(MmoGuid groupGuid, MmoGuid memberGuid)
		{
			var groupMemberRemoved = new GroupMemberRemoved
				{
					GroupGuid = groupGuid,
					MemberGuid = memberGuid,
				};
			this.SendEvent(new EventData((byte) ServerEventCode.GroupMemberRemoved, groupMemberRemoved), new SendParameters());
		}

		void IGroupManager.UpdateMemberStatus(MmoGuid groupGuid, MmoGuid memberGuid, OnlineStatus status)
		{
			switch (status)
			{
				case OnlineStatus.Online:
					var groupMemberOnline = new GroupMemberOnline
						{
							GroupGuid = groupGuid,
							MemberGuid = memberGuid,
						};
					this.SendEvent(new EventData((byte) ServerEventCode.GroupMemberOnline, groupMemberOnline), new SendParameters());
					break;

				case OnlineStatus.Offline:
					var groupMemberOffline = new GroupMemberOffline
						{
							GroupGuid = groupGuid,
							MemberGuid = memberGuid,
						};
					this.SendEvent(new EventData((byte) ServerEventCode.GroupMemberOffline, groupMemberOffline), new SendParameters());
					break;
			}
		}

		#endregion
	}
}
