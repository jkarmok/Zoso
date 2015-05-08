using System.Collections;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Server.Game.Objects
{
	public class Gameobject : MmoObject
	{
		#region Constants and Fields

		private readonly GameObjectType goType;

		private readonly short lootGroupId;
		private readonly LootRestriction lootRestriction;

		/// <summary>
		/// the loot container
		/// </summary>
		private ILootContainer lootContainer;
		
		/// <summary>
		/// the loot generation count
		/// </summary>
		private uint lootGeneration;

		/// <summary>
		/// initial position
		/// </summary>
		private Vector3 iPosition;

		/// <summary>
		/// initial rotation
		/// </summary>
		private Quaternion iRotation;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the game object type
		/// </summary>
		public GameObjectType GoType
		{
			get
			{
				return this.goType;
			}
		}

		/// <summary>
		/// Gets the loot restrictions
		/// </summary>
		public LootRestriction LootRestriction
		{
			get
			{
				return this.lootRestriction;
			}
		}

		/// <summary>
		/// Returns false. <see cref="Gameobject"/>s will not move.
		/// </summary>
		public override sealed bool IsMoving
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="Gameobject"/> class.
		/// </summary>
		public Gameobject(MmoZone zone, int objectId, GameObjectType goType, short familyId, GameObjectData goInfo)
			: base(zone, ObjectType.Gameobject, objectId, familyId, new Bounds(), null)
		{
			this.goType = goType;
			this.lootGroupId = goInfo.LootGroupId;

			this.BuildProperties(Properties, PropertyFlags.GameobjectAll);
			this.Revision = this.Properties.Count;
			if (Properties.Count > 0)
				this.Flags |= MmoObjectFlags.HasProperties;

			switch (goType)
			{
				case GameObjectType.Chest:
					// chest can be looted by a single person
					this.lootRestriction = LootRestriction.SingleLooter;
					break;

				case GameObjectType.Plant:
				case GameObjectType.Vein:
					// plants and veins can be looted by multiple
					this.lootRestriction = LootRestriction.MultipleLooter;
					break;

				default:
					this.lootRestriction = LootRestriction.None;
					break;
			}

			this.lootGeneration = 0;
		}

		#endregion

		#region Object Identification

		/// <summary>
		/// Returns false
		/// </summary>
		public sealed override bool IsMerchant()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		public sealed override bool IsCivilian()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		public sealed override bool IsGuard()
		{
			return false;
		}

		/// <summary>
		/// Returns false
		/// </summary>
		public sealed override bool IsTrainer()
		{
			return false;
		}

		/// <summary>
		/// Returns if the <see cref="Gameobject"/> is of type <see cref="GameObjectType.Plant"/>.
		/// </summary>
		public sealed override bool IsPlant()
		{
			return this.goType == GameObjectType.Plant;
		}

		/// <summary>
		/// Returns if the <see cref="Gameobject"/> is of type <see cref="GameObjectType.Vein"/>.
		/// </summary>
		public sealed override bool IsVein()
		{
			return this.goType == GameObjectType.Vein;
		}

		/// <summary>
		/// Returns if the <see cref="Gameobject"/> is is of type <see cref="GameObjectType.Item"/>.
		/// </summary>
		public sealed override bool IsItem()
		{
			return this.goType == GameObjectType.Item;
		}

		/// <summary>
		/// Returns if the <see cref="Gameobject"/> is of type <see cref="GameObjectType.Chest"/>.
		/// </summary>
		public sealed override bool IsChest()
		{
			return this.goType == GameObjectType.Chest;
		}

		#endregion

		#region Respawn System Implementation

		/// <summary>
		/// Called right before spawning. Responsible for creating loot holders for any lootables
		/// </summary>
		protected override void OnAwake()
		{
			switch (goType)
			{
				case GameObjectType.Plant:
				case GameObjectType.Vein:
					{
						this.lootContainer = new FixedLootContainer(this, lootGroupId, (itemId, looter) => true, null, player =>
							{
								this.HideFromPlayer(player.Guid);
								this.World.PrimaryFiber.Schedule(() => this.UnhideFromPlayer(player.Guid), ServerGameSettings.GATHER_RESPAWN_TIME_MS);
							});
						this.lootGeneration = 0;
					}
					break;

				case GameObjectType.Chest:
					{
						this.lootContainer = new RandomLootContainer(this, lootGroupId, (itemId, looter) => true);
						this.lootGeneration = 0;
					}
					break;
			}
		}

		/// <summary>
		/// Called when the object is spawned
		/// </summary>
		protected override void OnSpawn()
		{
			this.iPosition = this.Position;
			this.iRotation = this.Rotation;
#if MMO_DEBUG
			Logger.DebugFormat("Id: {0}. Type: GameObject ({1}). Position: {2}", this.Guid, this.GoType, this.Position);
#endif
		}

		#endregion

		#region Property System Implementation

		/// <summary>
		/// Does nothing for now
		/// </summary>
		public override sealed void BuildProperties(Hashtable builtProperties, PropertyFlags flags)
		{
		}

		#endregion

		#region Movement System Implementation

		/// <summary>
		/// Does nothing
		/// </summary>
		public override void SendMovementUpdateTo(Player player)
		{
		}

		#endregion

		#region Interaction System Implementation

		/// <summary>
		/// Determine whether a(n) <see cref="Player"/> can use this <see cref="Gameobject"/> or not.
		/// </summary>
		public virtual bool CanUse(Player player)
		{
			switch (goType)
			{
				case GameObjectType.Chest:
					if (lootRestriction == LootRestriction.SingleLooter)
						return lootGeneration == 0;
					return true;
			}
			return false;
		}

		/// <summary>
		/// Uses the <see cref="Gameobject"/>.
		/// </summary>
		/// <param name="player"></param>
		public virtual void Use(Player player)
		{
			switch (goType)
			{
				case GameObjectType.Chest:
					// only one person can use a chest
					if (lootGeneration > 0)
						return;
					// if there are no loot container or if the player already has loot from us just return
					if (lootContainer == null || lootContainer.HasLootFor(player))
						return;
					// generate loot for that player
					this.lootContainer.GenerateLootFor(player);
					this.lootGeneration++;
					// if there is no loot available for the player despawn us
					var waitTime = ServerGameSettings.LOOT_CHEST_WAIT_TIME_BEFORE_LOOTED_MS;
					if (!lootContainer.HasLootFor(player))
						waitTime = ServerGameSettings.LOOT_CHEST_WAIT_TIME_AFTER_LOOTED_MS;
					// the queue us for respawn
					this.QueueRespawn(waitTime, ServerGameSettings.LOOT_CHEST_RESPAWN_TIME_MS, this.iPosition, this.iRotation);
					break;
			}
		}

		/// <summary>
		/// Determine whether a(n) <see cref="Player"/> can gather this <see cref="Gameobject"/> or not.
		/// </summary>
		public virtual bool CanGather(Player player)
		{
			switch (goType)
			{
				case GameObjectType.Plant:
				case GameObjectType.Vein:
					if (lootRestriction == LootRestriction.SingleLooter)
						return lootGeneration == 0;
					return true;
			}
			return false;
		}

		/// <summary>
		/// Gathers the <see cref="Gameobject"/>.
		/// </summary>
		/// <param name="player"></param>
		public virtual void Gather(Player player)
		{
			switch (goType)
			{
				case GameObjectType.Plant:
				case GameObjectType.Vein:
					// if there are no loot container or if the player already has loot from us just return
					if (lootContainer == null || lootContainer.HasLootFor(player))
						return;
					// generate loot for that player
					this.lootContainer.GenerateLootFor(player);
					this.lootGeneration++;
					// if there is no loot available for the player despawn us
					var waitTime = ServerGameSettings.GATHER_WAIT_TIME_BEFORE_LOOTED_MS;
					if (!lootContainer.HasLootFor(player))
						waitTime = ServerGameSettings.GATHER_WAIT_TIME_AFTER_LOOTED_MS;
					// the queue us for respawn
					this.World.PrimaryFiber.Schedule(() =>
						{
							this.lootContainer.RemoveLootFor(player);
							// hide our player from the owners view so he or she cannot loot anymore
							this.HideFromPlayer(player.Guid);
							this.World.PrimaryFiber.Schedule(() => this.UnhideFromPlayer(player.Guid), ServerGameSettings.GATHER_RESPAWN_TIME_MS);

						}, waitTime);
					break;
			}
		}

		/// <summary>
		/// Determine whether a(n) <see cref="Player"/> can collect this <see cref="Gameobject"/> or not.
		/// </summary>
		public virtual bool CanCollect(Player player)
		{
			switch (goType)
			{
				case GameObjectType.Item:
					return true;
			}
			return false;
		}

		/// <summary>
		/// Collects the <see cref="Gameobject"/>.
		/// </summary>
		/// <param name="player"></param>
		public virtual void Collect(Player player)
		{
			switch (goType)
			{
				case GameObjectType.Item:
					// TODO: Collect the item
					break;
			}
		}

		#endregion

		#region Loot System Implementation

		/// <summary>
		/// Tells whether this <see cref="Gameobject"/> has loot for a(n) <see cref="Player"/> or not.
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
			return lootContainer != null ? lootContainer.GetLootFor(player) : EmptyLootContainer.EmptyLoot;
		}

		#endregion
	};
}
