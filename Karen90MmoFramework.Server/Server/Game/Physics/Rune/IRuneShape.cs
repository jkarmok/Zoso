using DigitalRune.Geometry.Shapes;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public interface IRuneShape : IShape
	{
		Shape Shape { get; }
	}
}
