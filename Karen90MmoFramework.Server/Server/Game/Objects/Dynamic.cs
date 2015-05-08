using System;
using System.Collections;
using System.Threading;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Quantum.Physics;
using Karen90MmoFramework.Server.Game.Messages;
using Karen90MmoFramework.Server.ServerToClient.Events;

namespace Karen90MmoFramework.Server.Game.Objects
{
	public class Dynamic : MmoObject, IPoolObject<short>
	{
		#region Constants and Fields

		/// <summary>
		/// dynamic type
		/// </summary>
		private DynamicType dynamicType;

		/// <summary>
		/// the teleportation location
		/// </summary>
		private GlobalPosition teleportToLocation;

		/// <summary>
		/// the doorway trigger
		/// </summary>
		private ITriggerVolume doorwayTrigger;

		/// <summary>
		/// The subscription which is responsible for calling <see cref="Update(int)"/> at regular intervals.
		/// </summary>
		private IDisposable updateSubscription;

		/// <summary>
		/// the region subscription
		/// </summary>
		private IDisposable regionSubscription;

		/// <summary>
		/// The subscription which track whether the owner who summoned the <see cref="Dynamic"/> has been disposed or not.
		/// This subscription will be use to abandon the current target and destroy (return to pool) the <see cref="Dynamic"/>.
		/// </summary>
		private IDisposable ownerDisposedSubscription;

		private float lastOrientation;
		private float lastPitch;
		private int lastSpeed;

		private Vector3 destination;

		private int nextMovementPublishTime;
		private int nextRotationCheckTime;

		/// <summary>
		/// This will be set by the <see cref="Interlocked"/> class so do not use it in any other places or set it directly
		/// </summary>
		private int interlockedIsMoving;

		/// <summary>
		/// callback to call when the element as hit the target
		/// </summary>
		private Action onHit;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the dynamic type
		/// </summary>
		public DynamicType DynamicType
		{
			get
			{
				return this.dynamicType;
			}
		}

		/// <summary>
		/// Gets the target we are after
		/// </summary>
		protected MmoObject CurrentFocus { get; set; }

