using BEPUphysics;
using BEPUphysics.CollisionRuleManagement;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	public interface IBEPUShape : IShape, ISpaceObject
	{
		float Mass { get; set; }
		CollisionGroup CollisionGroup { get; set; }
	}
}
