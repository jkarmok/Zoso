namespace Karen90MmoFramework.Game
{
	public struct ActionItemStructure
	{
		public short ItemId;
		public byte Type;
		public byte Index;

		public ActionItemStructure(short itemId, byte type, byte index)
		{
			this.ItemId = itemId;
			this.Type = type;
			this.Index = index;
		}

		public static explicit operator int(ActionItemStructure info)
		{
			return ((info.Type << 24) | (info.Index << 16) | (ushort)info.ItemId);
		}

		public static explicit operator ActionItemStructure(int rawValue)
		{
			return new ActionItemStructure((short)(rawValue & 0xFFFF), (byte)((rawValue >> 24) & 0xFF), (byte)((rawValue >> 16) & 0xFF));
		}
	}
}