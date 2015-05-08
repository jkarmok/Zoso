namespace Karen90MmoFramework.Game
{
	/// <summary>
	/// The type of method by which the method affects targets
	/// </summary>
	public enum SpellTargetSelectionMethods : byte
	{
		SingleTarget				= 0,
		AreaOfEffect				= 1,
		Cone						= 2,
		Channel						= 3,
	};
}