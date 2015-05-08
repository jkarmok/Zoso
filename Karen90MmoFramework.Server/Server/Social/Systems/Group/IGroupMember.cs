namespace Karen90MmoFramework.Server.Social.Systems
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
		/// Gets the session id
		/// </summary>
		int SessionId { get; }

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
		/// <returns></returns>
		bool InGroup();

		/// <summary>
		/// Gets or sets the current group
		/// </summary>
		Group Group { get; set; }

		/// <summary>
		/// Tells whether the member is invited or not
		/// </summary>
		/// <returns></returns>
		bool IsInvited();

		/// <summary>
		/// Gets or sets the inviting group
		/// </summary>
		Group InvitingGroup { get; set; }
	}
}
