namespace Karen90MmoFramework.Game
{
	[System.Flags]
	public enum MovementDirection : byte
	{
		None			= 0x0,
		Forward			= 0x1,
		Backward		= 0x2,
		Left			= 0x4,
		Right			= 0x8,
	};
}
