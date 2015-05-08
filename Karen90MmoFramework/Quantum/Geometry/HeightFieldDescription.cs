namespace Karen90MmoFramework.Quantum.Geometry
{
	public struct HeightFieldDescription
	{
		public int WidthX { get; set; }
		public int WidthZ { get; set; }
		public float[,] Heights { get; set; }
		public Vector3 Position { get; set; }
	}
}
