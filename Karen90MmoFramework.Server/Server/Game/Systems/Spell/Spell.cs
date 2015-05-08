using System.Linq;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public partial class Spell
	{
		#region Constants and Fields

		private readonly Character caster;

		private readonly short id;
		private readonly SpellSchools school;
		private readonly SpellTargetTypes requiredTargetType;
		private readonly SpellTargetSelectionMethods targetSelectionMethod;
		private readonly SpellAffectionMethod spellAffectionMethod;
		private readonly WeaponTypes requiredWeaponType;
		private readonly SpellFlags flags;
		private readonly SpellEffects[] effects;
		private readonly int[] effectBaseValues;
		private readonly bool affectedByGcd;
		private readonly bool isProc;
		private readonly bool triggersGcd;
		private readonly Vitals powerType;
		private readonly int powerCost;
		private readonly int minCastRadius;
		private readonly int maxCastRadius;
		private readonly float castTime;
		private readonly float cooldown;

		private float cooldownTimer;
		private float castingTimer;
	
		private MmoObject target;
		private SpellStates state;
		private ISpellController spellManager;

		private Vector3 lastCasterPosition;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the Id
		/// </summary>
		public short Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the spell school
		/// </summary>
		public SpellSchools School
		{
			get
			{
				return this.school;
			}
		}

		/// <summary>
		/// Gets the value indicating the spell's affection method
		/// </summary>
		public SpellTargetSelectionMethods TargetSelectionMethod
		{
			get
			{
				return this.targetSelectionMethod;
			}
		}

		/// <summary>
		/// Gets the required castTarget type
		/// </summary>
		public SpellTargetTypes RequiredTargetType
		{
			get
			{
				return this.requiredTargetType;
			}
		}

		/// <summary>
		/// [Flags] Gets the required weapon types
		/// </summary>
		public WeaponTypes RequiredWeaponType
		{
			get
			{
				return this.requiredWeaponType;
			}
		}

		/// <summary>
		/// [Flags] Gets the custom flags set for the spell
		/// </summary>
		public SpellFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		/// <summary>
		/// Tells whether the spell is affected by the Gcd or not
		/// </summary>
		public bool AffectedByGCD
		{
			get
			{
				return this.affectedByGcd;
			}
		}

		/// <summary>
		/// Tells whether the spell triggers Gcd or not
		/// </summary>
		public bool TriggersGCD
		{
			get
			{
				return this.triggersGcd;
			}
		}

		/// <summary>
		/// Tells whether the spell is initialized or not
		/// </summary>
		public bool IsReady { get; private set; }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates an instance of <see cref="Spell"/> from <see cref="SpellData"/>.
		/// </summary>
		/// <param name="spellData"> RavenItem which contains the spell info. </param>
		/// <param name="caster"></param>
		public Spell(SpellData spellData, Character caster)
		{
			this.caster = caster;
			
			this.id = spellData.SpellId;
			this.school = (SpellSchools) spellData.School;
			this.requiredTargetType = (SpellTargetTypes) spellData.RequiredTargetType;
			this.targetSelectionMethod = (SpellTargetSelectionMethods) spellData.TargetSelectionMethod;
			this.spellAffectionMethod = (SpellAffectionMethod) spellData.AffectionMethod;
			this.requiredWeaponType = (WeaponTypes) spellData.RequiredWeaponType;
			this.flags = (SpellFlags) spellData.Flags;

			this.powerType = (Vitals) spellData.PowerType;
			this.powerCost = spellData.PowerCost;
			this.cooldown = spellData.Cooldown;
			this.minCastRadius = spellData.MinCastRadius;
			this.maxCastRadius = spellData.MaxCastRadius;
			this.castTime = spellData.CastTime;

			this.affectedByGcd = spellData.AffectedByGCD;
			this.isProc = spellData.IsProc;
			this.triggersGcd = spellData.TriggersGCD;

			var spellEffects = spellData.Effects;
			this.effects = spellEffects != null ? spellEffects.Select(i => (SpellEffects) i).ToArray() : null;
			this.effectBaseValues = spellData.EffectBaseValues;

			this.state = SpellStates.Null;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes the spell. This MUST be called before the spell can be casted.
		/// </summary>
		public void Initialize(ISpellController mgr)
		{
			this.spellManager = mgr;
			this.LoadHandlers();

			this.state = this.isProc ? SpellStates.WaitingForProc : SpellStates.Idle;
			this.IsReady = true;
		}

		/// <summary>
		/// Checks to see whether the spell can be casted on self or not.
		/// </summary>
		protected CastResults CheckSpell()
		{
			if ((flags & SpellFlags.FLAG_SPECIAL_NO_REQ_CHECK) == SpellFlags.FLAG_SPECIAL_NO_REQ_CHECK)
				return CastResults.Ok;

			if (state != SpellStates.Idle)
				return CastResults.SpellNotReady;

			if (targetSelectionMethod == SpellTargetSelectionMethods.AreaOfEffect || targetSelectionMethod == SpellTargetSelectionMethods.Cone)
				// needs to check whether the character is in live or ghost mode
				return caster.HaveVitalPower(this.powerType, this.powerCost) ? CastResults.Ok : CastResults.NotEnoughPower;

			if ((requiredTargetType & SpellTargetTypes.Self) != SpellTargetTypes.Self)
				return CastResults.TargetRequired;

			return CastResults.Ok;
		}

		/// <summary>
		///  Checks to see whether the spell can be casted on a(n) <see cref="MmoObject"/> or not.
		/// </summary>
		protected CastResults CheckSpell(MmoObject victim)
		{
			if ((flags & SpellFlags.FLAG_SPECIAL_NO_REQ_CHECK) == SpellFlags.FLAG_SPECIAL_NO_REQ_CHECK)
				return CastResults.Ok;

			if (victim == null || victim == this.caster)
				return this.CheckSpell();

			if (state != SpellStates.Idle)
				return CastResults.SpellNotReady;

			if (targetSelectionMethod == SpellTargetSelectionMethods.AreaOfEffect || targetSelectionMethod == SpellTargetSelectionMethods.Cone)
				// TODO: Needs to check to see whether the character is in live or ghost mode
				return caster.HaveVitalPower(this.powerType, this.powerCost) ? CastResults.Ok : CastResults.NotEnoughPower;

			switch ((ObjectType)victim.Guid.Type)
			{
				case ObjectType.Player:
				case ObjectType.Npc:
					{
						var character = (Character) victim;
						if (character.IsHostileTo(caster))
						{
							if ((!character.IsDead() && requiredTargetType.HasFlag(SpellTargetTypes.HostileUnit)) == false &&
									(character.IsDead() && requiredTargetType.HasFlag(SpellTargetTypes.HostileCorpse)) == false)
								return CastResults.InvalidTarget;
						}
						else
						{
							if ((!character.IsDead() && requiredTargetType.HasFlag(SpellTargetTypes.FriendlyUnit)) == false &&
									(character.IsDead() && requiredTargetType.HasFlag(SpellTargetTypes.FriendlyCorpse)) == false)
								return CastResults.InvalidTarget;
						}
					}
					break;

				default:
					return CastResults.InvalidTarget;
			}

			if (!caster.HaveVitalPower(this.powerType, this.powerCost))
				return CastResults.NotEnoughPower;

			var sqrDistance = Vector3.SqrDistance(caster.Position, victim.Position);
			if (sqrDistance <= this.minCastRadius * this.minCastRadius)
				return CastResults.TargetTooClose;

			if (sqrDistance > this.maxCastRadius * this.maxCastRadius)
				return CastResults.OutOfRange;

			return CastResults.Ok;
		}

		/// <summary>
		/// Casts the spell on a(n) <see cref="MmoObject"/>
		/// </summary>
		public CastResults Cast(MmoObject victim)
		{
			if (victim == null)
				victim = this.caster;

			var result = this.CheckSpell(victim);
			if (result == CastResults.Ok)
			{
				this.target = victim;
				this.caster.DrainVitalPower(this.powerType, this.powerCost);

				if (castTime > 0)
				{
					this.state = SpellStates.Casting;
					this.castingTimer = 0;

					this.spellManager.OnBeginCasting(this);
				}
				else
				{
					this.state = SpellStates.CastFinished;
				}

				this.lastCasterPosition = caster.Position;
				this.spellManager.AddSpellUpdateEvent(this);
			}

			return result;
		}

		/// <summary>
		/// Interupts the spell casting
		/// </summary>
		public void InteruptCast()
		{
			if (state == SpellStates.Casting)
			{
				this.spellManager.RemoveSpellUpdateEvent(this);
				this.spellManager.OnEndCasting(this, true);

				this.castingTimer = 0;
				this.target = null;

				this.state = this.isProc ? SpellStates.WaitingForProc : SpellStates.Idle;
			}
		}

		/// <summary>
		/// Updates the spell.
		/// </summary>
		public void Update(float deltaTime)
		{
			// NOTE: deltaTime is in seconds

			switch (state)
			{
				case SpellStates.Casting:
					{
						// checking for movement
						if ((flags & SpellFlags.FLAG_MOVEMENT_BREAKS_CAST) == SpellFlags.FLAG_MOVEMENT_BREAKS_CAST)
						{
							if (lastCasterPosition != caster.Position)
							{
								this.InteruptCast();
								return;
							}
						}

						this.castingTimer += deltaTime;
						if (castingTimer >= castTime)
						{
							this.state = SpellStates.CastFinished;
							this.castingTimer = 0;
						}
					}
					break;

				case SpellStates.CastFinished:
					{
						if (castTime > 0)
							this.spellManager.OnEndCasting(this, false);

						switch (spellAffectionMethod)
						{
							case SpellAffectionMethod.Instant:
								{
									this.ApplyEffect(target);
								}
								break;

							case SpellAffectionMethod.OnHit:
								{
									var spellObject = Dynamic.CreateNew(caster.CurrentZone, this.id);
									spellObject.InitializeNewSpellObject();
									// offsetting the spell position to that of the character's center
									var position = caster.Position + caster.Bounds.Center;
									spellObject.Spawn(position, Quaternion.FromToRotation(Vector3.Forward, target.Position - position));

									var mTarget = target;
									spellObject.SpellChase(target, caster, 15, () => this.ApplyEffect(mTarget));
								}
								break;
						}

						this.ResetSpell();
					}
					break;

				case SpellStates.Cooldown:
					{
						this.cooldownTimer += deltaTime;
						if (cooldownTimer >= cooldown)
						{
							this.spellManager.RemoveSpellUpdateEvent(this);
							this.state = this.isProc ? SpellStates.WaitingForProc : SpellStates.Idle;

							this.spellManager.OnEndCooldown(this);
						}
					}
					break;
			}
		}

		/// <summary>
		/// Applies the spell effect on the casting target
		/// </summary>
		void ApplyEffect(MmoObject victim)
		{
			if (effects != null)
			{
				for (int i = 0; i < this.effects.Length; i++)
				{
					var index = (int)this.effects[i];
					this.effectHandlers[index](i, victim);
				}
			}
		}

		/// <summary>
		/// Resets the spell
		/// </summary>
		void ResetSpell()
		{
			this.target = null;

			if (cooldown > 0)
			{
				this.state = SpellStates.Cooldown;
				this.cooldownTimer = 0;

				this.spellManager.OnBeginCooldown(this);
			}
			else
			{
				this.state = this.isProc ? SpellStates.WaitingForProc : SpellStates.Idle;
				this.spellManager.RemoveSpellUpdateEvent(this);
			}
		}

		#endregion
	}
}
