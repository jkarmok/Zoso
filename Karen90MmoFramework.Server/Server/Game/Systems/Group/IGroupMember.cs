using System.Collections;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface IGroupMember
	{
		/// <summary>
		/// Gets the name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the guid
		/// </summary>
		MmoGuid Guid { get; }

		/// <summary>
		/// Gets the peer
		/// </summary>
		IPeer Peer { get; }

		/// <summary>
		/// Gets the async event to listen for disconnects
		/// </summary>
		AsyncEvent OnDisconnect { get; }

		/// <summary>
		/// Tells whether the member is in a group or not
		/// </summary>
		bool InGroup();

		/// <summary>
		/// Gets or sets the current group
		/// </summary>
		Group Group { get; set; }

		/// <summary>
		/// Gets or sets the member update flags
		/// </summary>
		GroupMemberPropertyFlags MemberUpdateFlags { get; set; }

		/// <summary>
		/// Builds the group member properties or <value>NULL</value> if there is none.
		/// </summary>
		Hashtable BuildGroupMemberProperties();

		/// <summary>
		/// Tells whether a(n) object with the <see cref="guid"/> is visible for the <see cref="IGroupMember"/> or not.
		/// </summary>
		bool IsVisible(MmoGuid guid);

		/// <summary>
		/// Gains xp
		/// </summary>
		void GainXp(int amount);
	}
}
