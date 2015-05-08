namespace Karen90MmoFramework.Game
{
	public struct ContainerItemStructure
	{
		public short ItemId;
		public byte Index;
		public byte Count;

		public ContainerItemStructure(short itemId, byte index, byte count)
		{
			this.ItemId = itemId;
			this.Index = index;
			this.Count = count;
		}

		public static explicit operator int(ContainerItemStructure info)
		{
			return ((info.Index << 24) | (info.Count << 16) | (ushort)info.ItemId);
		}

		public static explicit operator ContainerItemStructure(int rawValue)
		{
			return new ContainerItemStructure((short)(rawValue & 0xFFFF), (byte)((rawValue >> 24) & 0xFF), (byte)((rawValue >> 16) & 0xFF));
		}
	}
}