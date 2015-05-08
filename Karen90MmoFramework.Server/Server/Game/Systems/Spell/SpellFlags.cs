namespace Karen90MmoFramework.Server.Game.Systems
{
	/// <summary>
	/// Spell flags used for custom spell checks. Extend if necessary
	/// </summary>
	[System.Flags]
	public enum SpellFlags
	{
		FLAG_NONE						= 0x0,
		FLAG_RANGED						= 0x1,
		FLAG_MELEE						= 0x2,
		FLAG_ITEM						= 0x4,
		FLAG_CANT_USE_IN_COMBAT			= 0x8,
		FLAG_DONT_ADD_THREAT			= 0x10,
		FLAG_SPECIAL_NO_REQ_CHECK		= 0x20,		// special flag only for object uses (gathering, looting, etc)
		FLAG_SPECIAL_SERVER_ONLY		= 0x40,		// will not be visible in the client
		FLAG_MOVEMENT_BREAKS_CAST		= 0x80,		// moving will interupt the casting
	};
}