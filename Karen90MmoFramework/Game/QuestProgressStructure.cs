namespace Karen90MmoFramework.Game
{
	public struct QuestProgressStructure
	{
		public short QuestId;
		public byte Index;
		public byte Count;

		public QuestProgressStructure(short questId, byte index, byte count)
		{
			this.QuestId = questId;
			this.Index = index;
			this.Count = count;
		}

		public static explicit operator int(QuestProgressStructure info)
		{
			return ((info.Index << 24) | (info.Count << 16) | (ushort)info.QuestId);
		}

		public static explicit operator QuestProgressStructure(int rawValue)
		{
			return new QuestProgressStructure((short)(rawValue & 0xFFFF), (byte)((rawValue >> 24) & 0xFF), (byte)((rawValue >> 16) & 0xFF));
		}
	}
}