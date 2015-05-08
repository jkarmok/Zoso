using System;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public partial class Spell
	{
		private Action<int, MmoObject>[] effectHandlers;

		/// <summary>
		/// Loads the handlers to handle effects
		/// </summary>
		private void LoadHandlers()
		{
			this.effectHandlers = new Action<int, MmoObject>[]
				{
					HandleEffectSchoolDamage, // 0
					HandleEffectHeal, // 1
					HandleEffectWeaponDamage, // 2
					HandleEffectPowerDrain, // 3
					HandleEffectApplyAura, // 4
					HandleEffectPowerGain, // 5
				};
		}

		private void HandleEffectSchoolDamage(int effectIndex, MmoObject victim)
		{
			// TODO: Compensate dmg for school defense

			switch ((ObjectType) victim.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					{
						var amount = this.effectBaseValues[effectIndex];
						var charUnit = (Character) victim;
						charUnit.Damage(caster, amount, this);
					}
					break;
			}
		}

		private void HandleEffectHeal(int effectIndex, MmoObject victim)
		{
			switch ((ObjectType) victim.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					{
						var amount = this.effectBaseValues[effectIndex];
						var charUnit = (Character) victim;
						charUnit.Heal(caster, amount, this);
					}
					break;
			}
		}

		private void HandleEffectWeaponDamage(int effectIndex, MmoObject victim)
		{
			switch ((ObjectType) victim.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					{
						var amount = this.effectBaseValues[effectIndex];
						var charUnit = (Character) victim;
						charUnit.Damage(caster, amount, this);
					}
					break;
			}
		}

		private void HandleEffectPowerDrain(int effectIndex, MmoObject victim)
		{
			switch ((ObjectType) victim.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					{
						var amount = this.effectBaseValues[effectIndex];
						var charUnit = (Character) victim;
						charUnit.DrainPower(caster, amount, this);
					}
					break;
			}
		}

		private void HandleEffectApplyAura(int effectIndex, MmoObject victim)
		{
			Utils.Logger.DebugFormat("Effect: ApplyAura. Value: {0}. Target: {1}", this.effectBaseValues[effectIndex], victim.Guid);
		}

		private void HandleEffectPowerGain(int effectIndex, MmoObject victim)
		{
			switch ((ObjectType) victim.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					{
						var amount = this.effectBaseValues[effectIndex];
						var charUnit = (Character) victim;
						charUnit.GainPower(caster, amount, this);
					}
					break;
			}
		}
	}
}
