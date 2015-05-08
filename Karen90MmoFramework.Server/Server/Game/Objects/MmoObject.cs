using System;
using System.Collections;
using System.Collections.Generic;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Concurrency;
using Karen90MmoFramework.Server.ServerToClient.Events;
using Karen90MmoFramework.Server.Game.Systems;
using Karen90MmoFramework.Server.Game.Messages;

namespace Karen90MmoFramework.Server.Game.Objects
{
	/// <summary>
	/// This is the base class for every objects which clients can see.
	/// </summary>
	public abstract class MmoObject : IDisposable
	{
		#region Constants and Fields

		/// <summary>
		/// the logger
		/// </summary>
		protected static readonly ExitGames.Logging.ILogger Logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		private readonly MmoGuid guid;
		private readonly MmoZone zone;
		private readonly Transform transform;
		private readonly Hashtable properties;

		/// <summary>
		/// Add a property whenever that property changes so whenever someone requests <see cref="GetProperties()"/> the new property
		/// will be updated into the <see cref="properties"/> or the updated builtProperties only can be extracted alone.
		/// </summary>
		/// <remarks>
		/// Not every property will be generated but that will be based on each <see cref="MmoObject"/>s implementation of <see cref="BuildProperties(Hashtable,PropertyFlags)"/>.
		/// </remarks>
		protected PropertyFlags DirtyProperties;

		/// <summary>
		/// contains all players to whom this <see cref="MmoObject"/> is invisible
		/// </summary>
		private readonly HashSet<MmoGuid> hiddenPlayers;
		
		private readonly MessageChannel<MmoObjectDisposedMessage> disposeChannel;
		private readonly MessageChannel<MmoObjectEventMessage> eventChannel;
		private readonly MessageChannel<MmoObjectPositionMessage> positionUpdateChannel;

		private bool disposed;

		/// <summary>
		/// The current world region the <see cref="MmoObject"/> resides in.
		/// </summary>
		private Region currentRegion;

		/// <summary>
		/// This will be set when the <see cref="MmoObject"/> is queued for respawn.
		/// Dispose this in order to interrupt respawn.
		/// </summary>
		private IDisposable respawnSubscription;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the <see cref="MmoGuid"/>.
		/// </summary>
		public MmoGuid Guid
		{
			get
			{
				return this.guid;
			}
		}

		/// <summary>
		/// Gets the family id
		/// </summary>
		/// <remarks>
		/// A familyId is mainly used to distinguish <see cref="MmoObject"/>s which are differenct instances of the same family.
		/// For example, all wolves are of the family "Wolf".
		/// This is mainly used for quests and combat groups.
		/// </remarks>
		public short FamilyId
		{
			get
			{
				return guid.SubId;
			}
		}

		/// <summary>
		/// Gets the world
		/// </summary>
		public MmoWorld World
		{
			get
			{
				return this.zone.World;
			}
		}

		/// <summary>
		/// Gets the current zone
		/// </summary>
		public MmoZone CurrentZone
		{
			get
			{
				return this.zone;
			}
		}

		/// <summary>
		/// Gets the current region
		/// </summary>
		public IRegion CurrentRegion
		{
			get
			{
				return this.currentRegion;
			}
		}

		/// <summary>
		/// Gets the transform
		/// </summary>
		protected Transform Transform
		{
			get
			{
				return this.transform;
			}
		}

		/// <summary>
		/// Gets the position
		/// </summary>
		public Vector3 Position
		{
			get
			{
				return this.transform.Position;
			}
		}

		/// <summary>
		/// Gets the rotation
		/// </summary>
		public Quaternion Rotation
		{
			get
			{
				return this.transform.Rotation;
			}
		}

		/// <summary>
		/// Gets the builtProperties
		/// </summary>
		protected Hashtable Properties
		{
			get
			{
				return this.properties;
			}
		}

		/// <summary>
		/// Gets the bounds
		/// </summary>
		public Bounds Bounds
		{
			get
			{
				return this.transform.Bounds;
			}
		}

		/// <summary>
		/// Gets the flags
		/// </summary>
		public MmoObjectFlags Flags { get; protected set; }

		/// <summary>
		/// Gets the builtProperties revision
		/// </summary>
		/// <remarks>
		/// Revision of <value>0</value> means the <see cref="MmoObject"/> never had any builtProperties set.
		/// </remarks>
		public int Revision { get; protected set; }

