namespace Karen90MmoFramework.Game
{
	[System.Flags]
	public enum InterestFlags
	{
		None				= 0x00,

		QuestInProgress		= 0x01 | Quest,
		QuestActive			= 0x02 | Quest,
		QuestTurnIn			= 0x04 | Quest,

		Quest				= 0x08,
		Loot				= 0x10,
		Conversation		= 0x20,
		Shopping			= 0x40,

		Usable				= 0x80,
		Gatherable			= 0x100,
		Collectible			= 0x200,
	}
}
