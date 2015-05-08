using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Quantum.Physics;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Items;
using Karen90MmoFramework.Server.Game.Systems;
using Karen90MmoFramework.Server.Game.Messages;
using Karen90MmoFramework.Server.ServerToClient.Events;
using Karen90MmoFramework.Server.ServerToClient.Operations;

namespace Karen90MmoFramework.Server.Game.Objects
{
	public class Player : Character, IGroupMember
	{
		#region Constants and Fields

		private readonly WorldSession session;
		private readonly Inventory inventory;
		private readonly SpellManager spellManager;
		private readonly Actionbar actionbar;

		private readonly Origin origin;
		private int xp;
		private int money;

		private readonly ICharacterController characterController;
		private readonly HashSet<MmoGuid> subscribedGuids;

		private MmoObject currentInteractor;
		private IDisposable interactorDistanceCheckSubscription;

		private readonly Dictionary<short, QuestProgression> currentQuests;
		private readonly HashSet<short> finishedQuests;

		private readonly List<ContainerItemStructure> itemCluster;
		private bool clusterAddition;

		#region Movement System
		private MovementState lastMovementState;
		private MovementDirection lastDirectionFlags;
		private float lastOrientation;
		private int lastSpeed;

		private MovementState currentMovementState;
		private MovementDirection currentDirectionFlags;
		private int currentSpeed;

		private IDisposable movementUpdateSubscription;
		private int lastMovementUpdateTime;
		private int nextMovementPublishTime;
		private int nextInterestManagementMoveTime;
		#endregion

		#endregion

		#region Properties

		/// <summary>
		/// Gets the session
		/// </summary>
		public WorldSession Session
		{
			get
			{
				return session;
			}
		}

		/// <summary>
		/// Gets the inventory
		/// </summary>
		public Inventory Inventory
		{
			get
			{
				return this.inventory;
			}
		}

		/// <summary>
		/// Gets the actionbar
		/// </summary>
		public Actionbar Actionbar
		{
			get
			{
				return this.actionbar;
			}
		}

		/// <summary>
		/// Gets the spell manager
		/// </summary>
		public SpellManager SpellManager
		{
			get
			{
				return this.spellManager;
			}
		}

		/// <summary>
		/// Gets the player's origin
		/// </summary>
		public Origin Origin
		{
			get
			{
				return this.origin;
			}
		}

		/// <summary>
		/// Gets the amount of money
		/// </summary>
		public int Money
		{
			get
			{
				return this.money;
			}
		}

		/// <summary>
		/// Gets the current experience
		/// </summary>
		public int Xp
		{
			get
			{
				return this.xp;
			}
		}

		/// <summary>
		/// Gets the group
		/// </summary>
		public Group Group { get; set; }

		/// <summary>
		/// Gets or Sets member update flags
		/// </summary>
		public GroupMemberPropertyFlags MemberUpdateFlags { get; set; }

		/// <summary>
		/// Gets the value of whether the <see cref="Character"/> is moving or not.
		/// </summary>
		public override sealed bool IsMoving
		{
			get
			{
				return this.currentSpeed > 0;
			}
		}

		/// <summary>
		/// Gets the current movement state
		/// </summary>
		public MovementState CurrentMovementState
		{
			get
			{
				return this.currentMovementState;
			}
		}

		/// <summary>
		/// Gets the current interactor
		/// </summary>
		public MmoObject CurrentInteractor
		{
			get
			{
				return this.currentInteractor;
			}
		}

		#endregion
		
		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="Player"/> class.
		/// </summary>
		public Player(WorldSession session, MmoZone zone, PlayerData playerData)
			: base(zone, ObjectType.Player, session.Guid, 0, new Bounds(new Vector3(0, GlobalGameSettings.PLAYER_CONTROLLER_HEIGHT * 0.5f, 0), new Vector3(GlobalGameSettings.PLAYER_CONTROLLER_RADIUS, GlobalGameSettings.PLAYER_CONTROLLER_HEIGHT, GlobalGameSettings.PLAYER_CONTROLLER_RADIUS)), playerData)
		{
			this.session = session;
			this.origin = (Origin) playerData.Origin;
			this.xp = playerData.Xp;
			this.money = playerData.Money;

			this.subscribedGuids = new HashSet<MmoGuid>();
			this.itemCluster = new List<ContainerItemStructure>();

			// loading stats
			for (var i = Stats.eFirstStat; i <= Stats.eLastStat; i++)
				this.SetBaseStat(i, playerData.Stats[i - Stats.eFirstStat]);
			// loading the inventory
			this.inventory = MmoSerializer.DeserializePlayerInventory(playerData.Inventory);
			// loading spell manager
			this.spellManager = new SpellManager(playerData.Spells, this);
			// loading the action bar
			this.actionbar = MmoSerializer.DeserializeActionbar(playerData.ActionBar);
			// loading quests
			this.currentQuests = MmoSerializer.DeserializePlayerQuests(playerData.CurrentQuests);
			this.finishedQuests = new HashSet<short>(playerData.FinishedQuests);
			// loading properties
			this.BuildProperties(Properties, PropertyFlags.ActorAll);
			this.Revision = this.Properties.Count;
			if (Properties.Count > 0)
				this.Flags |= MmoObjectFlags.HasProperties;

			this.inventory.OnItemAdded += Inventory_OnItemAdded;
			this.inventory.OnItemRemoved += Inventory_OnItemRemoved;

			this.spellManager.OnSpellAdded += SpellManager_OnSpellAdded;
			this.spellManager.OnSpellRemoved += SpellManager_OnSpellRemoved;

			this.SetHealth(playerData.CurrHealth);
			this.SetPower(playerData.CurrMana);

			// creating the character controller from the physics world
			// do not create the character controller from its class directly instead use the current physics world
			this.characterController = CurrentZone.Physics.CreateCharacterController(new CharacterControllerDescription
				{
					Name = "character",
					UserData = Guid,
					Position = playerData.Position.ToVector(),
					Radius = GlobalGameSettings.PLAYER_CONTROLLER_RADIUS,
					Height = GlobalGameSettings.PLAYER_CONTROLLER_HEIGHT,
					SlopeLimit = GlobalGameSettings.PLAYER_CONTROLLER_SLOPE_LIMIT,
					SkinWidth = GlobalGameSettings.PLAYER_CONTROLLER_SKIN_WIDTH,
					StepOffset = GlobalGameSettings.PLAYER_CONTROLLER_STEP_OFFSET
				});
		}

		#endregion

		#region General Methods

		/// <summary>
		/// Called when a <see cref="MmoObject"/> is subscribed on the client
		/// </summary>
		internal void OnSubscribe(MmoObject mmoObject)
		{
			lock (subscribedGuids)
				this.subscribedGuids.Add(mmoObject.Guid);

			if (mmoObject.IsMoving)
				mmoObject.SendMovementUpdateTo(this);
		}

		/// <summary>
		/// Called when a <see cref="MmoObject"/> is unsubscribed from the client
		/// </summary>
		internal void OnUnsubscribe(MmoObject gameObject)
		{
			lock (subscribedGuids)
				this.subscribedGuids.Remove(gameObject.Guid);
		}

		/// <summary>
		/// Determines whether the client is subscribed with a <see cref="MmoObject"/> or not
		/// </summary>
		public bool HaveSubscriptionFor(MmoGuid guid)
		{
			lock (subscribedGuids)
				return this.subscribedGuids.Contains(guid);
		}

		/// <summary>
		/// Called when the <see cref="MmoObject"/> is destroyed. Unsubscribes from the world message channel.
		/// </summary>
		protected override void OnDestroy()
		{
			if(movementUpdateSubscription != null)
			{
				this.movementUpdateSubscription.Dispose();
				this.movementUpdateSubscription = null;
			}
			this.CurrentZone.Physics.RemoveCharacterController(characterController);
			base.OnDestroy();
		}

		/// <summary>
		/// Updates <see cref="PlayerData"/> with the latest changes.
		/// </summary>
		public void UpdateSaveData(PlayerData playerDataInfo)
		{
			playerDataInfo.Money = this.Money;
			playerDataInfo.Xp = this.Xp;
			playerDataInfo.Level = this.Level;
			playerDataInfo.Position = this.Position.ToFloatArray(3);
			playerDataInfo.Orientation = this.Rotation.EularAngles().Y;
			playerDataInfo.ZoneId = this.CurrentZone.Id;
			playerDataInfo.Inventory = MmoSerializer.SerializePlayerInventory(inventory);
			playerDataInfo.Stats = this.GetBaseStats().ToArray();
			playerDataInfo.Spells = this.spellManager.ToDataField();
			playerDataInfo.ActionBar = MmoSerializer.SerializeActionbar(actionbar);
			playerDataInfo.CurrHealth = this.CurrentHealth;
			playerDataInfo.CurrMana = this.CurrentPower;
			playerDataInfo.FinishedQuests = this.finishedQuests.ToArray();
			playerDataInfo.CurrentQuests = MmoSerializer.SerializePlayerQuests(currentQuests);
			playerDataInfo.GroupGuid = this.InGroup() ? (long?) this.Group.Guid : null;
		}

		#endregion

		#region ServerToClient Methods

