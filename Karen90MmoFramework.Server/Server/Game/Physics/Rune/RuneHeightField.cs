using DigitalRune.Geometry.Shapes;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneHeightField : HeightField, IHeightField, IRuneShape
	{
		public RuneHeightField(float widthX, float widthZ, float[,] heights)
			: base(widthX, widthZ, heights)
		{
		}

		Shape IRuneShape.Shape
		{
			get
			{
				return this;
			}
		}

		Bounds IShape.GetBounds()
		{
			var aabb = this.GetAabb();
			return new Bounds(aabb.Center.ToGameVector(), aabb.Extent.ToGameVector());
		}
	}
}
