using DigitalRune.Geometry.Shapes;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneCylinderShape : CylinderShape, ICylinderShape, IRuneShape
	{
		float ICylinderShape.Radius
		{
			get
			{
				return this.Radius;
			}
		}

		float ICylinderShape.Length
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

		public RuneCylinderShape(float radius, float length)
			: base(radius, length)
		{
		}

		Bounds IShape.GetBounds()
		{
			var aabb = this.GetAabb();
			return new Bounds(aabb.Center.ToGameVector(), aabb.Extent.ToGameVector());
		}
	}
}
