namespace Karen90MmoFramework.Server.Chat
{
	public class ChannelMessage
	{
		/// <summary>
		/// the channel
		/// </summary>
		private readonly IChannel channel;

		/// <summary>
		/// the publisher
		/// </summary>
		private readonly IPublisher publisher;

		/// <summary>
		/// the message
		/// </summary>
		private readonly string message;

		/// <summary>
		/// Creates a new instance of the <see cref="ChannelMessage"/> class.
		/// </summary>
		/// <param name="channel"> </param>
		/// <param name="publisher"> </param>
		/// <param name="message"></param>
		public ChannelMessage(IChannel channel, IPublisher publisher, string message)
		{
			this.channel = channel;
			this.publisher = publisher;
			this.message = message;
		}

		/// <summary>
		/// Gets the channel
		/// </summary>
		public IChannel Channel
		{
			get
			{
				return this.channel;
			}
		}

		/// <summary>
		/// Gets the publisher
		/// </summary>
		public IPublisher Publisher
		{
			get
			{
				return this.publisher;
			}
		}

		/// <summary>
		/// Gets the message
		/// </summary>
		public string Message
		{
			get
			{
				return this.message;
			}
		}
	}
}
