namespace Karen90MmoFramework.Game
{
	[System.Flags]
	public enum SpellTargetTypes : byte
	{
		None					= 0x0,
		Self					= 0x1,
		FriendlyUnit			= 0x2,
		HostileUnit				= 0x4,
		FriendlyCorpse			= 0x8,
		HostileCorpse			= 0x10,

		/// <summary>
		/// Friendly_Unit | Hostile_Unit
		/// </summary>
		AllUnit = FriendlyUnit | HostileUnit,

		/// <summary>
		/// Friendly_Corpse | Hostile_Corpse
		/// </summary>
		AllCorpse = FriendlyCorpse | HostileCorpse,

		/// <summary>
		/// Friendly_Unit | Hostile_Unit | Friendly_Corpse | Hostile_Corpse
		/// </summary>
		AllCharacters = AllUnit | AllCorpse,
	};
}