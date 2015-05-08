using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Quantum
{
	public class QuantumCharacterController : ICharacterController
	{
		private readonly MmoZone world;

		public QuantumCharacterController(MmoZone world, Vector3 position)
		{
			this.world = world;
			this.Position = position;
			this.Orientation = Quaternion.Identity;
		}

		#region Implementation of ICharacterController

		private Vector3 position;
		private Quaternion orientation;
		private Vector3 localVelocity;
		private Vector3 worldVelocity;

		public Vector3 Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public Quaternion Orientation
		{
			get
			{
				return this.orientation;
			}
			set
			{
				this.orientation = value;
				this.worldVelocity = Vector3.Transform(localVelocity, orientation);
			}
		}

		public Vector3 LocalVelocity
		{
			get
			{
				return this.localVelocity;
			}
			set
			{
				this.localVelocity = value;
				this.worldVelocity = Vector3.Transform(localVelocity, orientation);
			}
		}

		public Vector3 WorldVelocity
		{
			get
			{
				return this.worldVelocity;
			}
		}

		public float Radius
		{
			get
			{
				return 0;
			}
		}

		public float Height
		{
			get
			{
				return 0;
			}
		}

		public void Update(float delta)
		{
			if (worldVelocity.SqrMagnitude() > Vector3.kEpsilon)
			{
				var newPosition = this.position + worldVelocity * delta;
				if (world.Bounds.Contains(newPosition))
					newPosition.Y = world.GetHeight(newPosition.X, newPosition.Z) + this.Height * 0.5f + this.Radius;
				this.Position = newPosition;
			}
		}

		#endregion
	}
}
