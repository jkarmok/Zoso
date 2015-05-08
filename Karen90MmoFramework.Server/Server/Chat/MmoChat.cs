using System;
using System.Threading;

using ExitGames.Concurrency.Fibers;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Concurrency;

namespace Karen90MmoFramework.Server.Chat
{
	public sealed class MmoChat : IDisposable
	{
		#region Constants and Fields

		public const int INVALID_CHAT_CHANNEL = 0;

		/// <summary>
		/// the chat server
		/// </summary>
		private readonly IChatServer server;

		/// <summary>
		/// the channels
		/// </summary>
		private readonly ConcurrentStorageMap<int, Channel> channels;

		/// <summary>
		/// the session cache
		/// </summary>
		private readonly ChatSessionCache sessionCache;

		/// <summary>
		/// the sync fiber
		/// </summary>
		private readonly ISerialFiber syncFiber;

		/// <summary>
		/// the channel counter
		/// </summary>
		private readonly Counter counter;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the singleton instance
		/// </summary>
		public static MmoChat Instance { get; private set; }

		/// <summary>
		/// Gets the server
		/// </summary>
		public IServer Server
		{
			get
			{
				return this.server;
			}
		}

		/// <summary>
		/// Gets the channels
		/// </summary>
		public IStorageMapAccessor<int, Channel> Channels
		{
			get
			{
				return this.channels;
			}
		}

		/// <summary>
		/// Gets the <see cref="SessionCache"/> which contains all the connected <see cref="ChatSession"/>s.
		/// </summary>
		public IChatSessionCacheAccessor SessionCache
		{
			get
			{
				return this.sessionCache;
			}
		}

		/// <summary>
		/// Gets the sync fiber
		/// </summary>
		public IFiber SyncFiber
		{
			get
			{
				return this.syncFiber;
			}
		}

		/// <summary>
		/// Tells whether the <see cref="MmoChat"/> is disposed or not
		/// </summary>
		public bool Disposed { get; private set; }
		
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="MmoChat"/> class.
		/// </summary>
		/// <param name="server"></param>
		private MmoChat(IChatServer server)
		{
			this.server = server;
			this.channels = new ConcurrentStorageMap<int, Channel>(PeerSettings.MaxLockWaitTime);
			this.sessionCache = new ChatSessionCache(PeerSettings.MaxLockWaitTime);
			this.counter = new Counter(INVALID_CHAT_CHANNEL+1);

			this.syncFiber = new SerialThreadFiber(ThreadPriority.Highest);
			this.syncFiber.Start();
		}

		/// <summary>
		/// Instantiates the singleton of the <see cref="MmoChat"/>. If it has already been instantiated returns the old instance.
		/// </summary>
		public static MmoChat Instantiate(IChatServer server)
		{
			lock (typeof(MmoChat))
				return Instance ?? (Instance = new MmoChat(server));
		}

		~MmoChat()
		{
			this.Dispose(false);
		}
		
		#endregion

		#region IDisposable Implementation

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Kicks and disposes all sessions and channels
		/// </summary>
		/// <param name="disposing"></param>
		void Dispose(bool disposing)
		{
			this.Disposed = true;
			if (disposing)
			{
				foreach (var session in sessionCache)
				{
					server.KillSession(session.SessionId);
					session.Destroy(DestroySessionReason.KickedByServer);
				}
				this.sessionCache.Dispose();

				foreach (var channel in this.channels)
					channel.Dispose();
				this.channels.Dispose();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a new <see cref="Channel"/>.
		/// </summary>
		public Channel CreateChannel(ChannelType type)
		{
			return CreateChannel(type, string.Empty);
		}

		/// <summary>
		/// Creates a new <see cref="Channel"/>.
		/// </summary>
		public Channel CreateChannel(ChannelType type, string name)
		{
			var channelId = this.counter.Next;
			var channel = new Channel(channelId, type, name);
			this.channels.Add(channelId, channel);
			return channel;
		}

		/// <summary>
		/// Removes a(n) <see cref="Channel"/>.
		/// </summary>
		/// <returns> Returns true if successful </returns>
		public bool RemoveChannel(Channel channel)
		{
			Channel theChannel;
			if (channels.TryGetValue(channel.Id, out theChannel))
			{
				if(theChannel == channel)
				{
					channels.Remove(theChannel.Id);
					theChannel.Dispose();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Adds a(n) <see cref="ChatSession"/> to the chat
		/// </summary>
		/// <param name="session"></param>
		public bool AddSession(ChatSession session)
		{
			// using a while loop for the lock-timeout
			while (!sessionCache.AddSession(session))
			{
				ChatSession existingSession;
				if (sessionCache.TryGetSessionBySessionName(session.Name, out existingSession))
					return false;
			}
			
			return true;
		}

		/// <summary>
		/// Removes a(n) <see cref="ChatSession"/> from the chat
		/// </summary>
		/// <param name="session"></param>
		public bool RemoveSession(ChatSession session)
		{
			// using a while loop for the lock-timeout
			while (!sessionCache.RemoveSession(session))
			{
				ChatSession existingSession;
				if (!sessionCache.TryGetSessionBySessionName(session.Name, out existingSession))
					return false;
			}

			return true;
		}

		#endregion
	}
}
