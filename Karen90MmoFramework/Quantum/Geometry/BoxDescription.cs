namespace Karen90MmoFramework.Quantum.Geometry
{
	[System.Serializable]
	public struct BoxDescription
	{
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public Vector3 Scale { get; set; }
		public Vector3 Size { get; set; }
	}
}
