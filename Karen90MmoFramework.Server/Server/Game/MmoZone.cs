using System;
using System.Linq;
using System.Threading;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Quantum.Geometry;
using Karen90MmoFramework.Concurrency;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Config;
using Karen90MmoFramework.Server.Game.Objects;
using Karen90MmoFramework.Server.Game.Physics;
using Karen90MmoFramework.Server.Game.Physics.Quantum;
using Karen90MmoFramework.Server.Game.Physics.Rune;
using ThreadPriority = System.Threading.ThreadPriority;

namespace Karen90MmoFramework.Server.Game
{
	public class MmoZone : GridZone
	{
		#region Constants and Fields

		/// <summary>
		/// the id
		/// </summary>
		private readonly short id;

		/// <summary>
		/// the name
		/// </summary>
		private readonly string name;

		/// <summary>
		/// the world
		/// </summary>
		private readonly MmoWorld world;

		/// <summary>
		/// object cache
		/// </summary>
		private readonly WorldObjectCache objectCache;

		/// <summary>
		/// the dynamic object pool
		/// </summary>
		private readonly ObjectPool<short, Dynamic> dynamicObjectPool;

		/// <summary>
		/// the physics
		/// </summary>
		private readonly IPhysicsWorld physics;

		/// <summary>
		/// the heights
		/// </summary>
		private readonly IHeightField heightField;

		/// <summary>
		/// chat channels
		/// </summary>
		private int localChatChannel, tradeChatChannel;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the zone name
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the zone id
		/// </summary>
		public short Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the world
		/// </summary>
		public MmoWorld World
		{
			get
			{
				return this.world;
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
				return world.PrimaryFiber;
			}
		}

		/// <summary>
		/// Gets the physics
		/// </summary>
		public IPhysicsWorld Physics
		{
			get
			{
				return this.physics;
			}
		}

		/// <summary>
		/// Gets the <see cref="IWorldObjectCacheAccessor"/> which contains all the <see cref="MmoObject"/>s active in the world.
		/// </summary>
		public IWorldObjectCacheAccessor ObjectCache
		{
			get
			{
				return this.objectCache;
			}
		}

		/// <summary>
		/// Gets the <see cref="ObjectPool{TKey, TValue}"/> which contains all the pooled <see cref="Dynamic"/> objects.
		/// </summary>
		public ObjectPool<short, Dynamic> DynamicObjectPool
		{
			get
			{
				return dynamicObjectPool;
			}
		}

		/// <summary>
		/// Gets the local chat channel id
		/// </summary>
		public int LocalChatChannel
		{
			get
			{
				return this.localChatChannel;
			}

			private set
			{
				Interlocked.Exchange(ref localChatChannel, value);
			}
		}

