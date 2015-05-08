namespace Karen90MmoFramework.Quantum.Geometry
{
	[System.Serializable]
	public struct SphereDescription
	{
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public Vector3 Scale { get; set; }
		public float Radius { get; set; }
	}
}
