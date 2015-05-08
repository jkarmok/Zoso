namespace Karen90MmoFramework.Game
{
	public struct SlotItemStructure
	{
		public byte Slot;
		public byte Count;

		public SlotItemStructure(byte slot, byte count)
		{
			this.Slot = slot;
			this.Count = count;
		}

		public static explicit operator short(SlotItemStructure info)
		{
			return (short) ((info.Slot << 8) | info.Count);
		}

		public static explicit operator SlotItemStructure(short rawValue)
		{
			return new SlotItemStructure((byte)((rawValue >> 8) & 0xFF), (byte)(rawValue & 0xFF));
		}
	}
}
