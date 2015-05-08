namespace Karen90MmoFramework.Quantum
{
	public interface ITriggerVolume
	{
		Vector3 Position { get; set; }
		Quaternion Orientation { get; set; }
		void Update();
	}
}
