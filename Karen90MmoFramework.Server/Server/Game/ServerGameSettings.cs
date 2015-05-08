namespace Karen90MmoFramework.Server.Game
{
	// TODO: Need to load it from a file
	public static class ServerGameSettings
	{
		// Game
		public const long GAME_FPS = 30;
		public const long GAME_UPDATE_INTERVAL = 1000 / GAME_FPS;

		// Movement
		public const int MAX_MOVEMENT_LATENCEY = 1000;

		public const int OBJECT_MOVEMENT_PUBLISH_INTERVAL = 2000;
		public const int OBJECT_ROTATION_CHECK_INTERVAL = 200;
		public const int PLAYER_INTEREST_MANAGEMENT_UPDATE_INTERVAL = 1000;
		public const int NPC_DESTINATION_CHECK_INTERVAL = 700;
		
		// Npc
		public const float BASE_AGGRO_RADIUS = 25.0f;
		public const float BASE_AGGRO_DROP_RADIUS = 45.0f;

		public const int AGGRO_ENTER_CHECK_INTERVAL_MS = 1 * 1000; // 1 sec
		public const int AGGRO_EXIT_CHECK_INTERVAL_MS = 2 * 1000; // 2 sec

		public const float THREAT_DAMAGE_MULTIPLIER = 0.1f;
		public const float THREAT_POWER_MULTIPLIER = 0.02f;

		public const float THREAT_FOR_AGGRO_ENTER = 0.1f;
		public const float THREAT_THRESHOLD_FOR_REWARD = 0.1f;

		// Loot
		public const int BASE_NPC_WAIT_TIME_AFTER_LOOTED_MS = 5 * 1000; // 5 sec
		public const int BASE_NPC_WAIT_TIME_UNTIL_LOOTED_MS = 60 * 1000; // 30 sec
		public const int BASE_NPC_RESPAWN_TIME_MS = 20 * 1000; // 60 sec

		public const int LOOT_CHEST_WAIT_TIME_BEFORE_LOOTED_MS = 60 * 1000; // 60 sec
		public const int LOOT_CHEST_WAIT_TIME_AFTER_LOOTED_MS = 5 * 1000; // 5 sec
		public const int LOOT_CHEST_RESPAWN_TIME_MS = 60 * 1000; // 60 sec

		public const int GATHER_WAIT_TIME_BEFORE_LOOTED_MS = 60 * 1000; // 60 sec
		public const int GATHER_WAIT_TIME_AFTER_LOOTED_MS = 5 * 1000; // 5 sec
		public const int GATHER_RESPAWN_TIME_MS = 60 * 1000; // 60 sec

		public const int MAX_ITEMS_PER_LOOT = 16;
		
		// Player
		public const float XP_MULTIPLIER_IN_GROUP = 0.8f;

		// Spell
		public const int MAX_SPELL_EFFECTS = 5;
		public const short USE_SPELL_ID = 0;
		
		// Quest
		public const int MAX_ACTIVE_QUESTS = 10;
	}
}
