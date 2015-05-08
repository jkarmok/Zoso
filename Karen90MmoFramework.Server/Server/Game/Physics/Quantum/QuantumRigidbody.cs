using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Quantum
{
	public class QuantumRigidbody : IWorldObject
	{
		private readonly IShape shape;

		public Vector3 Position { get; set; }

		public Quaternion Rotation { get; set; }

		public IShape Shape
		{
			get
			{
				return shape;
			}
		}

		public QuantumRigidbody(IShape shape)
		{
			this.shape = shape;
		}
	}
}
