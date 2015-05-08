namespace Karen90MmoFramework.Quantum
{
	public interface ICharacterController
	{
		Vector3 Position { get; set; }
		Quaternion Orientation { get; set; }
		Vector3 LocalVelocity { get; set; }
		Vector3 WorldVelocity { get; }

		float Radius { get; }
		float Height { get; }

		void Update(float delta);
	}
}
