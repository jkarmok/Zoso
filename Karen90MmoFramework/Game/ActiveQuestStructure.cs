namespace Karen90MmoFramework.Game
{
	public struct ActiveQuestStructure
	{
		public short QuestId;
		public byte Status;
		public byte[] Counts;

		public ActiveQuestStructure(short questId, byte status, byte[] counts)
		{
			this.QuestId = questId;
			this.Status = status;
			this.Counts = counts;
		}
	}
}