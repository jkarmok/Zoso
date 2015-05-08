using System.Collections.Generic;

namespace Karen90MmoFramework.Server.Social.Systems
{
	/// <summary>
	/// The group manager class holds all the groups which have been created and not destroyed. Use this call to allow access to finding a certain <see cref="Group"/>.
	/// The group manager does not remove disbanded <see cref="Group"/>s. It has to be managed by the group or the player.
	/// </summary>
	public class GroupManager
	{
		#region Constants and Fields

		private static readonly GroupManager _instance = new GroupManager();

		private readonly Dictionary<MmoGuid, Group> groups;
		private readonly Counter counter;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the instance
		/// </summary>
		public static GroupManager Instance
		{
			get
			{
				return _instance;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new <see cref="GroupManager"/>.
		/// </summary>
		private GroupManager()
		{
			this.groups = new Dictionary<MmoGuid, Group>();
			this.counter = new Counter(1);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a group
		/// </summary>
		public void AddGroup(Group group)
		{
			lock (this.groups)
			{
				if (this.groups.Remove(group.Guid))
					Utils.Logger.WarnFormat("Group {0} Already exists", group.Guid);

				this.groups.Add(group.Guid, group);
			}
		}

		/// <summary>
		/// Removes a group
		/// </summary>
		public void RemoveGroup(Group group)
		{
			lock (this.groups)
				this.groups.Remove(group.Guid);
		}

		/// <summary>
		/// Tries to retrieve a group
		/// </summary>
		public bool TryGetGroup(MmoGuid guid, out Group group)
		{
			lock (this.groups)
				return this.groups.TryGetValue(guid, out group);
		}

		/// <summary>
		/// Generates a new group id
		/// </summary>
		public int GenerateGroupId()
		{
			return this.counter.Next;
		}

		#endregion
	}
}
