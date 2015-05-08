using DigitalRune.Geometry.Shapes;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneSphereShape : SphereShape, ISphereShape, IRuneShape
	{
		float ISphereShape.Radius
		{
			get
			{
				return this.Radius;
			}
		}

		Shape IRuneShape.Shape
		{
			get
			{
				return this;
			}
		}

		public RuneSphereShape(float radius)
			: base(radius)
		{
		}

		Bounds IShape.GetBounds()
		{
			var aabb = this.GetAabb();
			return new Bounds(aabb.Center.ToGameVector(), aabb.Extent.ToGameVector());
		}
	}
}