		/// <summary>
		/// Gets the current speed
		/// </summary>
		protected int CurrentSpeed { get; set; }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="Dynamic"/> class.
		/// </summary>
		public Dynamic(MmoZone zone, int objectId, short familyId, Hashtable properties)
			: base(zone, ObjectType.Dynamic, objectId, familyId, new Bounds(), properties)
		{
			this.Flags = MmoObjectFlags.SendPropertiesOnSubscribe;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Creates a new <see cref="Dynamic"/> or gets one from the pool
		/// </summary>
		public static Dynamic CreateNew(MmoZone world, short familyId)
		{
			return world.DynamicObjectPool.Take(familyId,
												() =>
												new Dynamic(world, Utils.NewGuidInt32(GuidCreationCulture.Utc), familyId, null));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Called when the object is spawned
		/// </summary>
		protected override void OnSpawn()
		{
			if(dynamicType == DynamicType.Doorway)
			{
				this.doorwayTrigger.Position = this.Position;
				this.doorwayTrigger.Orientation = this.Rotation;
#if ! USE_PHYSICS
				// if we do not use the physics engine we need to update the trigger manually
				if (CurrentRegion.NumListeners > 0)
				{
					// if there are players within the region we can start updating
					this.updateSubscription = this.CreateUpdateEvent(this.Update, ServerGameSettings.GAME_UPDATE_INTERVAL);
				}
				else
				{
					// if there are no players within the region we dont need to update until there are players
					this.regionSubscription = this.CurrentRegion.Subscribe(CurrentZone.SyncFiber, this.OnRegion_Receive);
				}
#endif
#if MMO_DEBUG
				Logger.DebugFormat("Id: {0}. Type: Dynamic ({1}). Position: {2}", this.Guid, this.DynamicType, this.Position);
#endif
			}
		}

		/// <summary>
		/// Updates the <see cref="MmoObject"/>.
		/// </summary>
		/// <param name="deltaTime"></param>
		public override sealed void Update(int deltaTime)
		{
			switch (dynamicType)
			{
				case DynamicType.SpellObject:
					// updating rotation early to avoid client prediction errors
					this.UpdateSpellRotation();

					// TODO: Need to use the target height / 2
					this.destination = this.CurrentFocus.Position + Vector3.Up * 2;
					// if we are within 3m range from the target consider it a hit
					if (Vector3.SqrDistance(Position, destination) <= 9)
					{
						// its a hit
						this.onHit();
						this.ResetSpellMovement();
						this.Destroy();
					}
					else
					{
						// follow the target
						this.DoSpellMove(this.destination, deltaTime);
					}
					break;

				case DynamicType.Doorway:
					this.doorwayTrigger.Update();
					// checking to see whether we have an empty region (no players) or not
					if(CurrentRegion.NumListeners == 0)
					{
						// if there are no players in the region stop updating and setup region subscription
						// which will reset the update when a player enters the region
						this.DisposeGameSubscription();
						this.regionSubscription = this.CurrentRegion.Subscribe(this.CurrentZone.PrimaryFiber, this.OnRegion_Receive);
					}
					break;
			}
		}

		/// <summary>
		/// Called when a property needs to be published to client(s). Publishes a <see cref="MmoObjectEventMessage"/> on the <see cref="MmoObject.EventChannel"/>
		/// by default with the changed property.
		/// </summary>
		protected override sealed void PublishProperty(PropertyCode propertyCode, object value)
		{
			// usually we wont have any builtProperties to publish but for future sake let the base class handle it
			base.PublishProperty(propertyCode, value);
		}

		/// <summary>
		/// Called when the <see cref="MmoObject"/> is destroyed
		/// </summary>
		protected override sealed void OnDestroy()
		{
			this.SetCurrentWorldRegion(null);
			// letting our subscribers know that we are destroyed (not really)
			this.DisposeChannel.Publish(new MmoObjectDisposedMessage(this));

			// clearing all subscribers
			this.EventChannel.ClearSubscribers();
			this.DisposeChannel.ClearSubscribers();
			this.PositionUpdateChannel.ClearSubscribers();

			if (dynamicType == DynamicType.Doorway)
			{
				this.CurrentZone.Physics.RemoveTriggerVolume(doorwayTrigger);
#if !USE_PHYSICS
				this.DisposeGameSubscription();
				this.DisposeRegionSubscription();
#endif
			}

			// returning the element back to the pool for recycling
			this.CurrentZone.DynamicObjectPool.GiveBack(this);
		}

		/// <summary>
		/// Cleans up all the values
		/// </summary>
		private void Refresh()
		{
			this.Transform.Position = Vector3.Zero;
			this.Transform.Rotation = Quaternion.Identity;

			this.destination = Vector3.Zero;
			this.lastOrientation = 0;
			this.lastPitch = 0;
			this.CurrentSpeed = this.lastSpeed = 0;
		}

		private void DisposeGameSubscription()
		{
			if (this.updateSubscription == null)
				return;

			this.updateSubscription.Dispose();
			this.updateSubscription = null;
		}

		private void DisposeRegionSubscription()
		{
			if (this.regionSubscription == null)
				return;

			this.regionSubscription.Dispose();
			this.regionSubscription = null;
		}

		#endregion

		#region Object Identification

		/// <summary>
		/// Returns false.
		/// </summary>
		public override sealed bool IsMerchant()
		{
			return false;
		}

		/// <summary>
		/// Returns false.
		/// </summary>
		public override sealed bool IsCivilian()
		{
			return false;
		}

		/// <summary>
		/// Returns false.
		/// </summary>
		public override sealed bool IsGuard()
		{
			return false;
		}

		/// <summary>
		/// Returns false.
		/// </summary>
		public override sealed bool IsTrainer()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
		public override sealed bool IsPlant()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
		public override sealed bool IsVein()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
		public override sealed bool IsItem()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
		public override sealed bool IsChest()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="Dynamic"/> is of type <see cref="Karen90MmoFramework.Game.DynamicType.SpellObject"/>.
		/// </summary>
		public override sealed bool IsSpellObject()
		{
			return this.dynamicType == DynamicType.SpellObject;
		}

		/// <summary>
		/// Returns if the <see cref="Dynamic"/> is of type <see cref="Karen90MmoFramework.Game.DynamicType.Doorway"/>.
		/// </summary>
		public override sealed bool IsDoorway()
		{
			return this.dynamicType == DynamicType.Doorway;
		}

		/// <summary>
		/// Returns if the <see cref="Dynamic"/> is of type <see cref="Karen90MmoFramework.Game.DynamicType.Portal"/>.
		/// </summary>
		public override sealed bool IsPortal()
		{
			return this.dynamicType == DynamicType.Portal;
		}

		#endregion

		#region Doorway System Implementation

		/// <summary>
		/// Initializes a new doorway
		/// </summary>
		/// <remarks>
		/// This must be called prior to using the <see cref="Dynamic"/> object
		/// </remarks>
		public void InitializeNewDoorway(Vector3 size, GlobalPosition teleportTo)
		{
			this.dynamicType = DynamicType.Doorway;
			this.teleportToLocation = teleportTo;
			// we will publish builtProperties right after subscribing
			// since dynamic types are short lived and does not usually contain too many (or any) builtProperties
			// we can safely send builtProperties right after subscription
			// this will also prevent the client from caching our builtProperties which would be useless
			this.Flags = MmoObjectFlags.SendPropertiesOnSubscribe;

			// cleanup before adding properties
			this.Refresh();
			// making sure to remove the object type if it was provided already
			this.Properties.Clear();
			this.Properties.Add((byte) PropertyCode.DynamicType, (byte) dynamicType);

			this.doorwayTrigger = CurrentZone.Physics.CreateTriggerVolume(new TriggerVolumeDescription {NameOfTarget = "character", Size = size},
			                                                        guid => OnEnterDoorway((MmoGuid) guid));
			// this will be checked at subscription and prevents sending revision value
			this.Flags |= MmoObjectFlags.HasProperties;
		}

		/// <summary>
		/// Called when an object enters the doorway
		/// </summary>
		/// <param name="objectGuid"></param>
		void OnEnterDoorway(MmoGuid objectGuid)
		{
			if (objectGuid.Type != (byte) ObjectType.Player)
				return;

			MmoObject playerObject;
			if(CurrentZone.ObjectCache.TryGetItem(objectGuid, out playerObject))
			{
				var player = playerObject as Player;
				if(player != null)
					player.TeleportTo(teleportToLocation);
			}
		}

		void OnRegion_Receive(RegionMessage regionMessage)
		{
			// snapShotRequest means a player is entering the region
			var snapshotRequest = regionMessage as MmoObjectSnapshotRequest;
			if (snapshotRequest == null)
				return;
			
			// double checking to make sure that there is actually one player within the region
			if (CurrentRegion.NumListeners == 0)
				return;

			// there is a player within the region so dispose region check and start updating
			this.DisposeRegionSubscription();
			this.updateSubscription = this.CreateUpdateEvent(this.Update, ServerGameSettings.GAME_UPDATE_INTERVAL);
		}

		#endregion

		#region Spell System Implementation

		/// <summary>
		/// Gets the value of whether the <see cref="Dynamic"/> is moving or not.
		/// </summary>
		public override sealed bool IsMoving
		{
			get
			{
				return this.interlockedIsMoving > 0;
			}
		}

		/// <summary>
		/// Initializes a new spell object
		/// </summary>
		/// <remarks>
		/// This must be called prior to using the <see cref="Dynamic"/> object
		/// </remarks>
		public void InitializeNewSpellObject()
		{
			this.dynamicType = DynamicType.SpellObject;
			// we will publish builtProperties right after subscribing
			// since element types are short lived and does not usually contain too many (or any) builtProperties
			// we can safely send builtProperties right after subscription
			// this will also prevent the client from caching our builtProperties which would be useless
			this.Flags = MmoObjectFlags.SendPropertiesOnSubscribe;

			// cleanup before adding properties
			this.Refresh();
			// making sure to remove the object type if it was provided already
			this.Properties.Clear();
			this.Properties.Add((byte)PropertyCode.DynamicType, (byte)dynamicType);

			// this will be checked at subscription and prevents sending revision value
			this.Flags |= MmoObjectFlags.HasProperties;
		}

		/// <summary>
		/// Sends a movement update to a <see cref="Player"/>.
		/// </summary>
		public override sealed void SendMovementUpdateTo(Player player)
		{
			var eularAngles = this.Rotation.EularAngles();
			var movementEvent = new ObjectMovement
				{
					ObjectId = this.Guid,
					Speed = (byte) this.CurrentSpeed,
					Orientation = eularAngles.Y,
					Pitch = eularAngles.X
				};

			player.Session.SendEvent(movementEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
		}

		/// <summary>
		/// Begins to chase a(n) <see cref="MmoObject"/> and calls <paramref name="onHitCallback"/> when hit.
		/// </summary>
		public void SpellChase(MmoObject target, MmoObject owner, int speed, Action onHitCallback)
		{
			// we are already being updated so skip
			if (updateSubscription != null)
				return;

			this.CurrentFocus = target;
			this.CurrentSpeed = speed;
			this.onHit = onHitCallback;

			// creating the owner disposal subscription
			this.ownerDisposedSubscription = owner.DisposeChannel.Subscribe(this.CurrentZone.PrimaryFiber, m => this.OnSpellOwner_Disposed());

			this.destination = this.CurrentFocus.Position;
			this.updateSubscription = this.CreateUpdateEvent(this.Update, ServerGameSettings.GAME_UPDATE_INTERVAL);

			// we are moving
			Interlocked.Exchange(ref this.interlockedIsMoving, 1);
		}

		/// <summary>
		/// Stops chasing
		/// </summary>
		private void ResetSpellMovement()
		{
			// we are not moving
			Interlocked.Exchange(ref this.interlockedIsMoving, 0);

			this.DisposeSpellSubscriptions();
			this.DisposeGameSubscription();

			this.CurrentSpeed = this.lastSpeed = 0;
			this.CurrentFocus = null;
			this.onHit = null;

			this.destination = Vector3.Zero;
		}

		/// <summary>
		/// Moves the <see cref="Dynamic"/>'s position towards the destination
		/// </summary>
		protected void DoSpellMove(Vector3 position, int deltaTime)
		{
			// the element can move only in the forward (+z) direction (facing the target most of the time)
			var direction = Vector3.Transform(Vector3.Forward, this.Rotation).Normalize();
			// note: deltaTime is in milliseconds
			var newPos = this.Position + direction * this.CurrentSpeed * (deltaTime / 1000f);
			// clamping to avoid walking off the terrain
			this.Transform.Position = this.CurrentZone.Bounds.Clamp(newPos);
			
			// send update if the speed changed
			if (lastSpeed != this.CurrentSpeed)
			{
				var movementEvent = new ObjectMovement
					{
						ObjectId = this.Guid,
						Speed = (byte) this.CurrentSpeed,
					};

				this.lastSpeed = this.CurrentSpeed;
				
				var eularAngles = this.Rotation.EularAngles();
				var yRotation = eularAngles.Y;
				// check the rotation for changes so we can send it with this update rather than let the UpdateRotation() handle it
				// this way we can send it as one update rather than two
				if (Math.Abs(lastOrientation - yRotation) > 0.1f)
				{
					movementEvent.Orientation = yRotation;
					this.lastOrientation = yRotation;
				}

				var xRotation = eularAngles.X;
				// check the rotation for changes so we can send it with this update rather than let the UpdateRotation() handle it
				// this way we can send it as one update rather than two
				if (Math.Abs(lastPitch - xRotation) > 0.1f)
				{
					movementEvent.Pitch = xRotation;
					this.lastPitch = xRotation;
				}

				var message = new MmoObjectEventMessage(this, movementEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
				this.EventChannel.Publish(message);
			}

			// publishing our real position in case the client needs a position correction
			if (nextMovementPublishTime <= CurrentZone.World.GlobalTime)
			{
				this.UpdateInterestManagement();
				var positionEvent = new ObjectTransform
					{
						ObjectId = this.Guid,
						Position = this.Position.ToFloatArray(3)
					};

				var message = new MmoObjectEventMessage(this, positionEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });

				// TODO: Consider sending this unreliable and reducing the update interval
				this.EventChannel.Publish(message);
				this.nextMovementPublishTime = CurrentZone.World.GlobalTime + ServerGameSettings.OBJECT_MOVEMENT_PUBLISH_INTERVAL;
			}
		}

		private void UpdateSpellRotation()
		{
			// rotation will be directly facing the destination
			this.Transform.Rotation = Quaternion.FromToRotation(Vector3.Forward, (destination - Position));
			if (nextRotationCheckTime <= CurrentZone.World.GlobalTime)
			{
				var eularAngles = this.Rotation.EularAngles();
				var xRotation = eularAngles.X;
				var yRotation = eularAngles.Y;
				var orientationChanged = Math.Abs(lastOrientation - yRotation) > 0.1f;
				var pitchChanged = Math.Abs(lastPitch - xRotation) > 0.1f;

				// send update if the rotation changed
				if (orientationChanged || pitchChanged)
				{
					var rotationEvent = new ObjectTransform
						{
							ObjectId = this.Guid,
						};

					if (orientationChanged)
					{
						rotationEvent.Orientation = yRotation;
						this.lastOrientation = yRotation;
					}

					if (pitchChanged)
					{
						rotationEvent.Pitch = xRotation;
						this.lastPitch = xRotation;
					}

					var message = new MmoObjectEventMessage(this, rotationEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
					this.EventChannel.Publish(message);
				}

				this.nextRotationCheckTime = CurrentZone.World.GlobalTime + ServerGameSettings.OBJECT_ROTATION_CHECK_INTERVAL;
			}
		}

		private void DisposeSpellSubscriptions()
		{
			if (ownerDisposedSubscription != null)
			{
				this.ownerDisposedSubscription.Dispose();
				this.ownerDisposedSubscription = null;
			}
		}

		private void OnSpellOwner_Disposed()
		{
			// if the owner is disposed we can destroy ourself
			this.ResetSpellMovement();
			this.Destroy();
		}

		#endregion

		#region Implementation of IPoolObject<out short>

		/// <summary>
		/// Gets the key
		/// </summary>
		/// <returns></returns>
		short IPoolObject<short>.GetKey()
		{
			return this.FamilyId;
		}

		#endregion
	}
}
