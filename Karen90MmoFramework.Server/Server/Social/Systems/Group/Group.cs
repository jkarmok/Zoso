using System.Collections;
using System.Collections.Generic;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.ServerToClient.Events;

namespace Karen90MmoFramework.Server.Social.Systems
{
	public sealed class Group
	{
		#region Constants and Fields

		private readonly MmoSocial social;

		private readonly List<GroupMemberStructure> members;
		private readonly Dictionary<MmoGuid, Reference<IGroupMember>> memberReferences;
		private readonly List<IGroupMember> sentInvites;
		
		private MmoGuid guid;
		private GroupMemberStructure leader;

		private int channelId;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the guid.
		/// </summary>
		public MmoGuid Guid
		{
			get
			{
				return this.guid;
			}
		}
		
		/// <summary>
		/// Gets the leader
		/// </summary>
		public GroupMemberStructure Leader
		{
			get
			{
				return this.leader;
			}
		}

		/// <summary>
		/// Gets the member's count
		/// </summary>
		public int Count
		{
			get
			{
				return this.members.Count;
			}
		}

		/// <summary>
		/// Gets the invites count
		/// </summary>
		public int Invites
		{
			get
			{
				return this.sentInvites.Count;
			}
		}

		/// <summary>
		/// Tells whether the group is formed or not
		/// </summary>
		public bool IsFormed { get; private set; }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new <see cref="Group"/>
		/// </summary>
		public Group(MmoSocial social)
		{
			this.social = social;

			this.members = new List<GroupMemberStructure>(4); // 4 items = 4 cap, 5 items = 8 cap.
			this.memberReferences = new Dictionary<MmoGuid, Reference<IGroupMember>>(4);
			this.sentInvites = new List<IGroupMember>(4); // 4 items = 4 cap, 5 items = 8 cap.

			this.IsFormed = false;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Forms a new group
		/// </summary>
		public void FormNewGroup(IGroupMember groupLeader)
		{
			// we are using the current zone id as the sub id so group ids created in different zones will not collide with each other
			this.guid = new MmoGuid((byte) ObjectType.Group, GroupManager.Instance.GenerateGroupId());
			this.leader = new GroupMemberStructure(groupLeader.Guid, groupLeader.Name);

			// add the leader as our first member
			this.AddMember(groupLeader);
			// add the group to the group manager
			GroupManager.Instance.AddGroup(this);
		}

		/// <summary>
		/// Adds a(n) <see cref="IGroupMember"/> to the group.
		/// </summary>
		public bool AddMember(IGroupMember memberToAdd)
		{
			if (memberToAdd.InGroup())
				return false;
			
			if (members.Count >= GlobalGameSettings.MAX_GROUP_MEMBERS)
				return false;

			var memberToAddInfo = new GroupMemberStructure(memberToAdd.Guid, memberToAdd.Name);
			this.members.Add(memberToAddInfo);
			// this will set the member's current group to this
			memberToAdd.Group = this;

			// only send updates if there are more than one player in the group
			// a group of one means its just the leader waiting for the other player to accept the invite
			if (members.Count > 1)
			{
				// sending the player the group info so it can setup the group
				var groupInit = new GroupInit { GroupData = new GroupStructure(guid, leader.Guid) };
				memberToAdd.Peer.SendEvent(groupInit, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
				
				// if the group chat channel has not been registered yet, register it
				// doing it this way will only create the channel if there are more than one players in the group
				if(!IsFormed)
				{
					// creating the group in every worlds
					foreach (var world in social.Worlds)
						world.GroupManager.FormGroup(guid, leader);

					// TODO: Send it via an event channel
					// send any remaining players the init message (usually the leader)
					foreach (var memberReference in memberReferences.Values)
						memberReference.Object.Peer.SendEvent(groupInit, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
					// create our chat channel
					this.CreateChatChannel();
					// mark the group formed
					this.IsFormed = true;
				}

				// TODO: Send it via an event channel
				// adding the member in every world groups
				if (IsFormed)
					foreach (var world in social.Worlds)
						world.GroupManager.AddMember(guid, memberToAddInfo);

				foreach (var profile in members)
				{
					// skip us
					if (profile.Guid == memberToAdd.Guid)
						continue;

					Reference<IGroupMember> reference;
					if (memberReferences.TryGetValue(profile.Guid, out reference))
					{
						var activeMember = reference.Object;
						activeMember.Peer.SendEvent(
							new GroupMemberAdded
								{
									GroupMemberData = new GroupMemberStructure(memberToAdd.Guid.Id, memberToAdd.Name)
								},
							new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});

						memberToAdd.Peer.SendEvent(
							new GroupMemberAdded
								{
									GroupMemberData = new GroupMemberStructure(activeMember.Guid.Id, activeMember.Name)
								},
							new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
					}
					else
					{
						memberToAdd.Peer.SendEvent(
							new GroupMemberAddedInactive
								{
									GroupMemberData = new GroupMemberStructure(profile.Guid.Id, profile.Name)
								},
							new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
					}
				}
				
				// if the chat channel is available join the new user to it
				if (channelId > 0)
					this.social.ChatManager.JoinChannel(memberToAdd.SessionId, this.channelId);
			}

			// adding the member reference to send updates
			this.DoAddReference(memberToAdd);
			return true;
		}

		/// <summary>
		/// Adds a(n) <see cref="GroupMemberStructure"/> to the group.
		/// </summary>
		public bool AddMember(GroupMemberStructure memberToAddInfo)
		{
			if (members.Count >= GlobalGameSettings.MAX_GROUP_MEMBERS)
				return false;

			this.members.Add(memberToAddInfo);
			return true;
		}

		/// <summary>
		/// Removes a(n) <see cref="IGroupMember"/> from the group.
		/// </summary>
		public bool RemoveMember(MmoGuid memberToRemoveGuid)
		{
			// if the member does not exist, just return
			if (!members.Exists(m => m.Guid == memberToRemoveGuid))
				return false;

			// TODO: Send it via an event channel
			// removing the member in every world groups
			if (IsFormed)
				foreach (var world in social.Worlds)
					world.GroupManager.RemoveMember(guid, memberToRemoveGuid);

			Reference<IGroupMember> reference;
			if (memberReferences.TryGetValue(memberToRemoveGuid, out reference))
			{
				// the member is online send an uninvite event so the client will be notified that he or she has been removed from the group
				var memberToRemove = reference.Object;
				// send an uninvite event so the client will be notified he/she has been removed from the group
				memberToRemove.Peer.SendEvent(new GroupUninvited(), new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
				memberToRemove.Group = null;

				reference.Dispose();
				this.memberReferences.Remove(memberToRemoveGuid);
			}

			// removing the member from the profiles
			this.members.RemoveAll(m => m.Guid == memberToRemoveGuid);

			// if there are active members notify all of them
			if (memberReferences.Count > 0)
			{
				var groupMemberRemoved = new GroupMemberRemoved {ObjectId = memberToRemoveGuid};
				// let every online players know that a player has been removed
				foreach (var memberReference in memberReferences)
					memberReference.Value.Object.Peer.SendEvent(groupMemberRemoved, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
			}

			return true;
		}

		/// <summary>
		/// Sends group invite to a player
		/// </summary>
		public bool SendInviteTo(IGroupMember memberToInvite)
		{
			// player is already in a group
			if (memberToInvite.InGroup())
				return false;
			
			// if the player has already been invited or we already sent that player an invite
			if (memberToInvite.InvitingGroup != null || memberToInvite.InvitingGroup == this)
				return false;

			this.sentInvites.Add(memberToInvite);
			// setting the player's inviting group to this
			// so if another groups sees this it will ignore an invite to that player
			memberToInvite.InvitingGroup = this;
			// send the invitation request
			memberToInvite.Peer.SendEvent(new GroupInviteReceived {Inviter = leader.Name}, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
			return true;
		}

		/// <summary>
		/// Removes an invite from a player
		/// </summary>
		public void RemoveInvite(IGroupMember member, bool notifyClient)
		{
			// don't have an invite? just return
			if (!sentInvites.Remove(member))
				return;

			// notify the client if requested
			if (notifyClient)
				member.Peer.SendEvent(new GroupInviteCancelled(), new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});

			// resetting the inviting group
			// so other groups can invite that player
			member.InvitingGroup = null;
		}

		/// <summary>
		/// Clears all invitation
		/// </summary>
		public void ClearInvites(bool notifyClient)
		{
			foreach (var player in this.sentInvites)
			{
				// no need to notify to a disposed player
				if (notifyClient)
					player.Peer.SendEvent(new GroupInviteCancelled(), new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});

				// resetting the inviting group
				// so other groups can invite that player
				player.InvitingGroup = null;
			}

			this.sentInvites.Clear();
		}

		/// <summary>
		/// Tells whether the group is full or not
		/// </summary>
		public bool IsFull()
		{
			return this.members.Count >= GlobalGameSettings.MAX_GROUP_MEMBERS;
		}

		/// <summary>
		/// Disbands the group and send event to clients
		/// </summary>
		public void Disband(bool notifyClient)
		{
			// TODO: Send it via an event channel
			// removing the group from every worlds
			if (IsFormed)
				foreach (var world in social.Worlds)
					world.GroupManager.DisbandGroup(guid);

			foreach (var memberReference in memberReferences.Values)
			{
				var member = memberReference.Object;
				member.Group = null;
				memberReference.Dispose();

				if (notifyClient)
					member.Peer.SendEvent(new GroupDisbanded(), new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
			}

			// remove us from the group manager
			GroupManager.Instance.RemoveGroup(this);

			this.memberReferences.Clear();
			this.members.Clear();

			// remove the chat channel if its registered
			if (channelId > 0)
				this.social.ChatManager.RemoveChannel(channelId);
		}

		/// <summary>
		/// Adds reference to a(n) <see cref="IGroupMember"/>.
		/// </summary>
		public bool AddReference(IGroupMember referenceMember)
		{
			// if the reference is not in our member's list just return
			// this can happen if a member logs out before being kicked out and logs back in
			var referenceMemberGuid = referenceMember.Guid;
			if (!members.Exists(m => m.Guid == referenceMember.Guid))
				return false;
			// we need to re-check to make sure that there aren't any exisiting subscription for the member
			if (memberReferences.ContainsKey(referenceMemberGuid))
				return false;
			// setting the reference's group
			referenceMember.Group = this;
			// add the reference
			this.DoAddReference(referenceMember);
			// TODO: Send it via an event channel
			// notifying player online
			if (IsFormed)
				foreach (var world in social.Worlds)
					world.GroupManager.UpdateMemberStatus(guid, referenceMember.Guid, OnlineStatus.Online);
			return true;
		}

		/// <summary>
		/// Adds reference to a(n) <see cref="IGroupMember"/>.
		/// </summary>
		void DoAddReference(IGroupMember referenceMember)
		{
			// setting up the disconnection subscription
			var disconnectSubscription = referenceMember.OnDisconnect.Subscribe(social.PrimaryFiber, () => this.Member_OnDisconnect(referenceMember.Guid));
			// adding the new reference
			var newReference = new Reference<IGroupMember>(referenceMember, disconnectSubscription);
			this.memberReferences.Add(referenceMember.Guid, newReference);
		}

		/// <summary>
		/// Removes reference for a(n) <see cref="IGroupMember"/>.
		/// </summary>
		public bool RemoveReference(MmoGuid referenceGuid)
		{
			// making sure that the reference exists
			Reference<IGroupMember> reference;
			if (!memberReferences.TryGetValue(referenceGuid, out reference))
				return false;
			// removing and disposing the reference
			reference.Dispose();
			this.memberReferences.Remove(referenceGuid);

			// TODO: Send it via an event channel
			// notifying player offline
			if (IsFormed)
				foreach (var world in social.Worlds)
					world.GroupManager.UpdateMemberStatus(guid, referenceGuid, OnlineStatus.Offline);

			return true;
		}

		/// <summary>
		/// Gets all the group members
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GroupMemberStructure> GetAllMembers()
		{
			return this.members;
		}
		
		/// <summary>
		/// Gets only the active (online) members
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IGroupMember> GetActiveMembers()
		{
			return new MemberEnumerator(memberReferences.Values.GetEnumerator());
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Creates a chat channel for the group
		/// </summary>
		void CreateChatChannel()
		{
			// asking the chat server to create a channel for us
			this.social.ChatManager.CreateChannel(string.Empty, ChannelType.Group, id => this.social.PrimaryFiber.Enqueue(() => OnChannelRegistered(id)));
		}

		/// <summary>
		/// Called when the group chat channel is registered
		/// </summary>
		void OnChannelRegistered(int newChannelId)
		{
			foreach (var memberReference in memberReferences.Values)
				this.social.ChatManager.JoinChannel(memberReference.Object.SessionId, newChannelId);

			this.channelId = newChannelId;
		}

		/// <summary>
		/// Called when a(n) group member has disconnected
		/// </summary>
		void Member_OnDisconnect(MmoGuid memberGuid)
		{
			// making sure that the reference exists
			Reference<IGroupMember> reference;
			if (!memberReferences.TryGetValue(memberGuid, out reference))
				return;
			// removing and disposing the reference
			reference.Dispose();
			this.memberReferences.Remove(memberGuid);

			// TODO: Send it via an event channel
			// notifying player offline
			if (IsFormed)
				foreach (var world in social.Worlds)
					world.GroupManager.UpdateMemberStatus(guid, memberGuid, OnlineStatus.Offline);

			// if there are no active members, just return
			if (memberReferences.Count == 0)
				return;
			// send the member disconnected message to all group members
			var groupMemberDisconnected = new GroupMemberDisconnected {ObjectId = memberGuid};
			foreach (var memberReference in memberReferences)
				memberReference.Value.Object.Peer.SendEvent(groupMemberDisconnected, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
		}

		#endregion

		#region struct MemberEnumerator : IEnumerator<IGroupMember>, IEnumerable<IGroupMember>
		struct MemberEnumerator : IEnumerator<IGroupMember>, IEnumerable<IGroupMember>
		{
			#region Constants and Fields

			private readonly IEnumerator<Reference<IGroupMember>> enumerator;

			#endregion

			#region Constructors and Destructors

			public MemberEnumerator(IEnumerator<Reference<IGroupMember>> enumerator)
			{
				this.enumerator = enumerator;
			}

			#endregion

			#region Implementation of IDisposable

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose()
			{
				enumerator.Dispose();
			}

			#endregion

			#region Implementation of IEnumerator

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
			public bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
			public void Reset()
			{
				this.enumerator.Reset();
			}

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>
			/// The element in the collection at the current position of the enumerator.
			/// </returns>
			public IGroupMember Current
			{
				get
				{
					return this.enumerator.Current.Object;
				}
			}

			/// <summary>
			/// Gets the current element in the collection.
			/// </summary>
			/// <returns>
			/// The current element in the collection.
			/// </returns>
			/// <filterpriority>2</filterpriority>
			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			#endregion

			#region Implementation of IEnumerable

			/// <summary>
			/// Returns an enumerator that iterates through the collection.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
			/// </returns>
			/// <filterpriority>1</filterpriority>
			public IEnumerator<IGroupMember> GetEnumerator()
			{
				return this;
			}

			/// <summary>
			/// Returns an enumerator that iterates through a collection.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
			/// </returns>
			/// <filterpriority>2</filterpriority>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion
		}
		#endregion
	}
}
