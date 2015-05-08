using System;
using System.Threading;

using ExitGames.Concurrency.Fibers;

using Karen90MmoFramework.Concurrency;
using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Social
{
	public sealed class MmoSocial : IDisposable
	{
		#region Constants and Fields

		/// <summary>
		/// the social server
		/// </summary>
		private readonly ISocialServer server;

		/// <summary>
		/// the session cache
		/// </summary>
		private readonly SocialSessionCache sessionCache;

		/// <summary>
		/// the worlds
		/// </summary>
		private readonly ConcurrentStorageMap<short, IWorld> worlds;

		/// <summary>
		/// the primary fiber
		/// </summary>
		private readonly ISerialFiber primaryFiber;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the singleton instance
		/// </summary>
		public static MmoSocial Instance { get; private set; }

		/// <summary>
		/// Gets the character database
		/// </summary>
		public IDatabase CharacterDatabase
		{
			get
			{
				return this.server.CharacterDatabase;
			}
		}

		/// <summary>
		/// Gets the chat manager
		/// </summary>
		public IChatManager ChatManager
		{
			get
			{
				return this.server.ChatManager;
			}
		}

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
		/// Gets the <see cref="SessionCache"/> which contains all the connected <see cref="SocialSession"/>s.
		/// </summary>
		public ISocialSessionCacheAccessor SessionCache
		{
			get
			{
				return this.sessionCache;
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IStorageMapAccessor{TKey, TValue}"/> which contains all <see cref="IWorld"/>s.
		/// </summary>
		public IStorageMapAccessor<short, IWorld> Worlds
		{
			get
			{
				return this.worlds;
			}
		}

		/// <summary>
		/// Gets the primary fiber
		/// </summary>
		public IFiber PrimaryFiber
		{
			get
			{
				return this.primaryFiber;
			}
		}

		/// <summary>
		/// Tells whether the <see cref="MmoSocial"/> is disposed or not
		/// </summary>
		public bool Disposed { get; private set; }
		
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="MmoSocial"/> class.
		/// </summary>
		/// <param name="server"></param>
		private MmoSocial(ISocialServer server)
		{
			this.server = server;
			this.sessionCache = new SocialSessionCache(PeerSettings.MaxLockWaitTime);
			this.worlds = new ConcurrentStorageMap<short, IWorld>(-1);

			this.primaryFiber = new SerialThreadFiber(ThreadPriority.Highest);
			this.primaryFiber.Start();
		}

		/// <summary>
		/// Instantiates the singleton of the <see cref="MmoSocial"/>. If it has already been instantiated returns the old instance.
		/// </summary>
		public static MmoSocial Instantiate(ISocialServer server)
		{
			lock (typeof(MmoSocial))
				return Instance ?? (Instance = new MmoSocial(server));
		}

		~MmoSocial()
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
				foreach (var session in this.sessionCache)
				{
					this.server.KillSession(session.SessionId);
					session.Destroy(DestroySessionReason.KickedByServer);
				}
				this.sessionCache.Dispose();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a(n) <see cref="SocialSession"/> to the social
		/// </summary>
		/// <param name="session"></param>
		public bool AddSession(SocialSession session)
		{
			// using a while loop for the lock-timeout
			while (!this.sessionCache.AddSession(session))
			{
				SocialSession existingSession;
				if (this.sessionCache.TryGetSessionBySessionName(session.Name, out existingSession))
					return false;
			}
			
			return true;
		}

		/// <summary>
		/// Removes a(n) <see cref="SocialSession"/> from the social
		/// </summary>
		/// <param name="session"></param>
		public bool RemoveSession(SocialSession session)
		{
			// using a while loop for the lock-timeout
			while (!this.sessionCache.RemoveSession(session))
			{
				SocialSession existingSession;
				if (!this.sessionCache.TryGetSessionBySessionName(session.Name, out existingSession))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Adds a(n) <see cref="IWorld"/> to the social
		/// </summary>
		public bool AddWorld(short worldId, IWorld world)
		{
			// using a while loop for the lock-timeout
			while (!worlds.Add(worldId, world))
			{
				IWorld existingWorld;
				if (worlds.TryGetValue(worldId, out existingWorld))
					return false;
			}

			return true;
		}
		
		/// <summary>
		/// Removes a(n) <see cref="IWorld"/> from the social
		/// </summary>
		public bool RemoveWorld(short worldId)
		{
			// using a while loop for the lock-timeout
			while (!worlds.Remove(worldId))
			{
				IWorld existingWorld;
				if (!worlds.TryGetValue(worldId, out existingWorld))
					return false;
			}

			return true;
		}

		#endregion
	}
}
