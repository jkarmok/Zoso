namespace Karen90MmoFramework.Quantum.Physics
{
	public struct CharacterControllerDescription
	{
		public string Name { get; set; }
		public object UserData { get; set; }
		public Vector3 Position { get; set; }
		public float Radius { get; set; }
		public float Height { get; set; }
		public float SlopeLimit { get; set; }
		public float StepOffset { get; set; }
		public float SkinWidth { get; set; }
	}
}
