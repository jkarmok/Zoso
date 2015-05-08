namespace Karen90MmoFramework.Game
{
	/// <summary>
	/// Spell cast result
	/// </summary>
	public enum 
		CastResults : byte
	{
		Ok						= 0,
		OutOfRange				= 1,
		TargetTooClose			= 2,
		NotEnoughPower			= 3,
		TargetIsDead			= 4,
		CasterIsDead			= 5,
		TargetRequired			= 6,
		InvalidTarget			= 7,
		SpellNotReady			= 8,
		SpellNotFound			= 9,
		InCooldown				= 10,
		InGcd					= 11,
		AlreadyCasting			= 12,
	};
}