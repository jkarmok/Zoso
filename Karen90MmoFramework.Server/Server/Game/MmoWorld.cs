using System;
using System.Linq;
using System.Threading;

using Karen90MmoFramework.Concurrency;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Database;
using Karen90MmoFramework.Server.Config;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Server.Game
{
	public class MmoWorld : IDisposable, IClock
	{
		#region Constants and Fields

		public const short INVALID_WORLD_ID = 0;

		/// <summary>
		/// the world server
		/// </summary>
		private readonly IWorldServer server;

		/// <summary>
		/// the primary fiber
		/// </summary>
		private readonly ISerialFiber primaryFiber;

		/// <summary>
		/// the configuration
		/// </summary>
		private readonly GameConfig configuration;

		/// <summary>
		/// the mmo zones
		/// </summary>
		private readonly ConcurrentStorageMap<int, MmoZone> zones;

		/// <summary>
		/// the player cache
		/// </summary>
		private readonly WorldSessionCache sessionCache;

		/// <summary>
		/// the item cache
		/// </summary>
		private readonly WorldItemCache itemCache;

		/// <summary>
		/// world bounds
		/// </summary>
		private Bounds bounds;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the singleton instance
		/// </summary>
		public static MmoWorld Instance { get; private set; }

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
		/// Gets the world database
		/// </summary>
		public IDatabase WorldDatabase
		{
			get
			{
				return this.server.WorldDatabase;
			}
		}

		/// <summary>
		/// Gets the item database
		/// </summary>
		public IDatabase ItemDatabase
		{
			get
			{
				return this.server.ItemDatabase;
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
		/// Gets the social manager
		/// </summary>
		public ISocialManager SocialManager
		{
			get
			{
				return this.server.SocialManager;
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
		/// Gets the main fiber (game fiber) which controls the game.
		/// Only queue game tasks which require synchronization with world objects.
		/// </summary>
		public ISerialFiber PrimaryFiber
		{
			get
			{
				return this.primaryFiber;
			}
		}

		/// <summary>
		/// Gets the world zoneBounds
		/// </summary>
		public Bounds Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		/// <summary>
		/// Gets the <see cref="SessionCache"/> which contains all the connected <see cref="WorldSession"/>s.
		/// </summary>
		public IPlayerSessionCacheAccessor SessionCache
		{
			get
			{
				return this.sessionCache;
			}
		}

		/// <summary>
		/// Gets the <see cref="IStorageMapAccessor{TKey, TValue}"/> which contains all <see cref="MmoZone"/>s.
		/// </summary>
		public IStorageMapAccessor<int, MmoZone> Zones
		{
			get
			{
				return this.zones;
			}
		}

		/// <summary>
		/// Gets the <see cref="IWorldItemCacheAccessor"/> which contains all the item datas.
		/// </summary>
		public IWorldItemCacheAccessor ItemCache
		{
			get
			{
				return this.itemCache;
			}
		}

		/// <summary>
		/// Gets the global game time in milliseconds
		/// </summary>
		public int GlobalTime
		{
			get
			{
				return this.server.GlobalTime;
			}
		}

		/// <summary>
		/// Tells whether the <see cref="MmoWorld"/> is disposed or not
		/// </summary>
		public bool Disposed { get; private set; }
		
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="MmoWorld"/> class.
		/// </summary>
		/// <param name="server"> The main peer used for communication with other servers and clients. </param>
		/// <param name="gameConfiguration"> </param>
		/// <param name="worldDescription"> The world data which contains data of the whole world </param>
		private MmoWorld(IWorldServer server, GameConfig gameConfiguration, WorldDescription worldDescription)
		{
			this.server = server;
			// world bounds will adjust its size based on the zones
			this.bounds = new Bounds(Vector3.Zero, Vector3.Zero);
			this.configuration = gameConfiguration;

			this.primaryFiber = new SerialThreadFiber(ThreadPriority.Highest);
			this.primaryFiber.Start();
			
			this.zones = new ConcurrentStorageMap<int, MmoZone>(1000);
			this.sessionCache = new WorldSessionCache(PeerSettings.MaxLockWaitTime);
			this.itemCache = new WorldItemCache();

			this.LoadWorld();
		}

		/// <summary>
		/// Instantiates the singleton of the <see cref="MmoWorld"/>. If it has already been instantiated returns the old instance.
		/// </summary>
		public static MmoWorld Instantiate(IWorldServer server, GameConfig gameConfiguration, WorldDescription worldDescription)
		{
			lock (typeof(MmoWorld))
				return Instance ?? (Instance = new MmoWorld(server, gameConfiguration, worldDescription));
		}

		~MmoWorld()
		{
			this.Dispose(false);
		}

		#endregion

		#region IDisposable Implementation

		/// <summary>
		/// Kicks and disposes all sessions and zones
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Kicks and disposes all sessions and zones
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
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

				foreach (var mmoZone in zones)
					mmoZone.Dispose();
				this.zones.Dispose();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates a new <see cref="MmoZone"/>.
		/// </summary>
		public MmoZone CreateZone(short id, string name, Bounds zoneBounds, Vector3 tileDimentions, ZoneDescription zoneDescription)
		{
			MmoZone existingZone;
			if (zones.TryGetValue(id, out existingZone))
				return existingZone;

			// locking to update the bounds
			lock (zones)
			{
				this.bounds = this.bounds.Size != Vector3.Zero ? this.bounds.UnionWith(zoneBounds) : zoneBounds;
				var newZone = new MmoZone(this, id, name, zoneBounds, tileDimentions, zoneDescription, configuration);
				zones.Add(id, newZone);
				return newZone;
			}
		}

		/// <summary>
		/// Removes a(n) <see cref="MmoZone"/>.
		/// </summary>
		public bool RemoveZone(MmoZone zone)
		{
			MmoZone theZone;
			if (zones.TryGetValue(zone.Id, out theZone))
			{
				// TODO: Recalculate bounds
				// TODO: Disconnect every player from that zone
				if(theZone == zone)
				{
					zones.Remove(zone.Id);
					theZone.Dispose();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Adds a(n) <see cref="WorldSession"/> to the world
		/// </summary>
		/// <param name="session"></param>
		public bool AddSession(WorldSession session)
		{
			// using a while loop for the lock-timeout
			while (!sessionCache.AddSession(session))
			{
				WorldSession existingSession;
				if (sessionCache.TryGetSessionByPlayerGuid(session.Guid, out existingSession))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Removes a(n) <see cref="WorldSession"/> from the world
		/// </summary>
		/// <param name="session"></param>
		public bool RemoveSession(WorldSession session)
		{
			// using a while loop for the lock-timeout
			while (!sessionCache.RemoveSession(session))
			{
				WorldSession existingSession;
				if (!sessionCache.TryGetSessionByPlayerGuid(session.Guid, out existingSession))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Gets the message of the day
		/// </summary>
		/// <returns></returns>
		public string GetMotd()
		{
			return
				"Welcome to the game. Collision system and world to world systems are working but collision needs tweaking. Please send any bugs you find to stridervan79@yahoo.com and thank you for playing the game and your support.";
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Loads world
		/// </summary>
		void LoadWorld()
		{
			// TODO: Needs to be loaded in packs of 20 - 30
			using (var session = ItemDatabase.OpenSession())
			{
				// load all game items
				var gameItemDatas = session.Query<GameItemData>().ToArray();
				foreach (var gameItemData in gameItemDatas)
					this.itemCache.AddGameItemData(gameItemData.ItemId, gameItemData);

				// load all spells
				var spellDatas = session.Query<SpellData>().ToArray();
				foreach (var spellData in spellDatas)
					this.itemCache.AddSpellData(spellData.SpellId, spellData);
			}

			using (var session = WorldDatabase.OpenSession())
			{
				// load all quests
				var questDatas = session.Query<QuestData>().ToArray();
				foreach (var questData in questDatas)
					this.itemCache.AddQuestData(questData.QuestId, questData);

				// load all loot groups
				var lootGroupDatas = session.Query<LootGroupData>().ToArray();
				foreach (var lootGroupData in lootGroupDatas)
					this.itemCache.AddLootGroupData(lootGroupData.GroupId, lootGroupData);
			}
		}

		#endregion
	}
}
