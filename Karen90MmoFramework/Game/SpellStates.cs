namespace Karen90MmoFramework.Game
{
	/// <summary>
	/// State of the spell
	/// </summary>
	public enum SpellStates : byte
	{
		Null						= 0,		// spell not initialized
		Idle						= 1,
		Casting						= 2,
		CastFinished				= 3,
		Cooldown					= 4,
		WaitingForProc				= 5,
	};
}