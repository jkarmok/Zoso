using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities.Prefabs;

using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	public class BEPUBoxShape : Box, IRigidbody, IBoxShape, IBEPUShape
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

		Vector3 IBoxShape.Size
		{
			get
			{
				return new Vector3(Width, Height, Length);
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

		public BEPUBoxShape(Vector3 size)
			: this(size, Vector3.Zero)
		{
		}

		public BEPUBoxShape(Vector3 size, Vector3 position)
			: base(new BEPUutilities.Vector3(position.X, position.Y, position.Z), size.X, size.Y, size.Z)
		{
		}

		Bounds IShape.GetBounds()
		{
			return new Bounds() {Min = new Vector3(0, 0, 0), Max = new Vector3(HalfLength * 2, HalfHeight * 2, HalfWidth * 2)};
		}
	}
}
