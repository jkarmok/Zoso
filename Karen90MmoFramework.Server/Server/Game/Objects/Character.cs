using System.Collections.Generic;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Server.Game.Objects
{
	public abstract class Character : MmoObject
	{
		#region Constants and Fields

		private readonly string name;
		private readonly Species species;
		private readonly Race race;

		private readonly short[] baseStats;

		private byte level;
		private int currentHealth;
		private int currentPower;
		private UnitState state;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the character's name
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the character's species
		/// </summary>
		public Species Species
		{
			get
			{
				return this.species;
			}
		}

		/// <summary>
		/// Gets the character's race
		/// </summary>
		public Race Race
		{
			get
			{
				return this.race;
			}
		}

		/// <summary>
		/// Gets the level
		/// </summary>
		public byte Level
		{
			get
			{
				return this.level;
			}
		}

		/// <summary>
		/// Gets the maximum health
		/// </summary>
		public short MaximumHealth
		{
			get
			{
				return this.CalculateStat(Stats.Health);
			}
		}

		/// <summary>
		/// Gets the maximum power
		/// </summary>
		public short MaximumPower
		{
			get
			{
				return this.CalculateStat(Stats.Power);
			}
		}

		/// <summary>
		/// Gets the current health
		/// </summary>
		public int CurrentHealth
		{
			get
			{
				return this.currentHealth;
			}
		}

		/// <summary>
		/// Gets the current power
		/// </summary>
		public int CurrentPower
		{
			get
			{
				return this.currentPower;
			}
		}

		/// <summary>
		/// Gets the state
		/// </summary>
		public UnitState State
		{
			get
			{
				return this.state;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="Character"/> class.
		/// </summary>
		protected Character(MmoZone zone, ObjectType type, int objectId, short familyId, Bounds bounds, CharacterData characterData)
			: base(zone, type, objectId, familyId, bounds, null)
		{
			this.name = characterData.Name;
			this.race = (Race) characterData.Race;
			this.species = (Species) characterData.Species;
			this.level = characterData.Level;
			this.state = UnitState.Alive;

			this.baseStats = new short[Stats.eLastStat - Stats.eFirstStat + 1];
		}

		#endregion

		#region Respawn System Implementation

		/// <summary>
		/// Called right before spawning.
		/// Sets <see cref="State"/> to <see cref="UnitState.Alive"/>.
		/// Sets <see cref="CurrentHealth"/> to <see cref="MaximumHealth"/>.
		/// Sets <see cref="CurrentPower"/> to <see cref="MaximumPower"/>.
		/// </summary>
		protected override void OnAwake()
		{
			base.OnAwake();

			// dont need to publish any of these since this happens before spawning
			this.SetUnitState(UnitState.Alive);
			this.SetHealth(MaximumHealth);
			this.SetPower(MaximumPower);
		}

		#endregion

		#region Stats System Implementation

		/// <summary>
		/// Gets all the base stats
		/// </summary>
		public IEnumerable<short> GetBaseStats()
		{
			return this.baseStats;
		}

		/// <summary>
		/// Gets the value of a certain <see cref="Stats"/>.
		/// </summary>
		public short GetBaseStat(Stats stat)
		{
			var index = stat - Stats.eFirstStat;
			if (index >= baseStats.Length)
				return 0;

			return this.baseStats[index];
		}

		/// <summary>
		/// Sets the value of a certain <see cref="Stats"/>.
		/// </summary>
		public short SetBaseStat(Stats stat, short value)
		{
			if (value < 0)
				value = 0;

			var index = stat - Stats.eFirstStat;
			if (index >= baseStats.Length)
				return 0;

			this.baseStats[index] = value;
			return value;
		}

		/// <summary>
		/// Calculates the resultant value of a <see cref="Stats"/>.
		/// </summary>
		public short CalculateStat(Stats stat)
		{
			return StatsHelper.GetCharacterStatCalculator(stat).CalculateValue(this, stat);
		}

		/// <summary>
		/// Sets the current level
		/// </summary>
		protected void SetLevel(byte value, bool publish = false)
		{
			var maxLevel = Guid.Type == (byte) ObjectType.Player ? GlobalGameSettings.MAX_PLAYER_LEVEL : GlobalGameSettings.MAX_NPC_LEVEL;
			var newLevel = (byte) Mathf.Clamp(value, 1, maxLevel);
			if (newLevel == level)
				return;

			this.level = newLevel;
			this.DirtyProperties |= PropertyFlags.Level;
			this.Revision++;

			if (Guid.Type == (byte) ObjectType.Player)
			{
				var player = (Player) this;
				if (player.InGroup())
				{
					// if we are in group set the group flags so the group members can receive updates
					player.MemberUpdateFlags |= GroupMemberPropertyFlags.PROPERTY_FLAG_LEVEL;
				}
			}

			if (publish)
				this.PublishProperty(PropertyCode.Level, this.Level);
		}

		/// <summary>
		/// Sets health
		/// </summary>
		protected void SetHealth(int value, bool publish = false)
		{
			// this will prevent from setting the same value over and over or min and max values
			// this will also save us from publishing messages if the value did not change
			var newHealth = Mathf.Clamp(value, 0, this.MaximumHealth);
			if (newHealth == currentHealth)
				return;

			this.currentHealth = newHealth;
			this.DirtyProperties |= PropertyFlags.CurrHp;
			this.Revision++;

			if (Guid.Type == (byte) ObjectType.Player)
			{
				var player = (Player) this;
				if (player.InGroup())
				{
					// if we are in group set the group flags so the group members can receive updates
					player.MemberUpdateFlags |= GroupMemberPropertyFlags.PROPERTY_FLAG_CURR_HP;
				}
			}

			if (publish)
				this.PublishProperty(PropertyCode.CurrHp, this.currentHealth);
		}

		/// <summary>
		/// Sets max health
		/// </summary>
		protected void SetMaxHealth(short value, bool publish = false)
		{
			// this will prevent from setting the same value over and over or min and max values
			// this will also save us from publishing messages if the value did not change
			var newMaxHealth = (short) Mathf.Max(1, value); // max hp cannot be 0
			if (newMaxHealth == MaximumHealth)
				return;

			newMaxHealth = this.SetBaseStat(Stats.Health, newMaxHealth);
			this.DirtyProperties |= PropertyFlags.MaxHp;
			this.Revision++;

			if (Guid.Type == (byte) ObjectType.Player)
			{
				var player = (Player) this;
				if (player.InGroup())
				{
					// if we are in group set the group flags so the group members can receive updates
					player.MemberUpdateFlags |= GroupMemberPropertyFlags.PROPERTY_FLAG_MAX_HP;
				}
			}

			if (publish)
				this.PublishProperty(PropertyCode.MaxHp, newMaxHealth);
		}

		/// <summary>
		/// Sets power
		/// </summary>
		protected void SetPower(int value, bool publish = false)
		{
			// this will prevent from setting the same value over and over or min and max values
			// this will also save us from publishing messages if the value did not change
			var newPower = Mathf.Clamp(value, 0, this.MaximumPower);
			if (newPower == currentPower)
				return;

			// power will not be set in the property hashtable
			// because it will not be visible to other players
			this.currentPower = newPower;
			if (publish)
				this.PublishProperty(PropertyCode.CurrPow, this.currentPower);
		}

		/// <summary>
		/// Sets max power
		/// </summary>
		protected void SetMaxPower(int value, bool publish = false)
		{
			// this will prevent from setting the same value over and over or min and max values
			// this will also save us from publishing messages if the value did not change
			var newMaxPower = (short) Mathf.Max(1, value); // max pow cannot be 0
			if (newMaxPower == MaximumPower)
				return;

			// max power will not be set in the property hashtable
			// because it will not be visible to other players
			newMaxPower = this.SetBaseStat(Stats.Health, newMaxPower);
			if (publish)
				this.PublishProperty(PropertyCode.MaxPow, newMaxPower);
		}

		/// <summary>
		/// Sets the character state
		/// </summary>
		protected void SetUnitState(UnitState newState, bool publish = false)
		{
			if (state == newState)
				return;

			this.state = newState;
			this.DirtyProperties |= PropertyFlags.UnitState;
			this.Revision++;

			if (publish)
				this.PublishProperty(PropertyCode.UnitState, this.state);
		}

		#endregion

		#region Combat System Implementation

		/// <summary>
		/// Returns whether the <see cref="Character"/> is dead.
		/// </summary>
		/// <returns></returns>
		public virtual bool IsDead()
		{
			return this.state == UnitState.Dead;
		}

		/// <summary>
		/// Tells whether a <see cref="Character"/> is hostile to this <see cref="Character"/> or not.
		/// Returns false by default.
		/// </summary>
		public virtual bool IsHostileTo(Character character)
		{
			return false;
		}

		/// <summary>
		/// Heals the character.
		/// </summary>
		public void Heal(MmoObject cUnit, int value, Spell spell)
		{
			if (state == UnitState.Dead)
				return;

			// set hp and publish
			this.SetHealth(currentHealth + value, true);
			// TODO: Add threat to all attackers from cUnit
		}

		/// <summary>
		/// Damages the character. Calls <see cref="AddThreat(MmoObject, float, Spell)"/> if <see cref="CanAddThreat()"/> returns <value>true</value>.
		/// </summary>
		public void Damage(MmoObject cUnit, int value, Spell spell)
		{
			// already dead so move along
			if (state == UnitState.Dead)
				return;

			// dont publish curr hp since the char may die after dmg
			this.SetHealth(currentHealth - value);
			if (CanAddThreat())
			{
				var threat = value * ServerGameSettings.THREAT_DAMAGE_MULTIPLIER;
				this.AddThreat(cUnit, threat, spell);
			}

			// kills the char if health is 0
			if (currentHealth <= 0)
				this.Kill(cUnit);

			// only publish curr hp if the char is not dead
			if (state != UnitState.Dead)
				this.PublishProperty(PropertyCode.CurrHp, this.currentHealth);
		}

		/// <summary>
		/// Gains power.
		/// </summary>
		public void GainPower(MmoObject cUnit, int value, Spell spell)
		{
			// already dead so move along
			if (state == UnitState.Dead)
				return;

			// set pow and publish
			this.SetPower(currentPower + value, true);
		}

		/// <summary>
		/// Drains power. Calls <see cref="AddThreat(MmoObject, float, Spell)"/> if <see cref="CanAddThreat()"/> returns <value>true</value>.
		/// </summary>
		public void DrainPower(MmoObject cUnit, int value, Spell spell)
		{
			// already dead so move along
			if (state == UnitState.Dead)
				return;

			// set pow and publish
			this.SetPower(currentPower - value, true);
			if (CanAddThreat())
			{
				var threat = value * ServerGameSettings.THREAT_POWER_MULTIPLIER;
				this.AddThreat(cUnit, threat, spell);
			}
		}

		/// <summary>
		/// Gains vital power. Vital power is used to cast spells and does not add threat.
		/// </summary>
		public void GainVitalPower(Vitals vital, int value)
		{
			// already dead so move along
			if (state == UnitState.Dead)
				return;

			switch (vital)
			{
				case Vitals.Health:
					{
						// set hp and publish
						this.SetHealth(this.currentHealth + value, true);
					}
					break;

				case Vitals.Power:
					{
						// set pow and publish
						this.SetPower(this.currentPower + value, true);
					}
					break;
			}
		}

		/// <summary>
		/// Drains vital power. Vital power is used to cast spells and does not add threat.
		/// </summary>
		public void DrainVitalPower(Vitals vital, int value)
		{
			// already dead so move along
			if (state == UnitState.Dead)
				return;

			switch (vital)
			{
				case Vitals.Health:
					{
						// dont publish curr hp since the char may die after dmg
						this.SetHealth(this.currentHealth - value);

						if (currentHealth <= 0)
							this.Kill(this); // killing self

						// only publish curr hp if the char is not dead
						if (state != UnitState.Dead)
							this.PublishProperty(PropertyCode.CurrHp, this.currentHealth);
					}
					break;

				case Vitals.Power:
					{
						// set pow and publish
						this.SetPower(this.currentPower - value, true);
					}
					break;
			}
		}

		/// <summary>
		/// Tells whether the character has a certain amount of vital power or not
		/// </summary>
		public bool HaveVitalPower(Vitals vital, int value)
		{
			switch (vital)
			{
				case Vitals.Health:
					// to avoid death
					return currentHealth > value;

				case Vitals.Power:
					return currentPower >= value;
			}

			return false;
		}

		/// <summary>
		/// Kills the <see cref="Character"/> and calls <see cref="OnDeath(MmoObject)"/>.
		/// Updates the <see cref="State"/> to <see cref="UnitState.Dead"/> and resets <see cref="CurrentHealth"/> and <see cref="CurrentPower"/> to <value>0</value>.
		/// Publishes the current <see cref="UnitState"/>.
		/// </summary>
		public void Kill(MmoObject killer)
		{
			if (state == UnitState.Dead)
				return;

			// set state and publish
			this.SetUnitState(UnitState.Dead, true);
			// curr hp and curr pow dont need to be published
			// since when the client receives SetUnitState(Dead)
			// it will automatically updates them and the revision
			this.SetHealth(0);
			this.SetPower(0);

			this.OnDeath(killer);
		}

		/// <summary>
		/// Called when this <see cref="Character"/> is killed. Remember the <paramref name="killer"/> could be <value>this</value>.
		/// This will be called after calling <see cref="Kill(MmoObject)"/>.
		/// Does nothing by default.
		/// </summary>
		protected virtual void OnDeath(MmoObject killer)
		{
		}

		#endregion

		#region Threat System Implementation

		/// <summary>
		/// Tells whether the character can receive threat.
		/// Returns <value>true</value> if the <see cref="Character"/> is of type <see cref="ObjectType.Npc"/> by default.
		/// </summary>
		public virtual bool CanAddThreat()
		{
			return this.Guid.Type == (byte) ObjectType.Npc;
		}

		/// <summary>
		/// Adds threat
		/// </summary>
		public virtual void AddThreat(MmoObject cUnit, float amount, Spell spell)
		{
		}

		/// <summary>
		/// Removes threat
		/// </summary>
		public virtual void RemoveThreat(MmoObject cUnit, float amount, Spell spell)
		{
		}

		#endregion
	};
}
