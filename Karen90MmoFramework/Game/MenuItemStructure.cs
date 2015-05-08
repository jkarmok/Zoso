namespace Karen90MmoFramework.Game
{
	public struct MenuItemStructure
	{
		public short ItemId;
		public byte ItemType;
		public byte IconType;

		public MenuItemStructure(short itemId, byte itemType, byte iconType)
		{
			this.ItemId = itemId;
			this.ItemType = itemType;
			this.IconType = iconType;
		}

		public static explicit operator int(MenuItemStructure info)
		{
			return ((info.ItemId << 16) | (info.ItemType << 8) | info.IconType);
		}

		public static explicit operator MenuItemStructure(int rawValue)
		{
			return new MenuItemStructure((short)((rawValue >> 16) & 0xFFFF), (byte)((rawValue >> 8) & 0xFF), (byte)(rawValue & 0xFF));
		}
	}
}
