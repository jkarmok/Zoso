using DigitalRune.Geometry.Shapes;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneCapsuleShape : CapsuleShape, ICapsuleShape, IRuneShape
	{
		float ICapsuleShape.Radius
		{
			get
			{
				return this.Radius;
			}
		}

		float ICapsuleShape.Height
		{
			get
			{
				return this.Height;
			}
		}

		Shape IRuneShape.Shape
		{
			get
			{
				return this;
			}
		}

		public RuneCapsuleShape(float radius, float height)
			: base(radius, height)
		{
			
		}

		Bounds IShape.GetBounds()
		{
			var aabb = this.GetAabb();
			return new Bounds(aabb.Center.ToGameVector(), aabb.Extent.ToGameVector());
		}
	}
}