		/// <summary>
		/// Gets the trade chat channel id
		/// </summary>
		public int TradeChatChannel
		{
			get
			{
				return this.tradeChatChannel;
			}

			private set
			{
				Interlocked.Exchange(ref tradeChatChannel, value);
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="MmoZone"/> class.
		/// </summary>
		/// <param name="world"> The world </param>
		/// <param name="name"> The name of the <see cref="MmoZone"/>. Does not have to be unique, this will be sent to the client. </param>
		/// <param name="id"> A unique id used to distinguish between other <see cref="MmoZone"/>s. Must be unique in order to avoid id conflicts. </param>
		/// <param name="tileDimentions"> </param>
		/// <param name="zoneDescription"> The zone data which contains data of the current zone </param>
		/// <param name="bounds"> </param>
		/// <param name="configuration"> </param>
		/// <remarks>
		/// The <see cref="MmoZone"/> uses an actual coordinate system, meaning x is right, z is forward and y is up always.
		/// </remarks>
		internal MmoZone(MmoWorld world, short id, string name, Bounds bounds, Vector3 tileDimentions, ZoneDescription zoneDescription, GameConfig configuration)
			: base(bounds, tileDimentions)
		{
			this.world = world;
			this.name = name;
			this.id = id;

			this.objectCache = new WorldObjectCache(PeerSettings.MaxLockWaitTime);
			this.dynamicObjectPool = new ObjectPool<short, Dynamic>();

			this.localChatChannel = Chat.MmoChat.INVALID_CHAT_CHANNEL;
			this.tradeChatChannel = Chat.MmoChat.INVALID_CHAT_CHANNEL;

			var physicsEngineConfig = configuration.physics.engine;
			switch (physicsEngineConfig.type.ToLower())
			{
				case "digitalrune":
					this.physics = new RunePhysicsWorld(Bounds, physicsEngineConfig.userObject);
					break;

				default:
					this.physics = new QuantumPhysicsWorld(Bounds, this);
					break;
			}

			var terrainWidthX = zoneDescription.TerrainWidthX;
			var terrainWidthZ = zoneDescription.TerrainWidthZ;

			var heights = new float[terrainWidthX, terrainWidthZ];
			for (var z = 0; z < terrainWidthZ; z++)
				for (var x = 0; x < terrainWidthX; x++)
					// we need to load it in reverse order
					heights[x, z] = zoneDescription.Heights[z * terrainWidthX + x];

			var heightFieldDescription = new HeightFieldDescription
				{
					Heights = heights,
					Position = Vector3.Zero,
					WidthX = terrainWidthX,
					WidthZ = terrainWidthZ
				};
			this.heightField = physics.CreateHeightField(heightFieldDescription).Shape as IHeightField;
			if (heightField == null)
				throw new NullReferenceException("heightField");

			// loading all box colliders
			var boxColliders = zoneDescription.Colliders.BoxColliders;
			if (boxColliders != null)
				foreach (var boxCollider in boxColliders)
					physics.CreateWorldObject(boxCollider, CollisionHelper.WorldObjectColliderDescription);

			// loading all sphere colliders
			var sphereColliders = zoneDescription.Colliders.SphereColliders;
			if (sphereColliders != null)
				foreach (var sphereCollider in sphereColliders)
					physics.CreateWorldObject(sphereCollider, CollisionHelper.WorldObjectColliderDescription);

			// loading all capsule colliders
			var capsuleColliders = zoneDescription.Colliders.CapsuleColliders;
			if (capsuleColliders != null)
				foreach (var capsuleCollider in capsuleColliders)
					physics.CreateWorldObject(capsuleCollider, CollisionHelper.WorldObjectColliderDescription);
#if USE_PHYSICS
			// setting up the physics thread
			new Thread(o =>
				{
					var timer = System.Diagnostics.Stopwatch.StartNew();
					timer.Start();
					
					while (true)
					{
						// sleeping for 16 ms ~ 1 / 60 s
						Thread.Sleep(16);
						// updating the physics engine
						this.physics.Update(timer.Elapsed);
						timer.Restart();
					}
				}) { Priority = ThreadPriority.AboveNormal, IsBackground = true }.Start(this);
#endif
			// load all npc, objects, items, quest, loot, etc
			this.PrimaryFiber.Enqueue(LoadZone);
#if MMO_DEBUG
			Utils.Logger.DebugFormat("Zone (Name={0}:Min={1}:Max={2}) created in World (Min={3}:Max={4}", this.Name, Bounds.Min, Bounds.Max, World.Bounds.Min,
									 World.Bounds.Max);
#endif
		}

		#endregion
		
		#region Public Methods

		/// <summary>
		/// Adds a(n) <see cref="MmoObject"/> to the <see cref="MmoZone"/>.
		/// </summary>
		/// <returns></returns>
		public bool Add(MmoObject mmoObject)
		{
			// using a while loop for the lock-timeout
			while (!objectCache.AddItem(mmoObject))
			{
				MmoObject existingObject;
				if (objectCache.TryGetItem(mmoObject.Guid, out existingObject))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Removes a(n) <see cref="MmoObject"/> from the <see cref="MmoZone"/>.
		/// </summary>
		/// <returns></returns>
		public bool Remove(MmoObject mmoObject)
		{
			while(!objectCache.RemoveItem(mmoObject.Guid))
			{
				MmoObject existingObject;
				if (!objectCache.TryGetItem(mmoObject.Guid, out existingObject))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Removes a(n) <see cref="MmoObject"/> from the <see cref="MmoZone"/>.
		/// </summary>
		/// <returns></returns>
		public bool Remove(MmoGuid mmoGuid)
		{
			while (!objectCache.RemoveItem(mmoGuid))
			{
				MmoObject existingObject;
				if (!objectCache.TryGetItem(mmoGuid, out existingObject))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the terrain height at a specified (x, z) coordinate.
		/// </summary>
		public float GetHeight(float x, float z)
		{
			return this.heightField.GetHeight(x, z);
		}

		/// <summary>
		/// Gets the default player spawn position
		/// </summary>
		/// <returns></returns>
		public Vector3 GetDefaultPlayerSpawnPosition()
		{
			// TODO: Needs to load from a config file
			switch (id)
			{
				case 1:
					return new Vector3(1442, 0, 998);

				case 2:
					return new Vector3(1437, 0, 964);

				default:
					throw new NotImplementedException();
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Disposes all local caches, threads, remove channels, and unregister social world
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// TODO: Remove chat channels and unregister social world
				this.objectCache.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Load all the zone objects and items
		/// </summary>
		private void LoadZone()
		{
			// creating the zone's chat channels
			World.ChatManager.CreateChannel(Name, ChannelType.Local, channelId => LocalChatChannel = channelId);
			World.ChatManager.CreateChannel(Name, ChannelType.Trade, channelId => TradeChatChannel = channelId);
			// registering our social world
			World.SocialManager.RegisterWorld(Id);
			
			// temp doorway
			if (id == 1)
			{
				var doorway = new Dynamic(this, 1, 100, null);
				doorway.InitializeNewDoorway(new Vector3(1, 40, 50), new GlobalPosition(2, new Vector3(1437, 0, 964)));
				this.objectCache.AddItem(doorway);
				this.PrimaryFiber.Enqueue(() => doorway.Spawn(new Vector3(1442, 14, 985.5f), Quaternion.CreateEular(0, 100, 0)));
			}

			// temp doorway
			if (id == 2)
			{
				var doorway = new Dynamic(this, 2, 100, null);
				doorway.InitializeNewDoorway(new Vector3(1, 40, 50), new GlobalPosition(1, new Vector3(1442, 0, 998)));
				this.objectCache.AddItem(doorway);
				this.PrimaryFiber.Enqueue(() => doorway.Spawn(new Vector3(1440, 14, 974.5f), Quaternion.CreateEular(0, 100, 0)));
			}

			using (var session = this.world.WorldDatabase.OpenSession())
			{
				// load all npcs
				foreach (var npcData in session.Query<NpcData>("Npc/ByZoneId").Where(n => n.ZoneId == this.id))
				{
					var npc = new Npc(this, npcData.Guid, npcData.GroupId, (NpcType) npcData.NpcType, npcData);
					this.objectCache.AddItem(npc);

					var position = npcData.Position.ToVector();
					position.Y = this.GetHeight(position.X, position.Z);
					var rotation = Quaternion.CreateEular(0, npcData.Orientation, 0);
					this.PrimaryFiber.Enqueue(() => npc.Spawn(position, rotation));
				}

				// load all game objects
				foreach (var goData in session.Query<GameObjectData>("GameObject/ByZoneId").Where(g => g.ZoneId == this.id))
				{
					var go = new Gameobject(this, goData.Guid, (GameObjectType) goData.GOType, goData.GroupId, goData);
					this.objectCache.AddItem(go);

					var position = goData.Position.ToVector();
					var rotation = Quaternion.CreateEular(0, goData.Orientation, 0);
					this.PrimaryFiber.Enqueue(() => go.Spawn(position, rotation));
				}
			}
		}

		#endregion
	}
}
