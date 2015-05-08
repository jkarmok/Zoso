namespace Karen90MmoFramework.Server.Game.Systems
{
	public struct NpcRequirement
	{
		public int Id;
		public byte Count;

		public NpcRequirement(int id, byte count)
		{
			this.Id = id;
			this.Count = count;
		}
	}
}
