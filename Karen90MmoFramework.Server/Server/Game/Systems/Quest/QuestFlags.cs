namespace Karen90MmoFramework.Server.Game.Systems
{
	[System.Flags]
	public enum QuestFlags
	{
		QUEST_FLAG_NONE					= 0x00000000,
		QUEST_FLAG_TALK					= 0x00000001,
		QUEST_FLAG_DELIVER				= 0x00000002,
		QUEST_FLAG_KILL					= 0x00000004,
		QUEST_FLAG_STAY_ALIVE			= 0x00000008,
		QUEST_FLAG_KEEP_ALIVE			= 0x00000010,
		QUEST_FLAG_GOTO_LOCATION		= 0x00000020,
		QUEST_FLAG_TIMED				= 0x00000040,

		QUEST_FLAG_COMPLETE_ON_PICKUP	= 0x00000080,		// automatically completes the quest on pickup
		QUEST_FLAG_AUTO_REWARD			= 0x00000100,		// automacially rewards on complete. used for quests which have multiple steps
		QUEST_FLAG_AUTO_START_NEXT		= 0x00000200,		// automatically starts the next quest
	};
}
