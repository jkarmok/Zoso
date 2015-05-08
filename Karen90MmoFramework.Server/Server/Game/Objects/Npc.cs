using System;
using System.Collections;
using System.Collections.Generic;

using Photon.SocketServer.Concurrency;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.AI;
using Karen90MmoFramework.Server.Game.Systems;
using Karen90MmoFramework.Server.Game.Messages;
using Karen90MmoFramework.Server.ServerToClient.Events;

namespace Karen90MmoFramework.Server.Game.Objects
{
	public class Npc : Character, IEngager, IMerchant
	{
		#region Constants and Fields

		private readonly NpcType npcType;
		private readonly SocialAlignment alignment;
		
		private ILootContainer lootContainer;

		private readonly short lootGroupId;
		private readonly short[] startableQuestIds;
		private readonly short[] completableQuestIds;
		private readonly short[] inventory;

		private readonly Dictionary<MmoGuid, float> threatTable;

		private Vector3 radarCenter;
		
		private readonly float radarEnterRadius;
		private readonly float radarExitRadius;

		private readonly Dictionary<Character, IDisposable> visibleCharacters;

		private NpcAI ai;

		private Bounds visibleArea;
		private IDisposable radarRegionsSubscription;

		private IDisposable gameUpdateSubscription;

		private float lastOrientation;
		private int lastSpeed;
		private MovementState lastMovementState;
		private MovementDirection lastDirection;
		private Vector3 uVelocity;

		private Vector3 iPosition;
		private Quaternion iRotation;

		private Vector3 destination;

		private int nextMovementPublishTime;
		private int nextRotationCheckTime;
		private int nextDestinationCheckTime;

		#endregion

		#region Properties

		/// <summary>
		/// Npc Type
		/// </summary>
		public NpcType NpcType
		{
			get
			{
				return this.npcType;
			}
		}

		/// <summary>
		/// Character's Alignment
		/// </summary>
		public SocialAlignment Alignment
		{
			get
			{
				return this.alignment;
			}
		}

		/// <summary>
		/// Gets the value of whether the <see cref="Character"/> is moving or not.
		/// </summary>
		public override bool IsMoving
		{
			get
			{
				return CurrentSpeed > 0;
			}
		}

		/// <summary>
		/// Gets the current aggro state
		/// </summary>
		public AggroState CurrentAggroState { get; private set; }

		/// <summary>
		/// Gets the current aggro target
		/// </summary>
		private Character CurrentFocus { get; set; }

		/// <summary>
		/// Gets the current movement state
		/// </summary>
		private MovementState CurrentMovementState { get; set; }

		/// <summary>
		/// Gets the current direction
		/// </summary>
		private MovementDirection CurrentDirection { get; set; }

		/// <summary>
		/// Gets the current speed
		/// </summary>
		private int CurrentSpeed { get; set; }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="Npc"/> class.
		/// </summary>
		public Npc(MmoZone zone, int objectId, short groupId, NpcType npcType, NpcData npcData)
			: base(zone, ObjectType.Npc, objectId, groupId, new Bounds(), npcData)
		{
			this.npcType = npcType;
			this.alignment = (SocialAlignment) npcData.Alignment;

			this.lootGroupId = npcData.LootGroupId;
			this.startableQuestIds = npcData.StartQuests;
			this.completableQuestIds = npcData.CompleteQuests;

			this.BuildProperties(Properties, PropertyFlags.NpcAll);
			this.Revision = this.Properties.Count;
			if (Properties.Count > 0)
				this.Flags |= MmoObjectFlags.HasProperties;

			this.SetBaseStat(Stats.Health, (short) npcData.MaxHealth);
			this.SetHealth(MaximumHealth);
			this.SetBaseStat(Stats.Power, (short) npcData.MaxMana);
			this.SetPower(MaximumPower);

			this.threatTable = new Dictionary<MmoGuid, float>();
			this.CurrentAggroState = AggroState.Idle;

			this.visibleCharacters = new Dictionary<Character, IDisposable>();

			this.radarEnterRadius = ServerGameSettings.BASE_AGGRO_RADIUS;
			this.radarExitRadius = ServerGameSettings.BASE_AGGRO_DROP_RADIUS;

			this.ai = npcType == NpcType.Enemy ? (NpcAI) new NpcAggressiveAI(this) : new NpcDefensiveAI(this);
			this.inventory = npcData.Items;
		}

