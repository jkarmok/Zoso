using Karen90MmoFramework.Quantum.Geometry;

namespace Karen90MmoFramework
{
	[System.Serializable]
	public class Colliders
	{
		public BoxDescription[] BoxColliders { get; set; }
		public SphereDescription[] SphereColliders { get; set; }
		public CapsuleDescription[] CapsuleColliders { get; set; }
		public CylinderDescription[] CylinderColliders { get; set; }
	}
}
