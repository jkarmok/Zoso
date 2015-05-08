using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Social
{
	public interface IFriend
	{
		/// <summary>
		/// Gets the name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the level
		/// </summary>
		byte Level { get; }

		/// <summary>
		/// Gets the status
		/// </summary>
		SocialStatus Status { get; }

		/// <summary>
		/// Gets the current world id
		/// </summary>
		short CurrentWorldId { get; }

		/// <summary>
		/// Gets the async event to listen for status changes
		/// </summary>
		AsyncEvent<SocialStatus> OnStatusChange { get; }

		/// <summary>
		/// Gets the async event to listen for profile updates
		/// </summary>
		AsyncEvent<ProfilePropertyUpdateFlags> OnProfileUpdate { get; }

		/// <summary>
		/// Receives a friend login notification of a(n) <see cref="IFriend"/>
		/// </summary>
		/// <param name="friend"></param>
		void ReceiveFriendLoginNotification(IFriend friend);

		/// <summary>
		/// Receives a friend request from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="requester"></param>
		void ReceiveFriendRequest(IFriend requester);

		/// <summary>
		/// Receives an accepted request from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="receiver"></param>
		void ReceiveAcceptedRequest(IFriend receiver);

		/// <summary>
		/// Receives a declined request from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="receiver"></param>
		/// <param name="declineRequestReason"></param>
		void ReceiveDeclinedRequest(IFriend receiver, DeclineRequestReason declineRequestReason);

		/// <summary>
		/// Clears an request from a(n) <see cref="IFriend"/>.
		/// </summary>
		/// <param name="requester"></param>
		void ClearRequest(IFriend requester);

		/// <summary>
		/// Gets the profile info
		/// </summary>
		/// <returns></returns>
		ProfileStructure GetProfile();
	}
}
