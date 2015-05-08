using DigitalRune.Geometry.Shapes;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneBoxShape : BoxShape, IBoxShape, IRuneShape
	{
		Vector3 IBoxShape.Size
		{
			get
			{
				return new Vector3(WidthX, WidthY, WidthZ);
			}
		}

		Shape IRuneShape.Shape
		{
			get
			{
				return this;
			}
		}

		public RuneBoxShape(float widthX, float widthY, float widthZ) 
			: base(widthX, widthY, widthZ)
		{
		}

		Bounds IShape.GetBounds()
		{
			var aabb = this.GetAabb();
			return new Bounds(aabb.Center.ToGameVector(), aabb.Extent.ToGameVector());
		}
	}
}
