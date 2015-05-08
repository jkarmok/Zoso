namespace Karen90MmoFramework.Server.Game.Systems
{
	public struct ItemRequirement
	{
		public short Id;
		public byte Count;

		public ItemRequirement(short id, byte count)
		{
			this.Id = id;
			this.Count = count;
		}
	}
}
