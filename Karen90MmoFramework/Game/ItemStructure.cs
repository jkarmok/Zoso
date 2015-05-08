namespace Karen90MmoFramework.Game
{
	public struct ItemStructure
	{
		public short ItemId;
		public byte Count;

		public ItemStructure(short itemId, byte count)
		{
			this.ItemId = itemId;
			this.Count = count;
		}
	}
}