		/// <summary>
		/// Sends an event <see cref="ObjectFlagsSet"/> to set flags for an <see cref="MmoObject"/> to the client.
		/// </summary>
		private void SendSetFlags(MmoObject mmoObject, InterestFlags flags)
		{
			var objectFlagsSet = new ObjectFlagsSet
				{
					ObjectId = mmoObject.Guid,
					Flags = (int)flags
				};
			this.session.SendEvent(objectFlagsSet, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Sends an event <see cref="ObjectFlagsUnset"/> to unset flags for an <see cref="MmoObject"/> to the client.
		/// </summary>
		private void SendUnsetFlags(MmoObject mmoObject, InterestFlags flags)
		{
			var objectFlagsUnset = new ObjectFlagsUnset
				{
					ObjectId = mmoObject.Guid,
					Flags = (int) flags
				};
			this.session.SendEvent(objectFlagsUnset, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Sends an event <see cref="ObjectProperty"/> to the client.
		/// </summary>
		private void SendEventPropertySet(PropertyCode propertiesCode, object propertyData)
		{
			var objectProperty = new ObjectProperty
				{
					PropertiesCode = (byte) propertiesCode,
					EventData = propertyData
				};
			this.session.SendEvent(objectProperty, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Sends an error to the client
		/// </summary>
		public void ReportError(ResultCode returnCode)
		{
			if (returnCode != ResultCode.Ok)
			{
				this.session.SendOperationResponse(
					new GameErrorResponse(0) {ReturnCode = (short) returnCode},
					new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			}
		}

		#endregion

		#region Object Identification

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
		public override sealed bool IsMerchant()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
		public override sealed bool IsCivilian()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
		public override bool IsGuard()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		/// <returns></returns>
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
		/// Returns false.
		/// </summary>
		public override sealed bool IsSpellObject()
		{
			return false;
		}

		/// <summary>
		/// Returns false.
		/// </summary>
		public override sealed bool IsDoorway()
		{
			return false;
		}

		/// <summary>
		/// Returns false.
		/// </summary>
		public override sealed bool IsPortal()
		{
			return false;
		}

		#endregion

		#region Respawn System Implementation

		/// <summary>
		/// Called right before spawning
		/// </summary>
		protected override void OnAwake()
		{
		}

		/// <summary>
		/// Called when the <see cref="MmoObject"/> is spawned. Subscribes to the world message channel.
		/// </summary>
		protected override void OnSpawn()
		{
			base.OnSpawn();
			// setting the controller's position
			this.characterController.Position = Position;
#if MMO_DEBUG
			Logger.DebugFormat("Id: {0}. Type: Player. Name: {1}. Level: {2}. Position: {3}", this.Guid, this.Name, this.Level, this.Position);
#endif
		}

		#endregion

		#region Stats System Implementation

		/// <summary>
		/// Gains a level
		/// </summary>
		public void GainLevel()
		{
			// we cannot level past our max level
			if (Level == GlobalGameSettings.MAX_PLAYER_LEVEL)
				return;
			// update our current level
			this.SetLevel((byte) (Level + 1));
			// TODO: Assign skill point
			// dont need to publish any of these
			// the client will update them and revision after receiving LevelUp event
			this.SetHealth(MaximumHealth);
			this.SetPower(MaximumPower);
			// publishing the level up event on our event channel
			var objectLevelUp = new ObjectLevelUp {ObjectId = this.Guid};
			var message = new MmoObjectEventMessage(this, objectLevelUp, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			this.EventChannel.Publish(message);
			// notifying the client of OUR level up
			// remember we do not need to set the objectId since in this case the player who leveled up is US
			// the client will automatically predict its US when we send a level up event without the object id
			this.session.SendEvent(new YouLevelUp {Xp = xp}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			// updating our social profile via social manager
			this.World.SocialManager.UpdateProfile(Session.SessionId, new Hashtable {{(byte) ProfileProperty.Level, Level}});
		}

		/// <summary>
		/// Gains Xp
		/// </summary>
		public void GainXp(int value)
		{
			// we cannot gain any xp if we are at our max level
			if (Level == GlobalGameSettings.MAX_PLAYER_LEVEL)
				return;
			// set our current xp
			this.xp += value;
			// calculate the max xp for the current level
			var maxXp = Formulas.GetMaxXp(Level);
			// if the current xp is equal or greater than the current xp,
			if (xp >= maxXp)
			{
				// set the xp difference
				this.xp -= maxXp;
				// gain a level
				this.GainLevel();
			}
			else
			{
				// only publish xp if you have not gained a level
				// GainLevel will automatically send the current xp
				this.PublishProperty(PropertyCode.Xp, this.xp);
			}
		}

		/// <summary>
		/// Gains Gold
		/// </summary>
		public void GainGold(int value, bool publish)
		{
			// set the current gold
			var oldMoney = this.money;
			this.money = Mathf.Clamp(money + value, 0, GlobalGameSettings.MAX_PLAYER_GOLD);
			// only publish if the old and new value are different
			if (publish && oldMoney != money)
				this.PublishProperty(PropertyCode.Money, this.money);
		}

		/// <summary>
		/// Deducts gold
		/// </summary>
		public void DeductGold(int value, bool publish)
		{
			// set the current gold
			var oldMoney = this.money;
			this.money = Mathf.Clamp(money - value, 0, GlobalGameSettings.MAX_PLAYER_GOLD);
			// only publish if the old and new value are different
			if (publish && oldMoney != money)
				this.PublishProperty(PropertyCode.Money, this.money);
		}

		#endregion

		#region Property System Implementation

		/// <summary>
		/// Called when a property needs to be published to client(s). Publishes a <see cref="MmoObjectEventMessage"/> on the <see cref="MmoObject.EventChannel"/>.
		/// Filters builtProperties which are private and ignores them.
		/// </summary>
		/// <param name="propertyCode"></param>
		/// <param name="value"></param>
		protected override void PublishProperty(PropertyCode propertyCode, object value)
		{
			switch (propertyCode)
			{
				// unchanged builtProperties
				//case PropertyCode.Name:
				//case PropertyCode.Species:
				//case PropertyCode.Class:

				// public builtProperties
				case PropertyCode.Level:
				case PropertyCode.MaxHp:
				case PropertyCode.CurrHp:
				case PropertyCode.UnitState:
					this.SendEventPropertySet(propertyCode, value);
					break; // break

				// private builtProperties
				case PropertyCode.Money:
				case PropertyCode.Xp:
				case PropertyCode.Stats:
				case PropertyCode.MaxPow:
				case PropertyCode.CurrPow:
					this.SendEventPropertySet(propertyCode, value);
					return; // return

				default:
					Logger.WarnFormat("PublishProperty.UnlistedProperty={0}", propertyCode);
					break; // break
			}

			// publishing builtProperties on the event channel
			base.PublishProperty(propertyCode, value);
		}

		/// <summary>
		/// Adds or sets builtProperties into the <paramref name="builtProperties"/> based on <paramref name="flags"/>.
		/// </summary>
		/// <remarks>
		/// This does not generate a new <see cref="Hashtable"/> because this will be called extensively based on how much a(n) <see cref="Player"/>s
		/// property changes. Generating a new <see cref="Hashtable"/> everytime would be expensive.
		/// </remarks>
		public override sealed void BuildProperties(Hashtable builtProperties, PropertyFlags flags)
		{
			if (builtProperties == null)
				return;

			if ((flags & PropertyFlags.Name) == PropertyFlags.Name)
				builtProperties[PropertyCode.Name] = this.Name;

			if ((flags & PropertyFlags.Level) == PropertyFlags.Level)
				builtProperties[PropertyCode.Level] = this.Level;

			if ((flags & PropertyFlags.UnitState) == PropertyFlags.UnitState)
				builtProperties[PropertyCode.UnitState] = (byte) this.State;

			if ((flags & PropertyFlags.MaxHp) == PropertyFlags.MaxHp)
				builtProperties[PropertyCode.MaxHp] = this.MaximumHealth;

			if ((flags & PropertyFlags.CurrHp) == PropertyFlags.CurrHp)
				builtProperties[PropertyCode.CurrHp] = this.CurrentHealth;
		}

		/// <summary>
		/// Get flags for a certain <see cref="MmoObject"/>
		/// </summary>
		public InterestFlags GetFlags(MmoObject mmoObject)
		{
			var flags = InterestFlags.None;
			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Npc:
					var npc = (Npc) mmoObject;
					// if the npc is dead we cannot interact with it
					if (npc.IsDead())
						break;
					// does the npc have any quests ?
					if (npc.HaveQuests())
					{
						// figure out what is our status for the quest giver
						var pickupStatus = GetQuestGiverQuestPickupStatus(mmoObject);
						if (pickupStatus != QuestPickupStatus.Inactive)
						{
							// and set appropriate flags depending on the current status
							switch (pickupStatus)
							{
								case QuestPickupStatus.Active:
									flags |= InterestFlags.QuestActive;
									break;
								case QuestPickupStatus.InProgress:
									flags |= InterestFlags.QuestInProgress;
									break;
								case QuestPickupStatus.TurnIn:
									flags |= InterestFlags.QuestTurnIn;
									break;
							}
						}
					}
					// does the npc have any conversation for us ?
					if (HaveConversationFor(npc))
						flags |= InterestFlags.Conversation;
					// is the npc a merchant ?
					if (npc.IsMerchant())
						flags |= InterestFlags.Shopping;
					break;

				case ObjectType.Gameobject:
					var gameObject = (Gameobject) mmoObject;
					// can we use the gameobject ?
					if (gameObject.CanUse(this))
						flags |= InterestFlags.Usable;
					// can we gather the gameobject ?
					if (gameObject.CanGather(this))
						flags |= InterestFlags.Gatherable;
					// can we collect the gameobject ?ssss
					if (gameObject.CanCollect(this))
						flags |= InterestFlags.Collectible;
					// is the gameobject a merchant ?
					if (gameObject.IsMerchant())
						flags |= InterestFlags.Shopping;
					break;
			}
			// if the object has loot for us set the loot flag
			if (mmoObject.HaveLootFor(this))
				flags |= InterestFlags.Loot;
			// return the flags
			return flags;
		}

		#endregion

		#region Teleport System Implementation

		/// <summary>
		/// Teleports the player to a <see cref="GlobalPosition"/>.
		/// </summary>
		/// <param name="location"></param>
		public void TeleportTo(GlobalPosition location)
		{
			if (CurrentZone.Id == location.ZoneId)
			{
				// TODO: Send event
				// if the teleport position is within the zone simply spawn the player at that position
				// this.Spawn(location.Position, Rotation);
				// TODO: Send position
			}
			else
			{
				// stoping the player if he or she is already moving
				if (IsMoving)
					this.StopMovement(CurrentZone.World.GlobalTime);
				// finally transferring the player
				this.session.Transfer(location.ZoneId, location.Position);
			}
		}

		#endregion

		#region Movement System Implementation

		/// <summary>
		/// Called when a rotation request has been received
		/// </summary>
		public void SetOrientation(float newOrientation)
		{
			this.Transform.Rotation = Quaternion.CreateEular(0, newOrientation, 0);
			//this.worldDirection = Vector3.Transform(localDirection, this.Rotation).Normalize();
			this.characterController.Orientation = this.Rotation;
			
			// TODO: Correct rotation error

			var yRotation = this.Rotation.EularAngles().Y;
			if (Math.Abs(lastOrientation - yRotation) > 0.1f)
			{
				var rotationEvent = new ObjectTransform
					{
						ObjectId = this.Guid,
						Orientation = yRotation
					};
				var message = new MmoObjectEventMessage(this, rotationEvent, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
				this.EventChannel.Publish(message);
				this.lastOrientation = yRotation;
			}
		}

		/// <summary>
		/// Handles the movement keys
		/// </summary>
		public void HandleMovement(MovementKeys keys, int clientTime)
		{
			var dx = GetUnitSpeedX(keys);
			var dz = GetUnitSpeedZ(keys);

			var lSpeed = 0;
			var lDirection = MovementDirection.None;

			if (dx > 0)
			{
				lSpeed = GlobalGameSettings.PLAYER_SIDE_SPEED_MAX;
				lDirection = MovementDirection.Right;
			}
			else if (dx < 0)
			{
				lSpeed = GlobalGameSettings.PLAYER_SIDE_SPEED_MAX;
				lDirection = MovementDirection.Left;
			}

			if (dz > 0)
			{
				lSpeed = GlobalGameSettings.PLAYER_FORWARD_SPEED_MAX;
				lDirection = MovementDirection.Forward;
			}
			else if (dz < 0)
			{
				lSpeed = GlobalGameSettings.PLAYER_BACKWARD_SPEED_MAX;
				lDirection = MovementDirection.Backward;
			}

			if (lSpeed > 0)
			{
				// TODO: Handle walking
				this.currentSpeed = lSpeed;
				this.currentMovementState = MovementState.Running;
				this.currentDirectionFlags = lDirection;

				var localDirection = new Vector3(dx, 0, dz);
				var worldDirection = Vector3.Transform(localDirection, this.Rotation).Normalize();
				var newPosition = this.Position + worldDirection * this.currentSpeed * (CurrentZone.World.GlobalTime - clientTime) * 0.001f;
				newPosition = CurrentZone.Bounds.Clamp(newPosition);
				newPosition.Y = CurrentZone.GetHeight(newPosition.X, newPosition.Z) + this.characterController.Height * 0.5f;
				this.Transform.Position = newPosition;
				this.characterController.Position = newPosition;
				this.characterController.Orientation = this.Rotation;
				this.characterController.LocalVelocity = localDirection * this.currentSpeed;
				this.PublishUpdatedMotion();

				if (!IsMoving)
				{
					this.lastMovementUpdateTime = clientTime;
					// this will alert the aggroers immediately
					this.UpdateInterestManagement();
					this.movementUpdateSubscription = this.CurrentZone.PrimaryFiber.ScheduleOnInterval(() => this.UpdateMotion(), ServerGameSettings.GAME_UPDATE_INTERVAL);
				}
			}
		}

		/// <summary>
		/// Stops the movement
		/// </summary>
		public void StopMovement(int clientTime)
		{
			if (movementUpdateSubscription != null)
			{
				this.movementUpdateSubscription.Dispose();
				this.movementUpdateSubscription = null;
			}

			this.currentSpeed = 0;
			this.currentMovementState = MovementState.Idle;
			this.currentDirectionFlags = MovementDirection.None;

			var worldVelocity = this.characterController.WorldVelocity;
			var newPosition = this.characterController.Position - worldVelocity * (CurrentZone.World.GlobalTime - clientTime) * 0.001f;
			newPosition = CurrentZone.Bounds.Clamp(newPosition);
			newPosition.Y = CurrentZone.GetHeight(newPosition.X, newPosition.Z) + this.characterController.Height * .5f + this.characterController.Radius;
			this.characterController.LocalVelocity = Vector3.Zero;
			this.characterController.Position = newPosition;
			this.Transform.Position = newPosition;
			this.PublishUpdatedMotion();

			if (lastMovementUpdateTime < clientTime)
				this.UpdateMotion(true);
		}

		private void PublishUpdatedMotion()
		{
			var speedChanged = (lastSpeed != this.currentSpeed);
			var flagsChanged = (lastDirectionFlags != this.currentDirectionFlags);
			var stateChanged = (lastMovementState != this.currentMovementState);

			if (speedChanged || stateChanged || flagsChanged)
			{
				var movementEvent = new ObjectMovement
					{
						ObjectId = this.Guid,
					};

				if (speedChanged)
				{
					movementEvent.Speed = (byte)this.currentSpeed;
					this.lastSpeed = this.currentSpeed;
				}

				var yRotation = this.Rotation.EularAngles().Y;
				if (Math.Abs(lastOrientation - yRotation) > 0.1f)
				{
					movementEvent.Orientation = yRotation;
					this.lastOrientation = yRotation;
				}

				if (flagsChanged)
				{
					movementEvent.Direction = (byte)this.currentDirectionFlags;
					this.lastDirectionFlags = this.currentDirectionFlags;
				}

				if (stateChanged)
				{
					movementEvent.State = (byte)this.currentMovementState;
					this.lastMovementState = this.currentMovementState;
				}

				var message = new MmoObjectEventMessage(this, movementEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
				this.EventChannel.Publish(message);
			}
		}

		/// <summary>
		/// Updates the <see cref="Player"/>'s motion.
		/// </summary>
		private void UpdateMotion(bool publishImmediate = false)
		{
#if ! USE_PHYSICS
			this.characterController.Update((World.GlobalTime - lastMovementUpdateTime) * 0.001f);
#endif
			// validating the current position
			var newPosition = this.characterController.Position;
			// if we are not within the zone boundary so we need to correct our position
			// we do not support zone to zone transfers within the same world because right now each zone gets its own world (server)
			if (!CurrentZone.Bounds.Contains(newPosition))
			{
				// we are not within the world which means we need to correct our position to the last valid position
				newPosition = this.Transform.Position;
				this.characterController.Position = newPosition;
#if MMO_DEBUG
				Logger.WarnFormat("correcting position");
#endif
				// right now each zone gets its own server so the world bounds dont matter
				// but in the future we will add more zones to a world and allow seamless zone transfer
				/*
				if (!World.World.Bounds.Contains(newPosition))
				{
					// we are not within the world which means we need to correct our position to the last valid position
					this.characterController.Position = this.Transform.Position;
				}
				else
				{
					throw new NotImplementedException("TeleportToZoneWithinWorld");
				}
				*/
			}

			this.Transform.Position = newPosition;
			if (publishImmediate || nextInterestManagementMoveTime <= CurrentZone.World.GlobalTime)
			{
				lock (Session.Camera.SyncRoot)
					Session.Camera.Move(Position);

				if (InGroup())
					this.MemberUpdateFlags |= GroupMemberPropertyFlags.PROPERTY_FLAG_POSITION;

				this.UpdateInterestManagement();
				this.nextInterestManagementMoveTime = CurrentZone.World.GlobalTime + ServerGameSettings.PLAYER_INTEREST_MANAGEMENT_UPDATE_INTERVAL;
			}

			if (publishImmediate || nextMovementPublishTime <= CurrentZone.World.GlobalTime)
			{
				var coord = this.Position.ToFloatArray(3);
				var positionEvent = new ObjectTransform
					{
						ObjectId = this.Guid,
						Position = coord
					};

				var playerPositionEvent = new ObjectTransform
					{
						Position = coord
					};
				
				var message = new MmoObjectEventMessage(this, positionEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
				this.EventChannel.Publish(message);
				this.session.SendEvent(playerPositionEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
				
				this.nextMovementPublishTime = CurrentZone.World.GlobalTime + ServerGameSettings.OBJECT_MOVEMENT_PUBLISH_INTERVAL;
			}

			this.lastMovementUpdateTime = CurrentZone.World.GlobalTime;
		}

		/// <summary>
		/// Sends a movement update to a(n) <see cref="Player"/>.
		/// </summary>
		public override void SendMovementUpdateTo(Player player)
		{
			var movementEvent = new ObjectMovement
				{
					ObjectId = this.Guid,
					Speed = (byte) this.currentSpeed,
					Orientation = this.Rotation.EularAngles().Y,
					Direction = (byte) this.currentDirectionFlags,
					State = (byte) this.currentMovementState
				};

			player.session.SendEvent(movementEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
		}

		/// <summary>
		/// Gets the unit x speed based on the keys pressed
		/// </summary>
		private static int GetUnitSpeedX(MovementKeys keys)
		{
			var dx = 0;
			if ((keys & MovementKeys.TurnRight) == MovementKeys.TurnRight)
				dx++;

			if ((keys & MovementKeys.TurnLeft) == MovementKeys.TurnLeft)
				dx--;

			return dx;
		}

		/// <summary>
		/// Gets the unit y speed based on the keys pressed
		/// </summary>
		private static int GetUnitSpeedZ(MovementKeys keys)
		{
			var dz = 0;
			if ((keys & MovementKeys.Steer) == MovementKeys.Steer)
				dz++;

			if ((keys & MovementKeys.Reverse) == MovementKeys.Reverse)
				dz--;

			return dz;
		}

		#endregion

		#region Interaction System Implementation

		/// <summary>
		/// Sets the current interactor
		/// </summary>
		public void SetInteractor(MmoObject item)
		{
			if (interactorDistanceCheckSubscription != null)
			{
				this.interactorDistanceCheckSubscription.Dispose();
				this.interactorDistanceCheckSubscription = null;
			}
			
			this.currentInteractor = item;
			if (currentInteractor != null)
			{
				this.interactorDistanceCheckSubscription = this.CurrentZone.PrimaryFiber.ScheduleOnInterval(() =>
					{
						const float minInteractionRange = GlobalGameSettings.MIN_INTERACTION_RANGE;
						if (Vector3.SqrDistance(currentInteractor.Position, Position) > minInteractionRange * minInteractionRange)
							this.SetInteractor(null);
					}, 1000);
			}
		}

		/// <summary>
		/// Tells the client to display loot
		/// </summary>
		public void DisplayLoot(MmoObject sender)
		{
			this.session.SendEvent(new InteractionLoot {ObjectId = sender.Guid}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Begins using a certain <see cref="MmoObject"/>.
		/// </summary>
		/// <param name="mmoObject"></param>
		public void BeginUsing(MmoObject mmoObject)
		{
			this.spellManager.CastUse(mmoObject);
		}

		/// <summary>
		/// Stops using a certain <see cref="MmoObject"/>.
		/// </summary>
		/// <param name="mmoObject"></param>
		public void EndUsing(MmoObject mmoObject)
		{
			this.spellManager.StopUse();
		}

		/// <summary>
		/// Hides an <see cref="MmoObject"/> from this <see cref="Player"/>'s view.
		/// </summary>
		public void HideObjectFromView(MmoObject mmoObject)
		{
			lock (subscribedGuids)
			{
				// if the object is not already in our view then we dont have to unsubscribe it
				if (!subscribedGuids.Contains(mmoObject.Guid))
					return;
			}

			// unsubscribes the object from our view so the client will be notified
			this.session.Camera.UnsubscribeManually(mmoObject);
		}

		/// <summary>
		/// Unhides an <see cref="MmoObject"/> from this <see cref="Player"/>'s view.
		/// </summary>
		public void UnhideObjectFromView(MmoObject mmoObject)
		{
			lock (subscribedGuids)
			{
				// if the object is not already in our view then we dont have to subscribe it
				if (!subscribedGuids.Contains(mmoObject.Guid))
					return;
			}

			// subscribes the object so we will receive updates
			this.session.Camera.AutoSubscribeManually(mmoObject);
		}

		#endregion

		#region Inventory System Implementation

		/// <summary>
		/// Called when an item is added to the inventory
		/// </summary>
		void Inventory_OnItemAdded(IMmoItem item, int count, int index)
		{
			// if we added an item that can be used let the spell manager know
			// since a usable item has a spell associated with it, this is the proper way to do it
			if (item.IsUsable)
				this.spellManager.AddUsableItem(item, count);

			// explicit conversion ItemInfo to int
			var itemData = new ContainerItemStructure(item.Id, (byte) index, (byte) count);
			if (clusterAddition)
			{
				// if the cluster was invoked add it to the cluster
				// remember cluster addition only sends the added items after a call to EndInventoryClusterAddition
				this.itemCluster.Add(itemData);
			}
			else
			{
				// no cluster addition, then add it normally
				// we do not need to provide the container type explicitly because the client can predict it based off the item type
				this.session.SendEvent(new InventoryItemAdded {ItemData = itemData}, new MessageParameters {ChannelId = PeerSettings.InventoryEventChannel});
			}

			// update quests that require possesion of that item
			this.UpdateQuestsForItemAddition(item, count);
		}

		/// <summary>
		/// Called when an item is removed from the inventory
		/// </summary>
		void Inventory_OnItemRemoved(IMmoItem item, int count, int index)
		{
			// if we removed an item that can be used let the spell manager know
			// since a usable item has a spell associated with it, this is the proper way to do it
			if (item.IsUsable)
				this.spellManager.RemoveUsableItem(item, count);
			// notifying the client
			var removeGeneraltem = new InventoryItemRemoved
				{
					Index = (byte) index,
					Quantity = (byte) count
				};
			this.session.SendEvent(removeGeneraltem, new MessageParameters { ChannelId = PeerSettings.InventoryEventChannel });
			// update quests that require possesion of that item
			this.UpdateQuestsForItemRemoval(item, count);
		}

		/// <summary>
		/// Prepares the inventory for cluster item addition
		/// </summary>
		public void BeginInventoryClusterAddition()
		{
			this.clusterAddition = true;
		}

		/// <summary>
		/// Ends inventory cluster addition
		/// </summary>
		public void EndInventoryClusterAddition()
		{
			// if there are items in the cluster
			if (itemCluster.Count > 0)
			{
				// send and clear the custer
				var itemAddedMultiple = new InventoryItemAddedMultiple
					{
						Items = itemCluster.ToArray()
					};
				this.session.SendEvent(itemAddedMultiple, new MessageParameters {ChannelId = PeerSettings.InventoryEventChannel});
				this.itemCluster.Clear();
			}

			this.clusterAddition = false;
		}

		#endregion

		#region Combat System Implementation

		/// <summary>
		/// Returns whether the <see cref="Player"/> is dead or not.
		/// </summary>
		/// <returns></returns>
		public override bool IsDead()
		{
			return State == UnitState.Dead;
		}

		/// <summary>
		/// Tells whether a <see cref="Character"/> is hostile to this <see cref="Player"/> or not.
		/// </summary>
		public override bool IsHostileTo(Character character)
		{
			return character.IsNpc() && ((Npc)character).NpcType == NpcType.Enemy;
		}

		/// <summary>
		/// Called when this <see cref="Character"/> is killed. Remember the <paramref name="killer"/> could be <value>this</value>.
		/// </summary>
		protected override void OnDeath(MmoObject killer)
		{
		}

		/// <summary>
		/// Rewards the <see cref="Player"/> of the kill.
		/// </summary>
		/// <param name="killed"></param>
		public override void RewardKill(MmoObject killed)
		{
			switch ((ObjectType)killed.Guid.Type)
			{
				case ObjectType.Npc:
					{
						var npc = killed as Npc;
						if (npc != null)
						{
							var gain = Formulas.GetXpGainForKill(this.Level, npc.Level);

							if (InGroup())
								this.Group.GainXp(this, gain);
							else
								this.GainXp(gain);

							if (currentQuests.Count > 0)
								this.UpdateQuestsForKill(npc);
						}
					}
					break;
			}
		}

		#endregion

		#region Spell System Implementation

		/// <summary>
		/// Informs the client to start casting a spell
		/// </summary>
		public void BeginClientCast(short spellId)
		{
			this.session.SendEvent(new SpellCastBegin {SpellId = spellId}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Informs the client to end casting
		/// </summary>
		public void EndClientCast()
		{
			this.session.SendEvent(new SpellCastEnd(), new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
		}

		/// <summary>
		/// Informs the client to start a cooldown of a spell
		/// </summary>
		public void BeginClientCooldown(short spellId)
		{
			this.session.SendEvent(new SpellCooldownBegin {SpellId = spellId}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Informs the client to end a cooldown of the currently casting spell
		/// </summary>
		public void EndClientCooldown(short spellId)
		{
			this.session.SendEvent(new SpellCooldownEnd {SpellId = spellId}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Called when a spell is added
		/// </summary>
		void SpellManager_OnSpellAdded(Spell spell)
		{
			this.session.SendEvent(new SpellAdded {SpellId = spell.Id}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		/// <summary>
		/// Called when a spell is removed
		/// </summary>
		void SpellManager_OnSpellRemoved(Spell spell)
		{
			this.session.SendEvent(new SpellRemoved {SpellId = spell.Id}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
		}

		#endregion

		#region Conversation System Implementation

		/// <summary>
		/// Determines whether the <see cref="Player"/> can talk to an <see cref="MmoObject"/> or not.
		/// </summary>
		public bool CanTalkTo(MmoObject mmoObject)
		{
			return (HaveConversationFor(mmoObject) || HaveQuestsFor(mmoObject));
		}

		/// <summary>
		/// Performs a conversation with a certain <see cref="MmoObject"/>
		/// </summary>
		public void PerformConversation(MmoObject mmoObject, short conversationId)
		{
			this.UpdateQuestsForTalk(mmoObject, conversationId);
			if (!HaveConversationFor(mmoObject))
				this.SendUnsetFlags(mmoObject, InterestFlags.Conversation);
		}

		/// <summary>
		/// Prepares dialogue for an <see cref="MmoObject"/>.
		/// </summary>
		public void PrepareGossipDialogueFor(MmoObject mmoObject, IList<MenuItemStructure> dialogues)
		{
			foreach (var entry in currentQuests)
			{
				var questProgression = entry.Value;
				if (questProgression.Status != QuestStatus.InProgress)
					continue;

				QuestData questData;
				if (!MmoWorld.Instance.ItemCache.TryGetQuestData(entry.Key, out questData))
				{
					Logger.ErrorFormat("[HaveConversationFor]: Quest (Id={0}) not found", entry.Key);
					continue;
				}

				if ((questData.Flags & QuestFlags.QUEST_FLAG_TALK) != QuestFlags.QUEST_FLAG_TALK)
					continue;

				var npcRequirements = questData.NpcRequirements;
				for (byte npcIndex = 0; npcIndex < npcRequirements.Length; npcIndex++)
				{
					var objectId = mmoObject.Guid.Id;
					if (npcRequirements[npcIndex].Id != objectId)
						continue;

					if (questProgression.NpcCount[npcIndex] > 0)
						continue;

					foreach (var conversation in questData.QuestConversations)
					{
						if (conversation.ObjectId == objectId)
							dialogues.Add(new MenuItemStructure(conversation.ConversationId, (byte) MenuItemType.Conversation, (byte) MenuIconType.Conversation));
					}
				}
			}
		}

		/// <summary>
		/// Determines whether an <see cref="MmoObject"/> has a conversation or not.
		/// </summary>
		bool HaveConversationFor(MmoObject mmoObject)
		{
			foreach (var entry in currentQuests)
			{
				var currentQuest = entry.Value;
				if (currentQuest.Status != QuestStatus.InProgress)
					continue;

				QuestData questData;
				if (!MmoWorld.Instance.ItemCache.TryGetQuestData(entry.Key, out questData))
				{
					Logger.ErrorFormat("[HaveConversationFor]: Quest (Id={0}) not found", entry.Key);
					continue;
				}

				if ((questData.Flags & QuestFlags.QUEST_FLAG_TALK) != QuestFlags.QUEST_FLAG_TALK)
					continue;

				var npcRequirements = questData.NpcRequirements;
				for (byte npcIndex = 0; npcIndex < npcRequirements.Length; npcIndex++)
				{
					var objectId = mmoObject.Guid.Id;
					if (npcRequirements[npcIndex].Id != objectId)
						continue;

					if (currentQuest.NpcCount[npcIndex] > 0)
						continue;

					if (questData.QuestConversations.Any(conversation => conversation.ObjectId == objectId))
						return true;
				}
			}

			return false;
		}

		#endregion

		#region Quest System Implementation

		/// <summary>
		/// Adds a quest
		/// </summary>
		public void AddQuest(short questId, MmoObject mmoObject)
		{
			QuestData questData;
			if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
			{
#if MMO_DEBUG
				Logger.DebugFormat("[AddQuest]: Quest (Id={0}) not found", questId);
#endif
				return;
			}

			if (CanAddQuest(questData, true))
			{
				var questProgression = new QuestProgression {Status = QuestStatus.InProgress};
				// setup the requirements count for kills
				var questFlags = questData.Flags;
				if ((questFlags & QuestFlags.QUEST_FLAG_KILL) == QuestFlags.QUEST_FLAG_KILL || (questFlags & QuestFlags.QUEST_FLAG_TALK) == QuestFlags.QUEST_FLAG_TALK)
				{
					questProgression.NpcCount = new byte[questData.NpcRequirements.Length];
					questProgression.PlayerCount = 0;
				}
				// setup the requirements count for items
				if ((questFlags & QuestFlags.QUEST_FLAG_DELIVER) == QuestFlags.QUEST_FLAG_DELIVER)
					questProgression.ItemCount = new byte[questData.ItemRequirements.Length];
				// add the quest to our collection
				this.currentQuests.Add(questData.QuestId, questProgression);
				// resets the pickup status of the quest giver
				this.SetQuestGiverQuestPickupStatus(mmoObject, this.GetQuestGiverQuestPickupStatus(mmoObject));
				// notifying our client to start the quest
				this.session.SendEvent(new QuestStarted {QuestId = questId}, new MessageParameters {ChannelId = PeerSettings.QuestEventChannel});
				// updates the quest
				this.UpdateQuest(questId);
				this.UpdateSubscribersForQuestStatusChange(questData.QuestId);
			}
		}

		/// <summary>
		/// Determines whether a quest can be completed
		/// </summary>
		public bool CanCompleteQuest(short questId)
		{
			QuestProgression questProgression;
			if (!currentQuests.TryGetValue(questId, out questProgression))
				return false;

			if (questProgression.Status == QuestStatus.TurnIn)
				return true;

			QuestData questData;
			if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
			{
#if MMO_DEBUG
				Logger.DebugFormat("[CanCompleteQuest]: Quest (Id={0}) not found", questId);
#endif
				return false;
			}

			if ((questData.Flags & QuestFlags.QUEST_FLAG_COMPLETE_ON_PICKUP) == QuestFlags.QUEST_FLAG_COMPLETE_ON_PICKUP)
				return true;

			var itemRequirements = questData.ItemRequirements;
			if (itemRequirements != null)
			{
				for (var i = 0; i < itemRequirements.Length; i++)
					if (questProgression.ItemCount[i] < itemRequirements[i].Count)
						return false;
			}

			var npcRequirements = questData.NpcRequirements;
			if (npcRequirements != null)
			{
				for (var i = 0; i < npcRequirements.Length; i++)
					if (questProgression.NpcCount[i] < npcRequirements[i].Count)
						return false;
			}

			return questData.PlayerRequirements >= questProgression.PlayerCount;
		}

		/// <summary>
		/// Completes a quest
		/// </summary>
		public void CompleteQuest(short questId)
		{
			QuestProgression questProgression;
			if (!currentQuests.TryGetValue(questId, out questProgression))
			{
#if MMO_DEBUG
				Logger.DebugFormat("[CompleteQuest]: Quest (Id={0}) not found", questId);
#endif
				return;
			}

			if (questProgression.Status == QuestStatus.TurnIn)
				return;

			questProgression.Status = QuestStatus.TurnIn;
			this.UpdateSubscribersForQuestStatusChange(questId);
		}

		/// <summary>
		/// Finishes a quest
		/// </summary>
		public void FinishQuest(short questId, MmoObject mmoObject)
		{
			QuestData questData;
			if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
			{
#if MMO_DEBUG
				Logger.DebugFormat("[FinishQuest]: Quest (Id={0}) not found", questId);
#endif
				return;
			}

			if (CanFinishQuest(questData, true))
			{
				var questFlags = questData.Flags;
				if ((questFlags & QuestFlags.QUEST_FLAG_AUTO_REWARD) != QuestFlags.QUEST_FLAG_AUTO_REWARD)
				{
					// if cannot be rewarded due to inventory space don't complete the quest
					if (!CanRewardQuest(questData, true))
						return;
					// otherwise auto-reward quest
					this.RewardQuest(questData);
				}

				this.currentQuests.Remove(questId);
				this.finishedQuests.Add(questId);

				if ((questFlags & QuestFlags.QUEST_FLAG_DELIVER) == QuestFlags.QUEST_FLAG_DELIVER)
				{
					var itemRequirements = questData.ItemRequirements;
					if (itemRequirements != null)
					{
						foreach (var itemRequirement in itemRequirements)
							this.inventory.RemoveItem(itemRequirement.Id, itemRequirement.Count);
					}
				}

				// resets the pickup status of the quest giver
				this.SetQuestGiverQuestPickupStatus(mmoObject, this.GetQuestGiverQuestPickupStatus(mmoObject));
				// notifying our client to finish the quest
				this.session.SendEvent(new QuestFinished {QuestId = questId}, new MessageParameters {ChannelId = PeerSettings.QuestEventChannel});
				this.UpdateSubscribersForQuestStatusChange(questData.QuestId);
			}
		}

		/// <summary>
		/// Prepares quest dialogue for an <see cref="MmoObject"/>.
		/// </summary>
		public void PrepareQuestDialogueFor(MmoObject mmoObject, IList<MenuItemStructure> dialogues)
		{
			var completableQuests = mmoObject.GetCompletableQuests();
			if (completableQuests != null)
			{
				foreach (var questId in completableQuests)
				{
					switch (GetQuestStatus(questId))
					{
						case QuestStatus.TurnIn:
							dialogues.Add(new MenuItemStructure(questId, (byte)MenuItemType.Quest, (byte)MenuIconType.QuestTurnIn));
							break;
						case QuestStatus.InProgress:
							dialogues.Add(new MenuItemStructure(questId, (byte)MenuItemType.Quest, (byte)MenuIconType.QuestInProgress));
							break;
					}
				}
			}

			var startableQuests = mmoObject.GetStartableQuests();
			if (startableQuests == null)
				return;

			foreach (var questId in startableQuests)
			{
				if (GetQuestStatus(questId) == QuestStatus.None)
				{
					QuestData questData;
					if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
					{
						Logger.ErrorFormat("[PrepareQuestDialogueFor]: Quest (Id={0}) not found", questId);
						continue;
					}

					if (CanPickupQuest(questData, false))
						dialogues.Add(new MenuItemStructure(questId, (byte)MenuItemType.Quest, (byte)MenuIconType.QuestActive));
				}
			}
		}

		/// <summary>
		/// Gets the quest pickup status for an <see cref="MmoObject"/>
		/// </summary>
		QuestPickupStatus GetQuestGiverQuestPickupStatus(MmoObject mmoObject)
		{
			var pickupStatus = QuestPickupStatus.Inactive;

			var completableQuests = mmoObject.GetCompletableQuests();
			if(completableQuests == null)
				return pickupStatus;

			foreach (var questId in completableQuests)
			{
				switch (GetQuestStatus(questId))
				{
					case QuestStatus.TurnIn:
						{
							if (pickupStatus < QuestPickupStatus.TurnIn)
								pickupStatus = QuestPickupStatus.TurnIn;
						}
						break;

					case QuestStatus.InProgress:
						{
							if (pickupStatus < QuestPickupStatus.InProgress)
								pickupStatus = QuestPickupStatus.InProgress;
						}
						break;
				}
			}

			if (pickupStatus != QuestPickupStatus.TurnIn)
			{
				var activeQuests = mmoObject.GetStartableQuests();
				if (activeQuests == null)
					return pickupStatus;

				foreach (var questId in activeQuests)
				{
					if (GetQuestStatus(questId) == QuestStatus.None)
					{
						QuestData questData;
						if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
						{
							Logger.ErrorFormat("[GetQuestGiverQuestPickupStatus]: Quest (Id={0}) not found", questId);
							continue;
						}

						if (CanViewQuest(questData))
						{
							if (pickupStatus < QuestPickupStatus.Active)
								pickupStatus = QuestPickupStatus.Active;
						}
					}
				}
			}

			return pickupStatus;
		}
		
		/// <summary>
		/// Sets the pickup status of a quest giver
		/// </summary>
		void SetQuestGiverQuestPickupStatus(MmoObject mmoObject, QuestPickupStatus status)
		{
			var questGiverStatus = new QuestGiverStatus
				{
					ObjectId = mmoObject.Guid,
					Status = (byte) status
				};
			this.session.SendEvent(questGiverStatus, new MessageParameters {ChannelId = PeerSettings.QuestEventChannel});
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has quests for this <see cref="Player"/>.
		/// </summary>
		bool HaveQuestsFor(MmoObject mmoObject)
		{
			var completableQuests = mmoObject.GetCompletableQuests();
			if (completableQuests != null)
			{
				foreach (var questId in completableQuests)
				{
					var questStatus = this.GetQuestStatus(questId); // can be optimized by only searching current quests but not going to make a huge diff :P
					if (questStatus == QuestStatus.TurnIn || questStatus == QuestStatus.InProgress)
						return true;
				}
			}

			var startableQuests = mmoObject.GetStartableQuests();
			if (startableQuests != null)
			{
				foreach (var questId in startableQuests)
				{
					if (GetQuestStatus(questId) == QuestStatus.None)
					{
						QuestData questData;
						if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
						{
							Logger.ErrorFormat("[HaveQuestsFor]: Quest (Id={0}) not found", questId);
							continue;
						}

						if (CanPickupQuest(questData, false))
							return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the status of a quest
		/// </summary>
		QuestStatus GetQuestStatus(short questId)
		{
			if (finishedQuests.Contains(questId))
				return QuestStatus.Finished;

			QuestProgression questProgression;
			return this.currentQuests.TryGetValue(questId, out questProgression) ? questProgression.Status : QuestStatus.None;
		}

		/// <summary>
		/// Determines whether a quest can be viewed or not
		/// </summary>
		bool CanViewQuest(QuestData questData)
		{
			if (HaveMetQuestLevelReq(questData, false) && HaveMetQuestStateReq(questData, false) && HaveMetPrevQuestReq(questData, false))
				return questData.Level >= this.Level - 3;

			return false;
		}

		/// <summary>
		/// Determines whether a quest can be picked up or not
		/// </summary>
		bool CanPickupQuest(QuestData questData, bool notifyClient)
		{
			return HaveMetQuestLevelReq(questData, notifyClient) &&
			       HaveMetQuestStateReq(questData, notifyClient) &&
			       HaveMetQuestStatusReq(questData, notifyClient) &&
			       HaveMetPrevQuestReq(questData, notifyClient);
		}

		/// <summary>
		/// Determines whether a quest can be added or not
		/// </summary>
		bool CanAddQuest(QuestData questData, bool notifyClient)
		{
			if (!CanPickupQuest(questData, notifyClient))
				return false;

			if (currentQuests.Count >= ServerGameSettings.MAX_ACTIVE_QUESTS)
			{
				if (notifyClient)
					this.ReportError(ResultCode.QuestJournalFull);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether a quest can be finished or not
		/// </summary>
		bool CanFinishQuest(QuestData questData, bool notifyClient)
		{
			if (GetQuestStatus(questData.QuestId) != QuestStatus.TurnIn)
			{
				if (notifyClient)
					this.ReportError(ResultCode.QuestNotCompleted);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether a quest can be rewarded or not
		/// </summary>
		bool CanRewardQuest(QuestData questData, bool notifyClient)
		{
			if ((questData.Flags & QuestFlags.QUEST_FLAG_AUTO_REWARD) == QuestFlags.QUEST_FLAG_AUTO_REWARD)
				return false;

			var rewardItems = questData.RewardItems;
			if (rewardItems != null)
			{
				foreach (var rewardItem in rewardItems)
				{
					if (!inventory.CanAddItem(rewardItem.ItemId, rewardItem.Count))
					{
						if (notifyClient)
							this.ReportError(ResultCode.InventoryFull);
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Rewards a quest
		/// </summary>
		void RewardQuest(QuestData questInfo)
		{
			var rewardItems = questInfo.RewardItems;
			if (rewardItems != null)
				foreach (var rewardItem in rewardItems)
					this.inventory.AddItem(rewardItem.ItemId, rewardItem.Count);

			if (questInfo.RewardXp > 0)
				this.GainXp(questInfo.RewardXp);

			if (questInfo.RewardMoney > 0)
				this.GainGold(questInfo.RewardMoney, true);
		}

		/// <summary>
		/// Determines whether the <see cref="Player"/> has met level requirement for the <see cref="QuestFlags"/>.
		/// </summary>
		bool HaveMetQuestLevelReq(QuestData questData, bool notifyClient)
		{
			if (questData.Level > this.Level + 3)
			{
				if (notifyClient)
					this.ReportError(ResultCode.YouAreTooLowForThisQuest);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the <see cref="Player"/> has met state requirement for the <see cref="QuestFlags"/>.
		/// </summary>
		bool HaveMetQuestStateReq(QuestData questData, bool notifyClient)
		{
			if (IsDead())
			{
				if (notifyClient)
					this.ReportError(ResultCode.CannotAcceptQuestWhileDead);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the <see cref="Player"/> has met state requirement for the <see cref="QuestFlags"/>.
		/// </summary>
		bool HaveMetQuestStatusReq(QuestData questData, bool notifyClient)
		{
			var questStatus = this.GetQuestStatus(questData.QuestId);
			if (questStatus == QuestStatus.Finished)
			{
				if (notifyClient)
					this.ReportError(ResultCode.QuestAlreadyCompleted);
				return false;
			}

			if (questStatus != QuestStatus.None)
			{
				if (notifyClient)
					this.ReportError(ResultCode.QuestAlreadyAccepted);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the <see cref="Player"/> has met state requirement for the <see cref="QuestFlags"/>.
		/// </summary>
		bool HaveMetPrevQuestReq(QuestData questData, bool notifyClient)
		{
			if (questData.PrevQuest == 0)
				return true;

			var questStatus = this.GetQuestStatus(questData.PrevQuest);
			if (questStatus != QuestStatus.Finished)
			{
				if (notifyClient)
					this.ReportError(ResultCode.QuestNotCompleted);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Updates a quest
		/// </summary>
		void UpdateQuest(short questId)
		{
			QuestProgression questProgression;
			if (!currentQuests.TryGetValue(questId, out questProgression))
				return;

			if (questProgression.Status != QuestStatus.InProgress)
				return;

			QuestData questData;
			if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
			{
				Logger.ErrorFormat("[UpdateQuest]: Quest (Id={0}) not found", questId);
				return;
			}

			var questFlags = questData.Flags;
			if ((questFlags & QuestFlags.QUEST_FLAG_DELIVER) != QuestFlags.QUEST_FLAG_DELIVER)
				return;

			var itemRequirments = questData.ItemRequirements;
			for (byte itemIndex = 0; itemIndex < itemRequirments.Length; itemIndex++)
			{
				var itemRequirement = itemRequirments[itemIndex];
				var inventoryItemCount = this.inventory.GetItemCount(itemRequirement.Id);

				if (inventoryItemCount < 1)
					continue;
				
				var currentCounts = questProgression.ItemCount;
				var currentCount = currentCounts[itemIndex];

				if (currentCount == inventoryItemCount)
					continue;

				currentCount = (byte) Mathf.Min(inventoryItemCount, itemRequirement.Count);
				currentCounts[itemIndex] = currentCount;

				var questProgress = new QuestProgressStructure(questId, itemIndex, currentCount);
				this.session.SendEvent(new QuestProgress {QuestProgressData = questProgress}, new MessageParameters {ChannelId = PeerSettings.QuestEventChannel});
			}
		}

		/// <summary>
		/// Update quests for talk
		/// </summary>
		void UpdateQuestsForTalk(MmoObject mmoObject, short conversationId)
		{
			foreach (var entry in currentQuests)
			{
				var questProgression = entry.Value;
				if (questProgression.Status != QuestStatus.InProgress)
					continue;

				QuestData questData;
				if (!MmoWorld.Instance.ItemCache.TryGetQuestData(entry.Key, out questData))
				{
					Logger.ErrorFormat("[UpdateQuestsForTalk]: Quest (Id={0}) not found", entry.Key);
					continue;
				}

				if ((questData.Flags & QuestFlags.QUEST_FLAG_TALK) != QuestFlags.QUEST_FLAG_TALK)
					continue;

				var npcRequirements = questData.NpcRequirements;
				for (byte npcIndex = 0; npcIndex < npcRequirements.Length; npcIndex++)
				{
					// matching npc id
					var objectId = mmoObject.Guid.Id;

					var npcRequirement = npcRequirements[npcIndex];
					if (npcRequirement.Id != objectId)
						continue;

					var questConversations = questData.QuestConversations;
					for (byte conversationIndex = 0; conversationIndex < questConversations.Length; conversationIndex++)
					{
						var questConversation = questConversations[conversationIndex];
						if (questConversation.ObjectId != objectId)
							continue;

						if (questConversation.ConversationId != conversationId)
							continue;

						var currentCounts = questProgression.NpcCount;

						var currentCount = currentCounts[conversationIndex];
						if (currentCount >= npcRequirement.Count)
							continue;

						currentCounts[npcIndex] = ++currentCount;

						var questId = questData.QuestId;
						if (CanCompleteQuest(questId))
							this.CompleteQuest(questId);

						// figuring out the sequencial index of the objective count index
						var objectiveIndex = npcIndex;
						if (questData.ItemRequirements != null)
							objectiveIndex += (byte) questData.ItemRequirements.Length;

						var questProgress = new QuestProgressStructure(questId, objectiveIndex, currentCount);
						this.session.SendEvent(new QuestProgress { QuestProgressData = questProgress }, new MessageParameters { ChannelId = PeerSettings.QuestEventChannel });
						return;
					}
				}
			}
		}

		/// <summary>
		/// Update quests for kill
		/// </summary>
		void UpdateQuestsForKill(MmoObject mmoObject)
		{
			foreach (var entry in currentQuests)
			{
				var questProgression = entry.Value;
				if (questProgression.Status != QuestStatus.InProgress)
					continue;

				QuestData questData;
				if (!MmoWorld.Instance.ItemCache.TryGetQuestData(entry.Key, out questData))
				{
					Logger.ErrorFormat("[UpdateQuestsForKill]: Quest (Id={0}) not found", entry.Key);
					continue;
				}

				if ((questData.Flags & QuestFlags.QUEST_FLAG_KILL) != QuestFlags.QUEST_FLAG_KILL)
					continue;

				var npcRequirements = questData.NpcRequirements;
				for (byte npcIndex = 0; npcIndex < npcRequirements.Length; npcIndex++)
				{
					// TODO: Replace family id with Guid
					var npcRequirement = npcRequirements[npcIndex];
					if (npcRequirement.Id != mmoObject.FamilyId)
						continue;

					var currentCounts = questProgression.NpcCount;

					var currentCount = currentCounts[npcIndex];
					if (currentCount >= npcRequirement.Count)
						continue;

					currentCounts[npcIndex] = ++currentCount;

					var questId = questData.QuestId;
					if (CanCompleteQuest(questId))
						CompleteQuest(questId);

					// figuring out the sequencial index of the objective count index
					int objectiveIndex = npcIndex;
					if (questData.ItemRequirements != null)
						objectiveIndex += questData.ItemRequirements.Length;

					var questProgress = new QuestProgressStructure(questId, (byte)objectiveIndex, currentCounts[npcIndex]);
					this.session.SendEvent(new QuestProgress { QuestProgressData = questProgress }, new MessageParameters { ChannelId = PeerSettings.QuestEventChannel });
					break;
				}
			}
		}

		/// <summary>
		/// Update quests for item addition
		/// </summary>
		void UpdateQuestsForItemAddition(IMmoItem mmoItem, int count)
		{
			foreach (var entry in currentQuests)
			{
				var questProgression = entry.Value;
				if (questProgression.Status != QuestStatus.InProgress)
					continue;

				QuestData questData;
				if (!MmoWorld.Instance.ItemCache.TryGetQuestData(entry.Key, out questData))
				{
					Logger.ErrorFormat("[UpdateQuestsForTalk]: Quest (Id={0}) not found", entry.Key);
					continue;
				}

				if ((questData.Flags & QuestFlags.QUEST_FLAG_DELIVER) != QuestFlags.QUEST_FLAG_DELIVER)
					continue;

				var itemRequirements = questData.ItemRequirements;
				for (byte itemIndex = 0; itemIndex < itemRequirements.Length; itemIndex++)
				{
					var itemRequirement = itemRequirements[itemIndex];
					if (itemRequirement.Id != mmoItem.Id)
						continue;

					var currentCounts = questProgression.ItemCount;

					var currentCount = currentCounts[itemIndex];
					if (currentCount >= itemRequirement.Count)
						continue;

					currentCount = (byte) Mathf.Min(currentCount + count, itemRequirement.Count);
					currentCounts[itemIndex] = currentCount;

					var questId = questData.QuestId;
					if (CanCompleteQuest(questId))
						this.CompleteQuest(questId);

					var questProgress = new QuestProgressStructure(questId, itemIndex, currentCount);
					this.session.SendEvent(new QuestProgress { QuestProgressData = questProgress }, new MessageParameters { ChannelId = PeerSettings.QuestEventChannel });
					break;
				}
			}
		}

		/// <summary>
		/// Update quests for item removal
		/// </summary>
		void UpdateQuestsForItemRemoval(IMmoItem mmoItem, int count)
		{
			foreach (var entry in currentQuests)
			{
				var questProgression = entry.Value;
				if (questProgression.Status != QuestStatus.InProgress)
					continue;

				QuestData questData;
				if (!MmoWorld.Instance.ItemCache.TryGetQuestData(entry.Key, out questData))
				{
					Logger.ErrorFormat("[UpdateQuestsForTalk]: Quest (Id={0}) not found", entry.Key);
					continue;
				}

				if ((questData.Flags & QuestFlags.QUEST_FLAG_DELIVER) != QuestFlags.QUEST_FLAG_DELIVER)
					continue;

				var itemRequirements = questData.ItemRequirements;
				for (byte itemIndex = 0; itemIndex < itemRequirements.Length; itemIndex++)
				{
					var itemRequirement = itemRequirements[itemIndex];
					if (itemRequirement.Id != mmoItem.Id)
						continue;

					var currentCounts = questProgression.ItemCount;

					var currentCount = currentCounts[itemIndex];
					if (currentCount < 1)
						continue;

					currentCount = (byte)Mathf.Max(currentCount - count, 0);
					currentCounts[itemIndex] = currentCount;

					var questProgress = new QuestProgressStructure(questData.QuestId, itemIndex, currentCount);
					this.session.SendEvent(new QuestRegress { QuestProgressData = questProgress }, new MessageParameters { ChannelId = PeerSettings.QuestEventChannel });
					break;
				}
			}
		}

		/// <summary>
		/// Updates the current subscribers for quest status change
		/// </summary>
		void UpdateSubscribersForQuestStatusChange(short questId)
		{
			switch (GetQuestStatus(questId))
			{
				case QuestStatus.InProgress:
					{
						lock (subscribedGuids)
						{
							foreach (var guid in this.subscribedGuids)
							{
								switch ((ObjectType) guid.Type)
								{
									case ObjectType.Npc:
										{
											MmoObject mmoObject;
											if (!CurrentZone.ObjectCache.TryGetItem(guid, out mmoObject))
												continue;

											// if the quest giver also starts the quest the overhead status was already set
											// via begin quest so no need to reset it
											if (mmoObject.HaveCompletableQuest(questId) && !mmoObject.HaveStartableQuest(questId))
											{
												var questGiverStatus = new QuestGiverStatus
													{
														ObjectId = mmoObject.Guid,
														Status = (byte) this.GetQuestGiverQuestPickupStatus(mmoObject)
													};
												this.session.SendEvent(questGiverStatus, new MessageParameters {ChannelId = PeerSettings.QuestEventChannel});
												return;
											}

											if (HaveConversationFor(mmoObject))
												this.SendSetFlags(mmoObject, InterestFlags.Conversation);
										}
										break;

									case ObjectType.Gameobject:
										{
											MmoObject mmoObject;
											if (!CurrentZone.ObjectCache.TryGetItem(guid, out mmoObject))
												continue;

											QuestData questData;
											if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
											{
												Logger.ErrorFormat("[UpdateSubscribersForQuestStatusChange]: Quest (Id={0}) not found", questId);
												continue;
											}

											if ((questData.Flags & QuestFlags.QUEST_FLAG_DELIVER) != QuestFlags.QUEST_FLAG_DELIVER)
												continue;

											if (questData.ItemRequirements.Any(itemRequirement => mmoObject.GetLootFor(this).HasItem(itemRequirement.Id)))
												this.SendSetFlags(mmoObject, InterestFlags.Loot);
										}
										break;
								}
							}
						}
					}
					break;

				case QuestStatus.TurnIn:
					{
						lock (subscribedGuids)
						{
							foreach (var guid in this.subscribedGuids)
							{
								switch ((ObjectType) guid.Type)
								{
									case ObjectType.Npc:
										{
											MmoObject mmoObject;
											if (!CurrentZone.ObjectCache.TryGetItem(guid, out mmoObject))
												continue;

											if (mmoObject.HaveCompletableQuest(questId))
											{
												var questGiverStatus = new QuestGiverStatus
													{
														ObjectId = mmoObject.Guid,
														Status = (byte) QuestPickupStatus.TurnIn // turn-in has the highest precedence
													};
												this.session.SendEvent(questGiverStatus, new MessageParameters {ChannelId = PeerSettings.QuestEventChannel});
												return;
											}

											if (HaveConversationFor(mmoObject))
												this.SendSetFlags(mmoObject, InterestFlags.Conversation);
										}
										break;

									case ObjectType.Gameobject:
										{
											MmoObject mmoObject;
											if (!CurrentZone.ObjectCache.TryGetItem(guid, out mmoObject))
												continue;

											QuestData questData;
											if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
											{
												Logger.ErrorFormat("[UpdateSubscribersForQuestStatusChange]: Quest (Id={0}) not found", questId);
												continue;
											}

											if ((questData.Flags & QuestFlags.QUEST_FLAG_DELIVER) != QuestFlags.QUEST_FLAG_DELIVER)
												continue;

											if (questData.ItemRequirements.Any(itemRequirement => mmoObject.GetLootFor(this).HasItem(itemRequirement.Id)))
												this.SendSetFlags(mmoObject, InterestFlags.Loot);
										}
										break;
								}
							}
						}
					}
					break;

				case QuestStatus.Finished:
					{
						QuestData questData;
						if (!MmoWorld.Instance.ItemCache.TryGetQuestData(questId, out questData))
						{
							Logger.ErrorFormat("[UpdateSubscribersForQuestStatusChange]: Quest (Id={0}) not found", questId);
							return;
						}

						var nextQuest = questData.NextQuest;
						if (nextQuest <= 0)
							return;

						lock (subscribedGuids)
						{
							foreach (var guid in this.subscribedGuids)
							{
								if (guid.Type != (byte) ObjectType.Npc)
									continue;

								MmoObject mmoObject;
								if (!CurrentZone.ObjectCache.TryGetItem(guid, out mmoObject))
									continue;

								if (mmoObject.HaveStartableQuest(nextQuest))
								{
									var questGiverStatus = new QuestGiverStatus
										{
											ObjectId = mmoObject.Guid,
											Status = (byte) this.GetQuestGiverQuestPickupStatus(mmoObject)
										};
									this.session.SendEvent(questGiverStatus, new MessageParameters {ChannelId = PeerSettings.QuestEventChannel});
									return;
								}

								if (HaveConversationFor(mmoObject))
									this.SendSetFlags(mmoObject, InterestFlags.Conversation);
							}
						}
					}
					break;
			}
		}

		#endregion

		#region Loot System Implementation

		/// <summary>
		/// Returns false. <see cref="Player"/>s cannot have loot.
		/// </summary>
		public override bool HaveLootFor(Player player)
		{
			return false;
		}
		
		/// <summary>
		/// Notifies the client of the availability of loot from an <see cref="MmoObject"/>.
		/// </summary>
		public void NotifyLoot(MmoObject mmoObject)
		{
			this.SendSetFlags(mmoObject, InterestFlags.Loot);
		}

		/// <summary>
		/// Notify the client that the loot has been cleared
		/// </summary>
		public void NotifyLootClear(MmoObject mmoObject)
		{
			this.SendUnsetFlags(mmoObject, InterestFlags.Loot);
		}

		/// <summary>
		/// Notifies the client of the loot item take
		/// </summary>
		public void NotifyLootItemTakenFor(MmoObject mmoObject, int index)
		{
			if (index == 0)
			{
				// if the item was taken from the 0th index (remember at 0th index there is always gold) remove gold
				this.session.SendEvent(new LootGoldRemoved {ObjectId = mmoObject.Guid}, new MessageParameters {ChannelId = PeerSettings.LootEventChannel});
			}
			else
			{
				var lootItemRemoved = new LootItemRemoved
					{
						ObjectId = mmoObject.Guid,
						// negating 1 because the client doesnt count gold as an item which is only for the server
						Index = (byte) (index - 1)
					};
				this.session.SendEvent(lootItemRemoved, new MessageParameters {ChannelId = PeerSettings.LootEventChannel});
			}
		}

		#endregion

		#region Group System Implementation

		/// <summary>
		/// Gets the async event to listen for disconnects
		/// </summary>
		public AsyncEvent OnDisconnect
		{
			get
			{
				return this.session.OnDisconnect;
			}
		}

		/// <summary>
		/// Tells whether the <see cref="Player"/> is in group or not
		/// </summary>
		public bool InGroup()
		{
			return this.Group != null;
		}

		/// <summary>
		/// Gets the session
		/// </summary>
		IPeer IGroupMember.Peer
		{
			get
			{
				return this.session;
			}
		}

		/// <summary>
		/// Tells whether a(n) object with the <see cref="guid"/> is visible for the <see cref="IGroupMember"/> or not.
		/// </summary>
		bool IGroupMember.IsVisible(MmoGuid guid)
		{
			return this.HaveSubscriptionFor(guid);
		}

		/// <summary>
		/// Build builtProperties from update flags
		/// </summary>
		/// <returns></returns>
		Hashtable IGroupMember.BuildGroupMemberProperties()
		{
			var properties = new Hashtable();

			if ((MemberUpdateFlags & GroupMemberPropertyFlags.PROPERTY_FLAG_NAME) == GroupMemberPropertyFlags.PROPERTY_FLAG_NAME)
				properties.Add(GroupMemberProperty.Name, this.Name);

			if ((MemberUpdateFlags & GroupMemberPropertyFlags.PROPERTY_FLAG_LEVEL) == GroupMemberPropertyFlags.PROPERTY_FLAG_LEVEL)
				properties.Add(GroupMemberProperty.Level, this.Level);

			if ((MemberUpdateFlags & GroupMemberPropertyFlags.PROPERTY_FLAG_MAX_HP) == GroupMemberPropertyFlags.PROPERTY_FLAG_MAX_HP)
				properties.Add(GroupMemberProperty.MaxHp, this.MaximumHealth);

			if ((MemberUpdateFlags & GroupMemberPropertyFlags.PROPERTY_FLAG_CURR_HP) == GroupMemberPropertyFlags.PROPERTY_FLAG_CURR_HP)
				properties.Add(GroupMemberProperty.CurrHp, this.CurrentHealth);

			if ((MemberUpdateFlags & GroupMemberPropertyFlags.PROPERTY_FLAG_POSITION) == GroupMemberPropertyFlags.PROPERTY_FLAG_POSITION)
				properties.Add(GroupMemberProperty.Position, new[] {(int) this.Position.X, (int) this.Position.Z});

			if ((MemberUpdateFlags & GroupMemberPropertyFlags.PROPERTY_FLAG_STATUS) == GroupMemberPropertyFlags.PROPERTY_FLAG_STATUS)
				properties.Add(GroupMemberProperty.Status, IsDead() ? GroupMemberStatus.Dead : GroupMemberStatus.Online);

			return properties;
		}

		#endregion
	}
}