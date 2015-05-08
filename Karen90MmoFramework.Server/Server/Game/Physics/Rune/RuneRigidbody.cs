using DigitalRune.Geometry;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneRigidbody : RigidBody, IWorldObject
	{
		private readonly IShape shape;

		Vector3 IWorldObject.Position
		{
			get
			{
				return this.Pose.Position.ToGameVector();
			}
			set
			{
				this.Pose = new Pose(value.ToRuneVector(), this.Pose.Orientation);
			}
		}

		Quaternion IWorldObject.Rotation
		{
			get
			{
				return QuaternionF.CreateRotation(this.Pose.Orientation).ToGameQuaternion();
			}
			set
			{
				this.Pose = new Pose(this.Pose.Position, value.ToRuneQuaternion().ToRotationMatrix33());
			}
		}

		IShape IWorldObject.Shape
		{
			get
			{
				return this.shape;
			}
		}

		public RuneRigidbody(IRuneShape shape)
			: base(shape.Shape)
		{
			this.shape = shape;
		}
	}
}
