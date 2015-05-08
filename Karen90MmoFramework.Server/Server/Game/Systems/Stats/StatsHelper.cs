using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public static class StatsHelper
	{
		private static readonly CharacterStatCalculator _defaultStatCalculator = new DefaultStatCalculator();
		private static readonly CharacterStatCalculator _maxHealthCalculator = new MaxHealthCalculator();
		private static readonly CharacterStatCalculator _maxPowerCalculator = new MaxPowerCalculator();
		private static readonly CharacterStatCalculator _weaponDamageCalculator = new WeaponDamageCalculator();
		private static readonly CharacterStatCalculator _spellDamageCalculator = new SpellDamageCalculator();
		private static readonly CharacterStatCalculator _armorCalculator = new ArmorCalculator();
		private static readonly CharacterStatCalculator _absorptionCalculator = new AbsorptionCalculator();

		/// <summary>
		/// Gets the <see cref="CharacterStatCalculator"/> for a particular <see cref="Stats"/>.
		/// </summary>
		/// <param name="stat"></param>
		/// <returns></returns>
		public static CharacterStatCalculator GetCharacterStatCalculator(Stats stat)
		{
			switch (stat)
			{
				case Stats.Health:
					return _maxHealthCalculator;

				case Stats.Power:
					return _maxPowerCalculator;

				case Stats.WeaponDamage:
					return _weaponDamageCalculator;

				case Stats.SpellDamage:
					return _spellDamageCalculator;

				case Stats.Armor:
					return _armorCalculator;

				case Stats.Absorption:
					return _absorptionCalculator;

				default:
					return _defaultStatCalculator;
			}
		}
	}
}
