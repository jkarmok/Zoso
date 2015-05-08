namespace Karen90MmoFramework.Game
{
	public struct CharacterStructure
	{
		public byte Race;
		public byte Origin;
		public byte Level;
		public string Name;

		public CharacterStructure(byte race, byte origin, byte level, string name)
		{
			this.Origin = origin;
			this.Race = race;
			this.Level = level;
			this.Name = name;
		}
	};
}