		#endregion

		#region IDisposable Implementation

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				this.DisposeGameSubscriptions();
				this.DisposeRadarSubscriptions();
			}
		}

		#endregion

		#region Implementation of IMerchant

		/// <summary>
		/// Gets the merchant inventory
		/// </summary>
		IEnumerable<short> IMerchant.Inventory
		{
			get
			{
				return this.inventory;
			}
		}

		#endregion

		#region Object Identification

		/// <summary>
		/// Returns if the <see cref="Npc"/> is of type <see cref="Karen90MmoFramework.Game.NpcType.Merchant"/>.
		/// </summary>
		public sealed override bool IsMerchant()
		{
			return this.npcType == NpcType.Merchant;
		}

		/// <summary>
		/// Returns if the <see cref="Npc"/> is of type <see cref="Karen90MmoFramework.Game.NpcType.Civilian"/>.
		/// </summary>
		public sealed override bool IsCivilian()
		{
			return this.npcType == NpcType.Civilian;
		}

		/// <summary>
		/// Returns if the <see cref="Npc"/> is of type <see cref="Karen90MmoFramework.Game.NpcType.Guard"/>.
		/// </summary>
		public sealed override bool IsGuard()
		{
			return this.npcType == NpcType.Guard;
		}

		/// <summary>
		/// Returns if the <see cref="Npc"/> is of type <see cref="Karen90MmoFramework.Game.NpcType.Trainer"/>.
		/// </summary>
		public sealed override bool IsTrainer()
		{
			return this.npcType == NpcType.Trainer;
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
				// case PropertyCode.Name:
				// case PropertyCode.Species:
				// case PropertyCode.Class:
				// case PropertyCode.NpcType:

				// public builtProperties
				case PropertyCode.Level:
				case PropertyCode.MaxHp:
				case PropertyCode.CurrHp:
				case PropertyCode.UnitState:
				case PropertyCode.Alignment:
					break; // break

				// private builtProperties
				case PropertyCode.MaxPow:
				case PropertyCode.CurrPow:
					return; // return

				default:
					Logger.WarnFormat("PublishProperty.UnlistedProperty={0}", propertyCode);
					break; // break
			}

			// publish builtProperties on the event channel
			base.PublishProperty(propertyCode, value);
		}

		/// <summary>
		/// Adds or sets builtProperties into the <paramref name="builtProperties"/> based on <paramref name="flags"/>.
		/// </summary>
		/// <remarks>
		/// This does not generate a new <see cref="Hashtable"/> because this will be called extensively based on how much a(n) <see cref="Npc"/>s
		/// property changes. Generating a new <see cref="Hashtable"/> everytime would be expensive.
		/// </remarks>
		public override sealed void BuildProperties(Hashtable builtProperties, PropertyFlags flags)
		{
			if (builtProperties == null)
				return;

			if ((flags & PropertyFlags.UnitState) == PropertyFlags.UnitState)
				builtProperties[PropertyCode.UnitState] = (byte) this.State;
			 
			if ((flags & PropertyFlags.Level) == PropertyFlags.Level)
				builtProperties[PropertyCode.Level] = this.Level;

			if ((flags & PropertyFlags.MaxHp) == PropertyFlags.MaxHp)
				builtProperties[PropertyCode.MaxHp] = this.MaximumHealth;

			if ((flags & PropertyFlags.CurrHp) == PropertyFlags.CurrHp)
				builtProperties[PropertyCode.CurrHp] = this.CurrentHealth;
		}

		#endregion

		#region Respawn System Implementation

		/// <summary>
		/// Called right before spawning.
		/// Calls <see cref="Character.OnAwake"/>.
		/// Sets <see cref="CurrentAggroState"/> to <see cref="AggroState.Idle"/>.
		/// </summary>
		protected override void OnAwake()
		{
			base.OnAwake();
			this.CurrentAggroState = AggroState.Idle;
		}

		/// <summary>
		/// Called when the object is spawned.
		/// Calls <see cref="Character.OnSpawn"/>.
		/// Initializes the radar.
		/// Resets the movement system.
		/// </summary>
		protected override void OnSpawn()
		{
			base.OnSpawn();

			this.iPosition = this.Position;
			this.iRotation = this.Rotation;

			this.SetRadarCenter(iPosition);

			// TODO: Setup wandering

			this.CurrentSpeed = this.lastSpeed = 0;
			this.CurrentDirection = this.lastDirection = MovementDirection.None;
			this.CurrentMovementState = this.lastMovementState = MovementState.Idle;
			this.lastOrientation = 0;
			this.uVelocity = Vector3.Zero;
			this.destination = Vector3.Zero;
#if MMO_DEBUG
			Logger.DebugFormat("Id: {0}. Type: Npc ({1}). Name: {2}. Level: {3}. Alignment: {4}. Position: {5}", this.Guid,
			                   this.NpcType, this.Name, this.Level, this.Alignment, this.Position);
#endif
		}

		/// <summary>
		/// Called when the instance has been removed from the world.
		/// Calls <see cref="Character.OnSleep"/>.
		/// Clears loot.
		/// </summary>
		protected override void OnSleep()
		{
			if (lootContainer != null)
				lootContainer.Clear();
			base.OnSleep();
		}

		#endregion

		#region Quest System Implementation

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has any startable quests.
		/// </summary>
		public sealed override bool HaveStartableQuests()
		{
			return startableQuestIds != null && this.startableQuestIds.Length > 0;
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has any completable quests.
		/// </summary>
		public sealed override bool HaveCompletableQuests()
		{
			return completableQuestIds != null && this.completableQuestIds.Length > 0;
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has any quests.
		/// </summary>
		public sealed override bool HaveQuests()
		{
			return (startableQuestIds != null && this.startableQuestIds.Length > 0) ||
				(completableQuestIds != null && this.completableQuestIds.Length > 0);
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has a particular start quest.
		/// </summary>
		public sealed override bool HaveStartableQuest(short questId)
		{
			return startableQuestIds != null && Array.FindIndex(startableQuestIds, id => id == questId) != -1;
		}

		/// <summary>
		/// Determines whether the <see cref="MmoObject"/> has a particular complete quest.
		/// </summary>
		public sealed override bool HaveCompletableQuest(short questId)
		{
			return completableQuestIds != null && Array.FindIndex(completableQuestIds, id => id == questId) != -1;
		}

		/// <summary>
		/// Gets all start quest ids
		/// </summary>
		public sealed override short[] GetStartableQuests()
		{
			return this.startableQuestIds;
		}

		/// <summary>
		/// Gets all complete quest ids
		/// </summary>
		public sealed override short[] GetCompletableQuests()
		{
			return this.completableQuestIds;
		}

		#endregion

		#region Loot System Implementation

		/// <summary>
		/// Tells whether this <see cref="Npc"/> has loot for a(n) <see cref="Player"/> or not.
		/// </summary>
		public override bool HaveLootFor(Player player)
		{
			return lootContainer != null && lootContainer.HasLootFor(player);
		}

		/// <summary>
		/// Gets the <see cref="ILoot"/> for a(n) <see cref="Player"/>.
		/// </summary>
		public override ILoot GetLootFor(Player player)
		{
			return this.lootContainer != null ? this.lootContainer.GetLootFor(player) : EmptyLootContainer.EmptyLoot;
		}

		#endregion

		#region Aggro System Implementation

		/// <summary>
		/// Adds threat
		/// </summary>
		public override sealed void AddThreat(MmoObject cUnit, float amount, Spell spell)
		{
			if (IsDead())
				return;

			if(amount <= 0)
				return;
			
			switch ((ObjectType)cUnit.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					{
						var id = cUnit.Guid;

						float oldThreat;
						if (threatTable.TryGetValue(id, out oldThreat) == false)
						{
							// this will not happen in regular scenarios
							// will only happen when damaging npc without entering its aggro range
							// mostly when debugging or via gm commands
							if (ManualAggro(cUnit as Character) == false)
								return;

							// looking up again to make sure that the manual aggro worked
							if (threatTable.TryGetValue(id, out oldThreat) == false)
								return;
						}

						// dont use += (two lookups)
						// making sure to clamp the threat between 1 to max
						// remember, threat of 1 means that the unit is in the npc's visible range
						this.threatTable[id] = Mathf.Clamp(oldThreat + amount, 1, int.MaxValue);
					}
					break;
			}
		}

		/// <summary>
		/// Removes threat
		/// </summary>
		public override sealed void RemoveThreat(MmoObject cUnit, float amount, Spell spell)
		{
			if (IsDead())
				return;

			if(amount <= 0)
				return;

			switch ((ObjectType)cUnit.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					{
						var id = cUnit.Guid;

						float oldThreat;
						if (threatTable.TryGetValue(id, out oldThreat) == false)
							return;

						// dont use += (two lookups)
						// making sure to clamp the threat between 1 to max
						// remember, threat of 1 means that the unit is in the npc's visible range
						this.threatTable[id] = Mathf.Clamp(oldThreat - amount, 1, int.MaxValue);
					}
					break;
			}
		}

		/// <summary>
		/// Filters the <see cref="mmoObject"/> for combat eligibility and takes appropriate action
		/// </summary>
		void IEngager.ProcessThreat(MmoObject mmoObject)
		{
			this.DoProcessThreat(mmoObject);
		}

		void DoProcessThreat(MmoObject mmoObject)
		{
			// this will make sure that we only subscribe world objects
			if (CanRadarSubscribe(mmoObject) == false)
				return;

			var character = mmoObject as Character;
			if (character != null)
			{
				if (!visibleCharacters.ContainsKey(character))
				{
					// subscribing character
					var subscription = mmoObject.PositionUpdateChannel.SubscribeToLast(this.CurrentZone.PrimaryFiber, this.Radar_OnCharacterPosition_AggroEnterCheck, ServerGameSettings.AGGRO_ENTER_CHECK_INTERVAL_MS);
					this.visibleCharacters.Add(character, subscription);
				}
			}
		}

		/// <summary>
		/// Tells whether a <see cref="MmoObject"/> can be subscribed or not.
		/// </summary>
		bool CanRadarSubscribe(MmoObject mmoObject)
		{
			var objectType = (ObjectType) mmoObject.Guid.Type;
			return mmoObject != this && objectType == ObjectType.Npc || objectType == ObjectType.Player;
		}

		/// <summary>
		/// Sets the radar center
		/// </summary>
		void SetRadarCenter(Vector3 center)
		{
			if (radarRegionsSubscription != null)
			{
				radarRegionsSubscription.Dispose();
				radarRegionsSubscription = null;
			}

			if (visibleCharacters.Count > 0)
			{
				foreach (var subscription in visibleCharacters.Values)
				{
					subscription.Dispose();
				}

				visibleCharacters.Clear();
			}

			this.radarCenter = center;
			this.visibleArea = new Bounds
				{
					Min = new Vector3 { X = this.radarCenter.X - radarEnterRadius, Z = this.radarCenter.Z - radarEnterRadius },
					Max = new Vector3 { X = this.radarCenter.X + radarEnterRadius, Z = this.radarCenter.Z + radarEnterRadius }
				};

			this.visibleArea = this.CurrentZone.GetRegionAlignedBoundingBox(this.visibleArea);
			var regions = this.CurrentZone.GetRegions(this.visibleArea);

			var subscriptions = new IDisposable[regions.Count];
			var index = 0;

			foreach (var region in regions)
			{
				subscriptions[index++] = region.Subscribe(this.CurrentZone.PrimaryFiber, this.Radar_OnRegionMessage);
			}

			this.radarRegionsSubscription = new UnsubscriberCollection(subscriptions);

			this.CurrentZone.PrimaryFiber.Enqueue(() =>
				{
					var snapShotRequest = new AggroSnapshotRequest(this);
					foreach (var region in regions)
					{
						region.Publish(snapShotRequest);
					}
				});
		}

		/// <summary>
		/// Manually aggroes a <see cref="Character"/>.
		/// </summary>
		bool ManualAggro(Character target)
		{
			if (!IsHostileTo(target))
				return false;

			if (Vector3.SqrDistance(target.Position, radarCenter) >= radarExitRadius * radarExitRadius)
				return false;

			IDisposable oldSubscription;
			if (!visibleCharacters.TryGetValue(target, out oldSubscription))
			{
				this.visibleCharacters.Add(target, null);
			}
			else
			{
				oldSubscription.Dispose();
			}

			var positionSubscription = target.PositionUpdateChannel.SubscribeToLast(this.CurrentZone.PrimaryFiber, Radar_OnCharacterPosition_AggroExitCheck, ServerGameSettings.AGGRO_EXIT_CHECK_INTERVAL_MS);
			var disposedSubscription = target.DisposeChannel.Subscribe(this.CurrentZone.PrimaryFiber, this.Radar_OnCharacterDisposed);
			this.visibleCharacters[target] = new UnsubscriberCollection(positionSubscription, disposedSubscription);

			// entering aggro range
			this.OnEnterRadarRange(target);

			return true;
		}

		/// <summary>
		/// Called when a <see cref="Character"/> enters the aggro range.
		/// </summary>
		void OnEnterRadarRange(Character character)
		{
			if (!IsHostileTo(character))
				return;

			if (threatTable.ContainsKey(character.Guid))
				return;

			this.threatTable.Add(character.Guid, ServerGameSettings.THREAT_FOR_AGGRO_ENTER);
			this.ai.OnCharacterEnterRange(character);
		}

		/// <summary>
		/// Called when a <see cref="Character"/> exits the aggro range.
		/// </summary>
		void OnExitRadarRange(Character character)
		{
			if (threatTable.Remove(character.Guid))
			{
				this.ai.OnCharacterExitRange(character);
			}
		}

		/// <summary>
		/// Disposes all radar subscriptions
		/// </summary>
		void DisposeRadarSubscriptions()
		{
			this.radarRegionsSubscription.Dispose();
			foreach (var entry in this.visibleCharacters)
			{
				entry.Value.Dispose();
			}

			this.visibleCharacters.Clear();
		}

		void Radar_OnRegionMessage(RegionMessage message)
		{
			var snapShot = message as MmoObjectSnapshot;
			if (snapShot != null)
				this.DoProcessThreat(snapShot.Source);
		}

		void Radar_OnCharacterPosition_AggroEnterCheck(MmoObjectPositionMessage message)
		{
			var squaredDistance = Vector3.SqrDistance(message.Position, radarCenter);
			if (squaredDistance <= radarEnterRadius * radarEnterRadius)
			{
				var character = message.Source as Character;
				if(character == null)
					return;

				IDisposable oldSubscription;
				if (visibleCharacters.TryGetValue(character, out oldSubscription))
				{
					oldSubscription.Dispose();
				}
				else
				{
					this.visibleCharacters.Add(character, null);
				}

				// aggro exit checker
				var positionSubscription = character.PositionUpdateChannel.SubscribeToLast(this.CurrentZone.PrimaryFiber, Radar_OnCharacterPosition_AggroExitCheck, ServerGameSettings.AGGRO_EXIT_CHECK_INTERVAL_MS);
				var disposedSubscription = character.DisposeChannel.Subscribe(this.CurrentZone.PrimaryFiber, this.Radar_OnCharacterDisposed);
				this.visibleCharacters[character] = new UnsubscriberCollection(positionSubscription, disposedSubscription);

				// entering aggro range
				this.OnEnterRadarRange(character);
			}
			else if (squaredDistance > radarExitRadius * radarExitRadius)
			{
				// unsubscribing character
				var character = message.Source as Character;
				if (character == null)
					return;

				this.visibleCharacters[character].Dispose();
				this.visibleCharacters.Remove(character);
			}
		}

		void Radar_OnCharacterPosition_AggroExitCheck(MmoObjectPositionMessage message)
		{
			var squaredDistance = Vector3.SqrDistance(message.Position, radarCenter);
			if (squaredDistance >= radarExitRadius * radarExitRadius)
			{
				var character = message.Source as Character;
				if(character == null)
					return;

				IDisposable oldSubscription;
				if (visibleCharacters.TryGetValue(character, out oldSubscription))
					oldSubscription.Dispose();

				if (visibleArea.Contains(character.Position))
				{
					// aggro enter checker
					var subscription = character.PositionUpdateChannel.SubscribeToLast(this.CurrentZone.PrimaryFiber, this.Radar_OnCharacterPosition_AggroEnterCheck, ServerGameSettings.AGGRO_ENTER_CHECK_INTERVAL_MS);
					this.visibleCharacters[character] = subscription;
				}
				else
				{
					// unsubscribing character
					this.visibleCharacters.Remove(character);
				}

				this.OnExitRadarRange(character);
			}
		}

		void Radar_OnCharacterDisposed(MmoObjectDisposedMessage message)
		{
			var character = message.Source as Character;
			if (character == null)
				return;

			this.visibleCharacters[character].Dispose();
			this.visibleCharacters.Remove(character);

			// exiting aggro range
			this.OnExitRadarRange(character);
		}

		#endregion

		#region Combat System Implementation

		/// <summary>
		/// Tells whether a <see cref="Character"/> is hostile to this <see cref="Character"/> or not.
		/// </summary>
		public override bool IsHostileTo(Character character)
		{
			return (character.Guid.Type == (byte) ObjectType.Player) && this.NpcType == NpcType.Enemy;
		}

		/// <summary>
		/// Gets the current focus
		/// </summary>
		/// <returns></returns>
		public Character GetCurrentFocus()
		{
			return this.CurrentFocus;
		}

		/// <summary>
		/// Tells whether a <see cref="Character"/> is within attack range or not
		/// </summary>
		public bool IsWithinAttackRange(Character victim)
		{
			return Vector3.SqrDistance(victim.Position, Position) <= 6; // ~2.5
		}

		/// <summary>
		/// Tells whether we can attack a <see cref="Character"/> or not.
		/// </summary>
		public bool CanAttackTarget(Character victim)
		{
			return this.IsHostileTo(victim);
		}

		/// <summary>
		/// Begins the combat
		/// </summary>
		public void BeginCombat(Character victim)
		{
			this.BeginChase(victim);
		}

		/// <summary>
		/// Ends the combat
		/// </summary>
		public void EndCombat()
		{
			this.EndChase();
		}

		// TODO: Temporary
		private const int CombatDelayInMs = 1500; // 1.5 sec
		private int nextAttackTime = 0;

		/// <summary>
		/// Called when the combat begins
		/// </summary>
		public void DoCombat()
		{
			if (nextAttackTime < CurrentZone.World.GlobalTime)
			{
				var victim = this.CurrentFocus;
				if (victim.IsDead())
					return;

				victim.Damage(this, 8, null);

				this.nextAttackTime = CurrentZone.World.GlobalTime + CombatDelayInMs;
			}
		}

		/// <summary>
		/// Called when this <see cref="Character"/> is killed. Remember the <paramref name="killer"/> could be <value>this</value>.
		/// </summary>
		protected override void OnDeath(MmoObject killer)
		{
			base.OnDeath(killer);
			if (IsMoving)
			{
				this.ResetMovement();
				this.PublishStopEvent();
			}

			this.DisposeRadarSubscriptions();

			this.CurrentAggroState = AggroState.Idle;
			this.CurrentFocus = null;

			RandomLootContainer randomLootContainer = null;

			ICollection<MmoGuid> skipPlayers = null;
			foreach (var pair in threatTable)
			{
				// only players can be rewarded for now
				var pGuid = pair.Key;
				if (pGuid.Type != (byte)ObjectType.Player)
					continue;
				// reward only if the threat is greater than this threshold
				if (pair.Value <= ServerGameSettings.THREAT_THRESHOLD_FOR_REWARD)
					continue;

				if (skipPlayers != null)
				{
					// making sure the player gets only one loot generation attempt
					if (skipPlayers.Contains(pGuid))
						continue;
					skipPlayers.Add(pGuid);
				}

				WorldSession session;
				if (MmoWorld.Instance.SessionCache.TryGetSessionByPlayerGuid(pGuid.Id, out session))
				{
					// generate loot, only if one of the killer is a player
					if (randomLootContainer == null)
						randomLootContainer = new RandomLootContainer(this, this.lootGroupId, (itemId, looter) => true);
					// create the collection only if one killer is a player
					if (skipPlayers == null)
						skipPlayers = this.threatTable.Count > 10 ? (ICollection<MmoGuid>) new HashSet<MmoGuid>() : new List<MmoGuid>();

					var player = session.Player;
					// player is in a group
					if (player.InGroup())
					{
						var members = player.Group.GetActiveMembers();
						foreach (var member in members)
						{
							// making sure the member gets only one loot generation attempt
							var mGuid = member.Guid;
							if (skipPlayers.Contains(mGuid))
								continue;

							skipPlayers.Add(mGuid);
							// finding the player
							WorldSession memberSession;
							if (MmoWorld.Instance.SessionCache.TryGetSessionByPlayerGuid(mGuid.Id, out memberSession))
								continue;
							// checking to see if the player is within the range of the member
							var memberPlayer = memberSession.Player;
							if (!player.HaveSubscriptionFor(memberPlayer.Guid) && player != memberPlayer)
								continue;

							randomLootContainer.GenerateLootFor(memberPlayer);
						}
					}
					// always generate loot for the player
					randomLootContainer.GenerateLootFor(player);
					player.RewardKill(this);
				}
			}

			this.threatTable.Clear();

			var spawnDelay = ServerGameSettings.BASE_NPC_RESPAWN_TIME_MS;
			if (randomLootContainer != null)
			{
				spawnDelay = ServerGameSettings.BASE_NPC_WAIT_TIME_UNTIL_LOOTED_MS;
				// set the loot
				lootContainer = randomLootContainer;
				// notify all the looters
				randomLootContainer.NotifyLooters();
			}

			this.QueueRespawn(spawnDelay, ServerGameSettings.BASE_NPC_RESPAWN_TIME_MS, this.iPosition, this.iRotation);
		}

		#endregion

		#region Movement System Implementation

		/// <summary>
		/// Begins chasing the aggroed <see cref="Character"/>.
		/// </summary>
		private void BeginChase(Character character)
		{
			// TODO: Check for current movement state (for wandering and stuff) and update accordingly

			if (gameUpdateSubscription != null)
				return;

			this.CurrentSpeed = GlobalGameSettings.NPC_FORWARD_SPEED_MAX;
			this.CurrentDirection = MovementDirection.Forward;
			this.CurrentMovementState = MovementState.Running;
			this.uVelocity = Vector3.Forward;
			this.CurrentFocus = character;

			this.destination = this.CurrentFocus.Position;
			this.CurrentAggroState = AggroState.Chasing;

			this.gameUpdateSubscription = this.CreateUpdateEvent(this.GameUpdate, ServerGameSettings.GAME_UPDATE_INTERVAL);
		}

		/// <summary>
		/// Ends the aggro movement and returns to original state
		/// </summary>
		private void EndChase()
		{
			this.destination = this.iPosition;
			this.CurrentAggroState = AggroState.Returning;
		}

		/// <summary>
		/// Stops chasing the aggro character
		/// </summary>
		private void ResetMovement()
		{
			this.DisposeGameSubscriptions();

			this.CurrentSpeed = this.lastSpeed = 0;
			this.CurrentDirection = this.lastDirection = MovementDirection.None;
			this.CurrentMovementState = this.lastMovementState = MovementState.Idle;
			this.uVelocity = Vector3.Zero;
			this.CurrentFocus = null;

			this.destination = Vector3.Zero;
			this.CurrentAggroState = AggroState.Idle;

			// TODO: Revert state to wandering
		}

		/// <summary>
		/// Handles all game actions
		/// </summary>
		private void GameUpdate(int deltaTime)
		{
			var deltaTimeInSec = deltaTime * .001f;

			this.UpdateMovement(deltaTimeInSec);
			this.ai.Update(deltaTime);
		}

		/// <summary>
		/// Updates movement
		/// </summary>
		/// <param name="deltaTime"></param>
		private void UpdateMovement(float deltaTime)
		{
			switch (CurrentAggroState)
			{
				case AggroState.Chasing:
					{
						if (nextDestinationCheckTime <= CurrentZone.World.GlobalTime)
						{
							this.destination = this.CurrentFocus.Position;
							this.nextDestinationCheckTime = CurrentZone.World.GlobalTime + ServerGameSettings.NPC_DESTINATION_CHECK_INTERVAL;
						}

						// updating orientation early to avoid client prediction errors
						this.UpdateRotation();

						var sqrDistance = Vector3.SqrDistance(destination, Position);
						if (sqrDistance <= 6) // ~2.5
						{
							// this will make sure stop event is only sent once after each stop
							if (lastSpeed > 0)
							{
								this.PublishStopEvent();

								this.lastSpeed = 0; /* m */
								this.lastMovementState = MovementState.Idle;
							}

							//this.OnReachedTarget();
						}
						else
						{
							this.Move(deltaTime);
						}
					}
					break;

				case AggroState.Returning:
					{
						if (nextDestinationCheckTime <= CurrentZone.World.GlobalTime)
						{
							this.destination = this.iPosition;
							this.nextDestinationCheckTime = CurrentZone.World.GlobalTime + ServerGameSettings.NPC_DESTINATION_CHECK_INTERVAL;
						}

						// updating orientation early to avoid client prediction errors
						this.UpdateRotation();

						var sqrDistance = Vector3.SqrDistance(destination, Position);
						if (sqrDistance <= 4) // 2
						{
							this.PublishStopEvent();

							if (CurrentHealth < MaximumHealth)
								this.SetHealth(MaximumHealth, true);

							if (CurrentPower < MaximumPower)
								this.SetPower(MaximumPower, true);

							this.ResetMovement();
						}
						else
						{
							this.Move(deltaTime);
						}
					}
					break;
			}
		}

		/// <summary>
		/// Moves the <see cref="Npc"/>'s position towards the destination
		/// </summary>
		private void Move(float deltaTime)
		{
			// direction is based on the velocity since the npc can move in any direction based on AI
			var direction = Vector3.Transform(uVelocity, this.Transform.Rotation).Normalize();
			// note: deltaTime is in seconds
			var newPosition = this.Position + direction * this.CurrentSpeed * deltaTime;
			// clamping to avoid walking off the terrain
			newPosition = this.CurrentZone.Bounds.Clamp(newPosition);
			newPosition.Y = this.CurrentZone.GetHeight(newPosition.X, newPosition.Z);
			this.Transform.Position = newPosition;
			
			var speedChanged = (lastSpeed != this.CurrentSpeed);
			var directionChanged = (lastDirection != this.CurrentDirection);
			var stateChanged = (lastMovementState != this.CurrentMovementState);

			if (speedChanged || stateChanged || directionChanged)
			{
				var movementEvent = new ObjectMovement
					{
						ObjectId = this.Guid,
					};

				if (speedChanged)
				{
					movementEvent.Speed = (byte)this.CurrentSpeed;
					this.lastSpeed = this.CurrentSpeed;
				}

				var yRotation = this.Rotation.EularAngles().Y;
				if (Math.Abs(lastOrientation - yRotation) > 0.1f)
				{
					movementEvent.Orientation = yRotation;
					this.lastOrientation = yRotation;
				}

				if (directionChanged)
				{
					movementEvent.Direction = (byte)this.CurrentDirection;
					this.lastDirection = this.CurrentDirection;
				}

				if (stateChanged)
				{
					movementEvent.State = (byte)this.CurrentMovementState;
					this.lastMovementState = this.CurrentMovementState;
				}

				var message = new MmoObjectEventMessage(this, movementEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
				this.EventChannel.Publish(message);
			}

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

		private void UpdateRotation()
		{
			// rotate the npc to face the destination (x, z)
			var direction = destination - Position;
			// setting y to 0 to avoid changing the pitch
			direction.Y = 0;
			this.Transform.Rotation = Quaternion.FromToRotation(Vector3.Forward, direction);

			if (nextRotationCheckTime <= CurrentZone.World.GlobalTime)
			{
				var yRotation = this.Rotation.EularAngles().Y;
				if (Math.Abs(lastOrientation - yRotation) > 0.1f)
				{
					var rotationEvent = new ObjectTransform
						{
							ObjectId = this.Guid,
							Orientation = yRotation
						};

					var message = new MmoObjectEventMessage(this, rotationEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
					this.EventChannel.Publish(message);
					this.lastOrientation = yRotation;
				}

				this.nextRotationCheckTime = CurrentZone.World.GlobalTime + ServerGameSettings.OBJECT_ROTATION_CHECK_INTERVAL;
			}
		}

		private void DisposeGameSubscriptions()
		{
			if (gameUpdateSubscription != null)
			{
				this.gameUpdateSubscription.Dispose();
				this.gameUpdateSubscription = null;
			}
		}

		/// <summary>
		/// Publishes a movement stop event
		/// </summary>
		private void PublishStopEvent()
		{
			var movementEvent = new ObjectMovement
				{
					ObjectId = this.Guid,
					Speed = 0,
					State = (byte) MovementState.Idle
				};

			var message = new MmoObjectEventMessage(this, movementEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
			this.EventChannel.Publish(message);
		}

		/// <summary>
		/// Sends a movement update to a <see cref="Player"/>.
		/// </summary>
		public override sealed void SendMovementUpdateTo(Player player)
		{
			var movementEvent = new ObjectMovement
				{
					ObjectId = this.Guid,
					Speed = (byte) this.CurrentSpeed,
					Orientation = this.Rotation.EularAngles().Y,
					Direction = (byte) this.CurrentDirection,
					State = (byte) this.CurrentMovementState
				};

			player.Session.SendEvent(movementEvent, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
		}

		#endregion
	}
}
