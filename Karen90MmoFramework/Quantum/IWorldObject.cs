namespace Karen90MmoFramework.Quantum
{
	public interface IWorldObject
	{
		Vector3 Position { get; set; }
		Quaternion Rotation { get; set; }

		IShape Shape { get; }
	}
}
