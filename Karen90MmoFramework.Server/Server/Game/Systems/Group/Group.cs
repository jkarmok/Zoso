using System;
using System.Collections;
using System.Collections.Generic;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.ServerToClient.Events;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public sealed class Group
	{
		#region Constants and Fields

		private readonly MmoWorld world;

		private readonly List<GroupMemberStructure> members;
		private readonly Dictionary<MmoGuid, Reference<IGroupMember>> memberReferences;

		private MmoGuid guid;
		private GroupMemberStructure leader;

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

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new <see cref="Group"/>
		/// </summary>
		public Group(MmoWorld world)
		{
			this.world = world;

			this.members = new List<GroupMemberStructure>(4); // 4 items = 4 cap, 5 items = 8 cap.
			this.memberReferences = new Dictionary<MmoGuid, Reference<IGroupMember>>(4);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Forms an existing group
		/// </summary>
		public bool FormExistingGroup(MmoGuid groupGuid, GroupMemberStructure groupLeader)
		{
			this.guid = groupGuid;
			this.leader = groupLeader;

			// making sure that we do not already have a group with the same id
			// if its already been added and its us then we dont have to do anything
			// otherwise throw error
			Group oldGroup;
			if (GroupManager.Instance.TryGetGroup(guid, out oldGroup))
			{
				if(oldGroup != this)
					throw new ArgumentException("ExistingGroupWithSameId");
				return true;
			}

			GroupManager.Instance.AddGroup(this);
			return true;
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

			var memberInfo = new GroupMemberStructure(memberToAdd.Guid, memberToAdd.Name);
			this.members.Add(memberInfo);
			
			// this will set the member's current group to this
			memberToAdd.Group = this;
			// only send updates if there are more than one player in the group
			// seriously what is there to send if its only one player
			// a group of one means its just the leader waiting for the other player to accept the invite
			if (memberReferences.Count > 0)
			{
				foreach (var memberReference in memberReferences.Values)
				{
					// skip us
					var member = memberReference.Object;
					var memberGuid = member.Guid;
					var memberToAddGuid = memberToAdd.Guid;

					if (memberGuid == memberToAddGuid)
						continue;
					
					if (!member.IsVisible(memberToAddGuid))
					{
						// otherwise we need to manually build the updates
						memberToAdd.MemberUpdateFlags = GroupMemberPropertyFlags.PROPERTY_FLAG_ALL;
						var properties = memberToAdd.BuildGroupMemberProperties();
						memberToAdd.MemberUpdateFlags = GroupMemberPropertyFlags.PROPERTY_FLAG_NONE;
						// send the updates only if there is properties to send
						if (properties != null)
						{
							var groupMemberUpdate = new GroupMemberUpdate
								{
									ObjectId = memberToAddGuid,
									Properties = properties
								};
							member.Peer.SendEvent(groupMemberUpdate, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
						}
					}
					
					if (!memberToAdd.IsVisible(memberGuid))
					{
						// otherwise we need to manually build the updates
						member.MemberUpdateFlags = GroupMemberPropertyFlags.PROPERTY_FLAG_ALL;
						var properties = member.BuildGroupMemberProperties();
						member.MemberUpdateFlags = GroupMemberPropertyFlags.PROPERTY_FLAG_NONE;
						// send the updates only if there is properties to send
						if (properties != null)
						{
							var groupMemberUpdate = new GroupMemberUpdate
								{
									ObjectId = memberGuid,
									Properties = properties
								};
							memberToAdd.Peer.SendEvent(groupMemberUpdate, new MessageParameters {ChannelId = PeerSettings.GroupEventChannel});
						}
					}
				}
			}
			
			// adding the member reference to send updates
			this.AddReference(memberToAdd);
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

			// removes the member's reference
			this.RemoveReference(memberToRemoveGuid);
			// removing the member from the profiles
			this.members.RemoveAll(m => m.Guid == memberToRemoveGuid);
			return true;
		}

		/// <summary>
		/// Tells whether the group is full or not
		/// </summary>
		public bool IsFull()
		{
			return members.Count >= GlobalGameSettings.MAX_GROUP_MEMBERS;
		}
		
		/// <summary>
		/// Deforms a group
		/// </summary>
		public void Deform()
		{
			foreach (var memberReference in memberReferences.Values)
			{
				var member = memberReference.Object;
				member.Group = null;
				memberReference.Dispose();
			}

			// remove us from the group manager
			GroupManager.Instance.RemoveGroup(this);

			this.memberReferences.Clear();
			this.members.Clear();
		}

		/// <summary>
		/// Removes a member's reference
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
			// setting up the disconnection subscription
			var disconnectSubscription = referenceMember.OnDisconnect.Subscribe(world.PrimaryFiber, () => this.Member_OnDisconnect(referenceMember.Guid));
			// setting up regular updates at a not-so-frequent interval
			// this way all members will receive updates from group members regardless of their current zone
			var updateSubscription = world.PrimaryFiber.ScheduleOnInterval(() => this.PublishGroupUpdateFrom(referenceMember), 2000);
			// adding the new reference
			var newReference = new Reference<IGroupMember>(referenceMember, updateSubscription, disconnectSubscription);
			this.memberReferences.Add(referenceMember.Guid, newReference);
			return true;
		}

		/// <summary>
		/// Removes a member's reference
		/// </summary>
		public bool RemoveReference(MmoGuid referenceGuid)
		{
			Reference<IGroupMember> reference;
			if (!memberReferences.TryGetValue(referenceGuid, out reference))
				return false;

			reference.Dispose();
			this.memberReferences.Remove(referenceGuid);
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
		/// Gets all the active members
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IGroupMember> GetActiveMembers()
		{
			return new MemberEnumerator(memberReferences.Values.GetEnumerator());
		}

		/// <summary>
		/// Adds xp for all the visible members
		/// </summary>
		public void GainXp(IGroupMember memberGained, int amount)
		{
			var actualXpGain = (int)(amount * ServerGameSettings.XP_MULTIPLIER_IN_GROUP);
			foreach (var reference in memberReferences.Values)
			{
				var member = reference.Object;
				if (member == memberGained)
					continue;

				// only add xp for players who are within the range of the xp gainer
				if (memberGained.IsVisible(memberGained.Guid))
					member.GainXp(actualXpGain);
			}

			memberGained.GainXp(actualXpGain);
		}
		
		#endregion

		#region Helper Methods

		/// <summary>
		/// Publishes a group update from a player
		/// </summary>
		void PublishGroupUpdateFrom(IGroupMember memberUpdated)
		{
			// if there is nothing to update just return
			if (memberUpdated.MemberUpdateFlags == GroupMemberPropertyFlags.PROPERTY_FLAG_NONE)
				return;
			
			GroupMemberUpdate groupMemberUpdate = null;
			foreach (var memberReference in memberReferences.Values)
			{
				// skip us
				var member = memberReference.Object;
				if (member.Guid == memberUpdated.Guid)
					continue;

				if (member.IsVisible(memberUpdated.Guid))
					continue;

				if (groupMemberUpdate == null)
					groupMemberUpdate = new GroupMemberUpdate { ObjectId = memberUpdated.Guid, Properties = memberUpdated.BuildGroupMemberProperties() };

				memberReference.Object.Peer.SendEvent(groupMemberUpdate, new MessageParameters { ChannelId = PeerSettings.GroupEventChannel });
			}

			// resetting the flags so the updates can resume from  the start
			memberUpdated.MemberUpdateFlags = GroupMemberPropertyFlags.PROPERTY_FLAG_NONE;
		}

		/// <summary>
		/// Called when a(n) group member has disconnected
		/// </summary>
		void Member_OnDisconnect(MmoGuid memberGuid)
		{
			this.RemoveReference(memberGuid);
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
