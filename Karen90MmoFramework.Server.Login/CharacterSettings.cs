using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Login
{
	// TODO: Need to load it from a file
	public static class CharacterSettings
	{
		public const int NEW_CHARACTER_DEFAULT_ZONE_ID = 1;
		public const byte NEW_CHARACTER_DEFAULT_LEVEL = 1;
		
		public static readonly Vector3 NewCharacterDefaultPosition = new Vector3 { X = 1450, Y = 0, Z = 1365 };
		public static readonly float NewCharacterDefaultOrientation = 0;

		public static readonly short[] NewCharacterDefaultStats = new short[]
			{
				100,	// Health
				120,	// Power
				10,		// WeaponDamage
				12,		// SpellDamage
				0,		// Armor
				0,		// Absorption
			};

		public static readonly short[] NewCharacterDefaultSpells = new short[]
			{
				10,		// BasicAttack
				13,		// FireBolt
				14,		// Heal
			};

		public static readonly ItemStructure[] NewCharacterDefaultItems = new ItemStructure[]
			{
			};
	}
}
