namespace Karen90MmoFramework.Game
{
	public struct ProfileStructure
	{
		public byte Level;
		public byte OnlineStatus;
		public short WorldId;
		public string Name;

		public ProfileStructure(string name, byte level, short worldId, byte onlineStatus)
		{
			this.Name = name;
			this.OnlineStatus = onlineStatus;
			this.Level = level;
			this.WorldId = worldId;
		}
	}
}