using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Chat
{
	public interface IChannel
	{
		/// <summary>
		/// Gets the channel id
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Channel Name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the channel type
		/// </summary>
		ChannelType Type { get; }

		/// <summary>
		/// Joins a(n) <see cref="IListener"/> on the channel
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		bool Join(IListener listener);

		/// <summary>
		/// Leaves a(n) <see cref="IListener"/> from the channel
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		bool Leave(IListener listener);

		/// <summary>
		/// Publishes a message to be received by all subscribed <see cref="IListener"/>s.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="publisher"> </param>
		void Publish(string message, IPublisher publisher);
	}
}
