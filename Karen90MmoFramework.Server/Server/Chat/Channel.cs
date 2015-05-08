using System;
using System.Threading;
using System.Collections.Generic;

using ExitGames.Concurrency.Channels;
using ExitGames.Concurrency.Fibers;

using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Chat
{
	public sealed class Channel : IDisposable, IChannel
	{
		#region Constants and Fields

		/// <summary>
		/// channel id
		/// </summary>
		private readonly int id;

		/// <summary>
		/// channel name
		/// </summary>
		private readonly string name;

		/// <summary>
		/// channel type
		/// </summary>
		private readonly ChannelType type;
	
		/// <summary>
		/// the fiber
		/// </summary>
		private readonly IFiber fiber;

		/// <summary>
		/// the message channel
		/// </summary>
		private readonly Channel<ChannelMessage> messageChannel;

		/// <summary>
		/// holds all listener subscriptions
		/// </summary>
		private readonly Dictionary<IListener, IDisposable> listenerSubscriptions;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the channel id
		/// </summary>
		public int Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Channel Name
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the channel type
		/// </summary>
		public ChannelType Type
		{
			get
			{
				return this.type;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="Channel"/> class.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		internal Channel(int id, ChannelType type)
			: this(id, type, string.Empty)
		{
		}

		/// <summary>
		/// Creates a new instance of the <see cref="Channel"/> class.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		/// <param name="name"> </param>
		internal Channel(int id, ChannelType type, string name)
		{
			this.id = id;
			this.type = type;
			this.name = name;

			this.fiber = new PoolFiber();
			this.fiber.Start();

			this.messageChannel = new Channel<ChannelMessage>();
			this.listenerSubscriptions = new Dictionary<IListener, IDisposable>();
		}

		~Channel()
		{
			this.Dispose(false);
		}

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (listenerSubscriptions)
				{
					foreach (var listenerSubscription in listenerSubscriptions.Values)
						listenerSubscription.Dispose();
					this.listenerSubscriptions.Clear();
				}

				this.messageChannel.ClearSubscribers();
				this.fiber.Dispose();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Joins a(n) <see cref="IListener"/> on the channel
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public bool Join(IListener listener)
		{
			Monitor.Enter(listenerSubscriptions);
			try
			{
				IDisposable exisitingSubscription;
				if (listenerSubscriptions.TryGetValue(listener, out exisitingSubscription))
					return false;

				var messageSubscription = this.messageChannel.Subscribe(fiber, listener.ReceiveMessage);
				this.listenerSubscriptions.Add(listener, messageSubscription);
				listener.OnJoinedChannel(this);
				return true;
			}
			finally
			{
				Monitor.Exit(listenerSubscriptions);
			}
		}

		/// <summary>
		/// Leaves a(n) <see cref="IListener"/> from the channel
		/// </summary>
		/// <param name="listener"></param>
		/// <returns></returns>
		public bool Leave(IListener listener)
		{
			Monitor.Enter(listenerSubscriptions);
			try
			{
				IDisposable subscription;
				if (!listenerSubscriptions.TryGetValue(listener, out subscription))
					return false;

				subscription.Dispose();
				this.listenerSubscriptions.Remove(listener);
				listener.OnLeftChannel(this);
				return true;
			}
			finally
			{
				Monitor.Exit(listenerSubscriptions);
			}
		}

		/// <summary>
		/// Publishes a message to be received by all subscribed <see cref="IListener"/>s.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="publisher"> </param>
		public void Publish(string message, IPublisher publisher)
		{
			this.messageChannel.Publish(new ChannelMessage(this, publisher, message));
		}

		#endregion
	}
}