		/// <summary>
		/// Gets the value of whether the <see cref="MmoObject"/> is moving or not.
		/// </summary>
		public abstract bool IsMoving { get; }

		/// <summary>
		/// Gets or sets CurrentWorldRegionSubscription.
		/// </summary>
		private IDisposable currentWorldRegionSubscription;

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> is disposed or not
		/// </summary>
		public bool Disposed
		{
			get
			{
				return this.disposed;
			}
		}

		/// <summary>
		/// Gets the dispose channel where <see cref="MmoObjectDisposedMessage"/>s are published
		/// </summary>
		public MessageChannel<MmoObjectDisposedMessage> DisposeChannel
		{
			get
			{
				return this.disposeChannel;
			}
		}

		/// <summary>
		/// Gets the event channel where <see cref="MmoObjectEventMessage"/>s are published
		/// </summary>
		public MessageChannel<MmoObjectEventMessage> EventChannel
		{
			get
			{
				return this.eventChannel;
			}
		}

		/// <summary>
		/// Gets the position update channel where <see cref="MmoObjectPositionMessage"/>s are published
		/// </summary>
		public MessageChannel<MmoObjectPositionMessage> PositionUpdateChannel
		{
			get
			{
				return this.positionUpdateChannel;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref = "MmoObject" /> class.
		/// </summary>
		/// <param name="zone"> The <see cref="MmoZone"/> the <see cref="MmoObject"/> belongs to. </param>
		/// <param name="objectId"> A unique identifier used to distinguish between other <see cref="MmoObject"/>s of this type. </param>
		/// <param name="type"> The type of the object. </param>
		/// <param name="familyId"> The family id. This will be the same for all the objects of the same kind. For example, every wolves is of the family wolf.
		/// Used in quests and combat groups. </param>
		/// <param name="bounds"> The character bounds </param>
		/// <param name="properties"> The initial builtProperties. If it is <value>NULL</value> an empty <see cref="Hashtable"/> will be created. </param>
		/// <remarks>
		/// <paramref name="objectId"/>, <paramref name="type"/>, and <paramref name="familyId"/> will be compressed into a single <see cref="Guid"/> value.
		/// This value will (have to) be globally unique accross all <see cref="MmoObject"/>s across all <see cref="MmoZone"/>s, otherwise conflicts will occur.
		/// </remarks>
		protected MmoObject(MmoZone zone, ObjectType type, int objectId, short familyId, Bounds bounds, Hashtable properties)
		{
			this.eventChannel = new MessageChannel<MmoObjectEventMessage>();
			this.disposeChannel = new MessageChannel<MmoObjectDisposedMessage>();
			this.positionUpdateChannel = new MessageChannel<MmoObjectPositionMessage>();

			this.guid = new MmoGuid((byte) type, objectId, familyId);
			this.zone = zone;
			this.transform = new Transform(bounds);

			this.properties = properties ?? new Hashtable();
			this.Revision = this.properties.Count;
			this.DirtyProperties = PropertyFlags.None;
			this.Flags = MmoObjectFlags.None;

			this.hiddenPlayers = new HashSet<MmoGuid>();
		}

		~MmoObject()
		{
			this.Dispose(false);
		}

		#endregion

		#region IDisposable Impelmentation

		/// <summary>
		/// Calls <see cref="Dispose(bool)"/> and suppresses finalization.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Publishes a <see cref="MmoObjectDisposedMessage"/> through the <see cref="DisposeChannel"/> and disposes all channels.
		/// <see cref="Disposed"/> is set to true.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !disposed)
			{
				this.SetCurrentWorldRegion(null);
				this.disposeChannel.Publish(new MmoObjectDisposedMessage(this));
				this.eventChannel.Dispose();
				this.disposeChannel.Dispose();
				this.positionUpdateChannel.Dispose();

				this.disposed = true;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Creates an update event and starts updating it.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="intervalInMs"> Interval in which the event should repeat. </param>
		/// <returns> Returns a(n) <see cref="IDisposable"/> object which is used to abort the event. </returns>
		public IDisposable CreateUpdateEvent(Action<int> action, long intervalInMs)
		{
			return new UpdateEvent(this.CurrentZone.PrimaryFiber, this.CurrentZone.World, action, intervalInMs).Start();
		}

		/// <summary>
		/// Rewards this <see cref="MmoObject"/> of the kill.
		/// </summary>
		/// <param name="killed"> The <see cref="MmoObject"/> that was killed. </param>
		public virtual void RewardKill(MmoObject killed)
		{
		}

		/// <summary>
		/// Sends a movement update to a(n) <see cref="Player"/>.
		/// </summary>
		public abstract void SendMovementUpdateTo(Player player);

		#endregion

		#region Object Identification

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is of type <see cref="ObjectType.Npc"/>.
		/// </summary>
		public bool IsNpc()
		{
			return this.guid.Type == (byte) ObjectType.Npc;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is of type <see cref="ObjectType.Gameobject"/>.
		/// </summary>
		public bool IsGameobject()
		{
			return this.guid.Type == (byte) ObjectType.Gameobject;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is of type <see cref="ObjectType.Player"/>.
		/// </summary>
		public bool IsPlayer()
		{
			return this.guid.Type == (byte) ObjectType.Player;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a merchant.
		/// </summary>
		public virtual bool IsMerchant()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a civilian.
		/// </summary>
		public virtual bool IsCivilian()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a guard.
		/// </summary>
		public virtual bool IsGuard()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a trainer.
		/// </summary>
		public virtual bool IsTrainer()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a plant.
		/// </summary>
		public virtual bool IsPlant()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a vein.
		/// </summary>
		public virtual bool IsVein()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is an item.
		/// </summary>
		public virtual bool IsItem()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a chest.
		/// </summary>
		public virtual bool IsChest()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a spell object.
		/// </summary>
		public virtual bool IsSpellObject()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a doorway.
		/// </summary>
		public virtual bool IsDoorway()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="MmoObject"/> is a portal.
		/// </summary>
		public virtual bool IsPortal()
		{
			return false;
		}

		#endregion

		#region Update(int deltaTime)

		/// <summary>
		/// Updates the <see cref="MmoObject"/>.
		/// </summary>
		/// <param name="deltaTime"></param>
		public virtual void Update(int deltaTime)
		{
		}

		#endregion

		#region Interest Management System Implementation

		/// <summary>
		/// Does nothing but calls <see cref="OnDestroy"/>. Make sure to call this before disposing.
		/// </summary>
		public void Destroy()
		{
			this.OnDestroy();
		}

		/// <summary>
		/// Called when the <see cref="MmoObject"/> is destroyed
		/// </summary>
		protected virtual void OnDestroy()
		{
		}

		/// <summary>
		/// Publishes a <see cref="MmoObjectPositionMessage"/> in the <see cref="PositionUpdateChannel"/> 
		/// and in the current <see cref="CurrentRegion"/> if it changes
		/// and then updates the <see cref="CurrentRegion"/>.
		/// </summary>
		public void UpdateInterestManagement()
		{
			// gets the region at our current position
			var newRegion = this.CurrentZone.GetRegion(Position);

			// inform subscribers of the position change
			var message = this.GetPositionUpdateMessage(this.Position, newRegion);
			this.positionUpdateChannel.Publish(message);

			// if the new region is different than the old one
			// we will publish our snapshot to the new region
			if (SetCurrentWorldRegion(newRegion))
			{
				var snapshot = this.GetMmoObjectSnapshot();
				// any subscribed interest areas will be notified of the snapshot
				newRegion.Publish(snapshot);
			}
		}

		/// <summary>
		/// Creates an <see cref="MmoObjectSnapshot"/> with a snapshot of the current attributes.
		/// Override this method to return a subclass of <see cref="MmoObjectSnapshot"/> that includes more data.
		/// The return value is published through the <see cref="CurrentRegion"/> or sent directly to an <see cref="InterestArea"/>.
		/// </summary>
		public MmoObjectSnapshot GetMmoObjectSnapshot()
		{
			return new MmoObjectSnapshot(this, this.Position, this.Rotation.EularAngles(), this.Revision, this.currentRegion);
		}

		/// <summary>
		/// Creates an <see cref="MmoObjectPositionMessage"/> with the current position and region.
		/// The return value is published through the <see cref="PositionUpdateChannel"/>.
		/// </summary>
		protected MmoObjectPositionMessage GetPositionUpdateMessage(Vector3 position, Region region)
		{
			return new MmoObjectPositionMessage(this, position, region);
		}

		/// <summary>
		/// Sets the current world region
		/// </summary>
		protected bool SetCurrentWorldRegion(Region newRegion)
		{
			// out of bounds
			if (newRegion == null)
			{
				// was not out of bounce before
				if (CurrentRegion != null)
				{
					this.currentRegion = null;
					this.currentWorldRegionSubscription.Dispose();
					this.currentWorldRegionSubscription = null;
				}

				return false;
			}

			// was out of bounce before
			if (CurrentRegion == null)
			{
				// subscribing to the new region to receive updates
				this.currentWorldRegionSubscription = newRegion.Subscribe(this.CurrentZone.PrimaryFiber, this.Region_OnReceive);
				this.currentRegion = newRegion;
				return true;
			}

			// current region changed
			if (newRegion != this.CurrentRegion)
			{
				// subscribe to the new region to get updates
				var newSubscription = newRegion.Subscribe(this.CurrentZone.PrimaryFiber, this.Region_OnReceive);

				// dispose the old region subscription
				this.currentWorldRegionSubscription.Dispose();
				this.currentWorldRegionSubscription = newSubscription;
				this.currentRegion = newRegion;

				return true;
			}

			return false;
		}

		/// <summary>
		/// Called when a <see cref="RegionMessage"/> has been received.
		/// </summary>
		private void Region_OnReceive(RegionMessage message)
		{
			if (disposed)
				return;

			message.OnMmoObjectReceive(this);
		}

		#endregion

		#region Property System Implementation

		/// <summary>
		/// Gets the current up-to-date builtProperties. This never generates a new <see cref="Hashtable"/>.
		/// </summary>
		public Hashtable GetProperties()
		{
			if (DirtyProperties != PropertyFlags.None)
			{
				// updates the current builtProperties with any changed builtProperties
				this.BuildProperties(properties, this.DirtyProperties);
				this.DirtyProperties = PropertyFlags.None;
			}

			return this.properties;
		}

		/// <summary>
		/// Adds or sets builtProperties into the <paramref name="builtProperties"/> based on <paramref name="flags"/>.
		/// </summary>
		/// <remarks>
		/// This does not generate a new <see cref="Hashtable"/> because this will be called extensively based on how much a(n) <see cref="MmoObject"/>s
		/// property changes. Generating a new <see cref="Hashtable"/> everytime would be expensive.
		/// </remarks>
		public virtual void BuildProperties(Hashtable builtProperties, PropertyFlags flags)
		{
		}

		/// <summary>
		/// Called when a property needs to be published to client(s). Publishes a <see cref="MmoObjectEventMessage"/> on the <see cref="EventChannel"/>
		/// by default with the changed property.
		/// </summary>
		protected virtual void PublishProperty(PropertyCode propertyCode, object value)
		{
			var setProperty = new ObjectProperty
				{
					ObjectId = this.Guid,
					PropertiesCode = (byte) propertyCode,
					EventData = value
				};

			var msg = new MmoObjectEventMessage(this, setProperty, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			this.EventChannel.Publish(msg);
		}

		#endregion

		#region Respawn System Implementation

		/// <summary>
		/// Spawns the item at the given <paramref name="position"/> with the <paramref name="rotation"/> and updates interest management.
		/// </summary>
		public void Spawn(Vector3 position, Quaternion rotation)
		{
			this.OnAwake();

			this.transform.Position = position;
			this.transform.Rotation = rotation;
			// updating interest management to notify interest areas on our region
			this.UpdateInterestManagement();

			this.OnSpawn();
		}

		/// <summary>
		/// Queues the current <see cref="MmoObject"/> for respawn.
		/// </summary>
		protected void QueueRespawn(int delayBeforeRespawnMs, int respawnTimerMs, Vector3 spawnPosition, Quaternion spawnRotation)
		{
			// previously queued? then dispose
			if (respawnSubscription != null)
				this.respawnSubscription.Dispose();

			// setting the timer to remove corpse
			this.respawnSubscription = this.CurrentZone.PrimaryFiber.Schedule(() =>
				{
					// publishing a disposed message to notify interest areas
					this.CurrentZone.Remove(this.Guid);
					this.SetCurrentWorldRegion(null);
					this.DisposeChannel.Publish(new MmoObjectDisposedMessage(this));

					this.OnSleep();

					// setting the actual respawn timer
					this.respawnSubscription = this.CurrentZone.PrimaryFiber.Schedule(() =>
						{
							this.CurrentZone.Add(this);
							this.Spawn(spawnPosition, spawnRotation);
							this.respawnSubscription = null;
						},
					                                                                  respawnTimerMs);
				},
			                                                                  delayBeforeRespawnMs);
		}

		/// <summary>
		/// Called right before spawning
		/// </summary>
		protected virtual void OnAwake()
		{
		}

		/// <summary>
		/// Called when the object is spawned
		/// </summary>
		protected virtual void OnSpawn()
		{
		}

		/// <summary>
		/// Called when the instance has been removed from the world
		/// </summary>
		protected virtual void OnSleep()
		{
		}

		#endregion

		#region Interaction System Implementation

		/// <summary>
		/// Tells whether this <see cref="MmoObject"/> is hidden for a certain player or not
		/// </summary>
		/// <param name="playerGuid"></param>
		/// <returns></returns>
		public virtual bool IsHiddenFor(MmoGuid playerGuid)
		{
			// we cant hide ourselves
			if (playerGuid == this.guid)
				return false;

			lock (hiddenPlayers)
			{
				return this.hiddenPlayers.Contains(playerGuid);
			}
		}

		/// <summary>
		/// Hides this <see cref="MmoObject"/> from a player's view
		/// </summary>
		/// <param name="playerGuid"></param>
		protected void HideFromPlayer(MmoGuid playerGuid)
		{
			// we cant hide ourselves
			if (playerGuid == this.guid)
				return;

			lock (hiddenPlayers)
			{
				// if the player is not added then he/she was already in the list
				// so skip the next step to save the overhead of lookup
				if (hiddenPlayers.Add(playerGuid) == false)
					return;
			}

			WorldSession session;
			if (CurrentZone.World.SessionCache.TryGetSessionByPlayerGuid(playerGuid.Id, out session))
			{
				// telling that player to hide us
				session.Player.HideObjectFromView(this);
			}
		}

		/// <summary>
		/// Unhides this <see cref="MmoObject"/> from a player's view
		/// </summary>
		/// <param name="playerGuid"></param>
		protected void UnhideFromPlayer(MmoGuid playerGuid)
		{
			// we cant hide ourselves
			if (playerGuid == this.guid)
				return;

			lock (hiddenPlayers)
			{
				// if the player is not removed then he/she was not in the list in the first place
				// so skip the next step to save the overhead of lookup
				if (hiddenPlayers.Remove(playerGuid) == false)
					return;
			}

			WorldSession player;
			if (CurrentZone.World.SessionCache.TryGetSessionByPlayerGuid(playerGuid.Id, out player))
			{
				// telling that player to unhide us
				player.Player.UnhideObjectFromView(this);
			}
		}

		#endregion

		#region Loot System Implementation

		/// <summary>
		/// Tells whether this <see cref="MmoObject"/> has loot for a(n) <see cref="Player"/> or not.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public virtual bool HaveLootFor(Player player)
		{
			return false;
		}

		/// <summary>
		/// Gets the <see cref="ILoot"/> for a(n) <see cref="Player"/>.
		/// </summary>
		public virtual ILoot GetLootFor(Player player)
		{
			return EmptyLootContainer.EmptyLoot;
		}

		#endregion

		#region Quest System Implementation

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has any quests at all.
		/// </summary>
		public virtual bool HaveQuests()
		{
			return false;
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has any startable quests.
		/// </summary>
		public virtual bool HaveStartableQuests()
		{
			return false;
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has any completable quests.
		/// </summary>
		public virtual bool HaveCompletableQuests()
		{
			return false;
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has a particular startable quest.
		/// </summary>
		public virtual bool HaveStartableQuest(short questId)
		{
			return false;
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has a particular completable quest.
		/// </summary>
		public virtual bool HaveCompletableQuest(short questId)
		{
			return false;
		}

		/// <summary>
		/// Gets all startable quest ids
		/// </summary>
		/// <returns></returns>
		public virtual short[] GetStartableQuests()
		{
			return null;
		}

		/// <summary>
		/// Gets all completable quest ids
		/// </summary>
		/// <returns></returns>
		public virtual short[] GetCompletableQuests()
		{
			return null;
		}

		#endregion
	}
}