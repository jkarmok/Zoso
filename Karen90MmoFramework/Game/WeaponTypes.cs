namespace Karen90MmoFramework.Game
{
	[System.Flags]
	public enum WeaponTypes : short
	{
		None					= 0x0,
		OneHandedAxe			= 0x1,
		OneHandedSword			= 0x2,
		OneHandedMace			= 0x4,
		TwoHandedAxe			= 0x8,
		TwoHandedSword			= 0x10,
		TwoHandedMace			= 0x20,
		Crossbow				= 0x40,
		Bow						= 0x80,
		Staff					= 0x100,
		Shield					= 0x200,

		/// <summary>
		/// One_Handed_Axe | One_Handed_Sword | One_Handed_Mace
		/// </summary>
		OneHandedMelee = OneHandedAxe | OneHandedSword | OneHandedMace,

		/// <summary>
		/// Two_Handed_Axe | Two_Handed_Sword | Two_Handed_Mace | Staff
		/// </summary>
		TwoHandedMelee = TwoHandedAxe | TwoHandedSword | TwoHandedMace | Staff,

		/// <summary>
		/// One_Handed_Melee | Two_Handed_Melee
		/// </summary>
		Melee = OneHandedMelee | TwoHandedMelee,

		/// <summary>
		/// Crossbow | Bow
		/// </summary>
		Ranged = Crossbow | Bow,
	};
}