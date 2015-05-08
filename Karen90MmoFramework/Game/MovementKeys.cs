namespace Karen90MmoFramework.Game
{
	[System.Flags]
	public enum MovementKeys : byte
	{
		None			= 0,
		Steer			= 1,
		Reverse			= 2,
		TurnRight		= 4,
		TurnLeft		= 8,
		Jump			= 16,
		Shift			= 32,

		MoveKeys		= Steer | Reverse | TurnLeft | TurnRight,
		TurnKeys		= TurnLeft | TurnRight,
		ModifierKeys	= Shift,
	}
}
