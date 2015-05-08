using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game
{
	public struct GlobalPosition
	{
		public short ZoneId;
		public Vector3 Position;

		public GlobalPosition(short zoneId, Vector3 position)
		{
			this.ZoneId = zoneId;
			this.Position = position;
		}
	}
}
