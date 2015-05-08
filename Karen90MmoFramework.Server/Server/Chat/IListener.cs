namespace Karen90MmoFramework.Server.Chat
{
	public interface IListener
	{
		/// <summary>
		/// Gets the listener name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Receives a chat message
		/// </summary>
		/// <param name="message"></param>
		void ReceiveMessage(ChannelMessage message);

		/// <summary>
		/// Called after joining the channel
		/// </summary>
		/// <param name="channel"></param>
		void OnJoinedChannel(IChannel channel);

		/// <summary>
		/// Called after leaving a channel
		/// </summary>
		/// <param name="channel"></param>
		void OnLeftChannel(IChannel channel);
	}
}
