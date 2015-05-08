using System.Collections;

namespace Karen90MmoFramework.Server.Game.Systems
{
	// ReSharper disable ParameterHidesMember
	public class GroupMemberReference : WorldSessionReference, IGroupMember
	{
		#region Constants and Fields

		private readonly MmoGuid guid;
		private readonly AsyncEvent onDisconnect;

		#endregion

		#region Constructors and Destructors

		public GroupMemberReference(MmoGuid guid, int sessionId, string name, IServer server)
			: base(sessionId, name, server)
		{
			this.guid = guid;
			this.onDisconnect = new AsyncEvent();
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
				return this.guid;
			}
		}

		/// <summary>
		/// Gets the session
		/// </summary>
		public IPeer Peer
		{
			get
			{
				return this;
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
		/// Tells whether the member is in a group or not
		/// </summary>
		/// <returns></returns>
		public bool InGroup()
		{
			return Group != null;
		}

		/// <summary>
		/// Gets or sets the current group
		/// </summary>
		public Group Group { get; set; }

		/// <summary>
		/// Gets or sets the member update flags
		/// </summary>
		public GroupMemberPropertyFlags MemberUpdateFlags
		{
			get
			{
				return GroupMemberPropertyFlags.PROPERTY_FLAG_NONE;
			}

			set { }
		}

		/// <summary>
		/// Builds the group member properties or <value>NULL</value> if there is none.
		/// </summary>
		public Hashtable BuildGroupMemberProperties()
		{
			return null;
		}

		/// <summary>
		/// Tells whether a(n) object with the <see cref="guid"/> is visible for the <see cref="IGroupMember"/> or not.
		/// </summary>
		public bool IsVisible(MmoGuid guid)
		{
			return false;
		}

		/// <summary>
		/// Gains xp
		/// </summary>
		public void GainXp(int amount)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
