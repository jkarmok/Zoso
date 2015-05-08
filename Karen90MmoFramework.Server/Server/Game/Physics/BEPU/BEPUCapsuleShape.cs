using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities.Prefabs;

using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	public class BEPUCapsuleShape : Capsule, IRigidbody, ICapsuleShape, IBEPUShape
	{
		Vector3 IRigidbody.Position
		{
			get
			{
				return new Vector3(Position.X, Position.Y, Position.Z);
			}

			set
			{
				base.Position = new BEPUutilities.Vector3(value.X, value.Y, value.Z);
			}
		}

		Quaternion IRigidbody.Rotation
		{
			get
			{
				return new Quaternion(Orientation.X, Orientation.Y, Orientation.Z, Orientation.W);
			}

			set
			{
				this.Orientation = new BEPUutilities.Quaternion(value.X, value.Y, value.Z, value.W);
			}
		}

		IShape IRigidbody.Shape
		{
			get
			{
				return this;
			}
		}

		float ICapsuleShape.Radius
		{
			get
			{
				return Radius;
			}
		}

		float ICapsuleShape.Height
		{
			get
			{
				return Length + Radius * 2;
			}
		}

		float IBEPUShape.Mass
		{
			get { return this.Mass; }
			set { this.Mass = value; }
		}

		CollisionGroup IBEPUShape.CollisionGroup
		{
			get { return this.CollisionInformation.CollisionRules.Group; }
			set { this.CollisionInformation.CollisionRules.Group = value; }
		}

		public BEPUCapsuleShape(float radius, float height)
			: this(radius, height, Vector3.Zero)
		{
		}

		public BEPUCapsuleShape(float radius, float height, Vector3 position)
			: base(new BEPUutilities.Vector3(position.X, position.Y, position.Z), height - radius * 2, radius)
		{
		}

		Bounds IShape.GetBounds()
		{
			return new Bounds() { Min = new Vector3(0, 0, 0), Max = new Vector3(Radius, Length + Radius * 2, Radius) };
		}
	}
}
