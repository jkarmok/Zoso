namespace Karen90MmoFramework.Game
{
	public static class Formulas
	{
		// ======================== XP =========================== //

		const int LevelToXpLt10MultiplierB1 = 230;
		const int LevelToXpLt10MultiplierB2 = 64;
		const int LevelToXpLt10MultiplierB3 = 27;

		const int LevelToXpLt20MultiplierB1 = 340;
		const int LevelToXpLt20MultiplierB2 = 82;
		const int LevelToXpLt20MultiplierB3 = 33;

		const int LevelToXpLt30MultiplierB1 = 550;
		const int LevelToXpLt30MultiplierB2 = 88;
		const int LevelToXpLt30MultiplierB3 = 41;

		const int LevelToXpLt40MultiplierB1 = 860;
		const int LevelToXpLt40MultiplierB2 = 140;
		const int LevelToXpLt40MultiplierB3 = 51;

		/// <summary>
		/// Gets the Max Xp for the level.
		/// </summary>
		public static int GetMaxXp(int level)
		{
			if (level < 1)
				return 0;

			if (level < 10)
				return LevelToXpLt10MultiplierB1 * level + LevelToXpLt10MultiplierB2 * (level - 1) + LevelToXpLt10MultiplierB3 * (level - 2);

			if (level < 20)
				return LevelToXpLt20MultiplierB1 * level + LevelToXpLt20MultiplierB2 * (level - 1) + LevelToXpLt20MultiplierB3 * (level - 2);

			if (level < 30)
				return LevelToXpLt30MultiplierB1 * level + LevelToXpLt30MultiplierB2 * (level - 1) + LevelToXpLt30MultiplierB3 * (level - 2);

			if (level <= GlobalGameSettings.MAX_PLAYER_LEVEL)
				return LevelToXpLt40MultiplierB1 * level + LevelToXpLt40MultiplierB2 * (level - 1) + LevelToXpLt40MultiplierB3 * (level - 2);

			return 0;
		}

		// ============================= XP GAIN =============================== //

		const int LevelToXpBaseGainLt10 = 11;
		const int LevelToXpBaseGainLt20 = 16;
		const int LevelToXpBaseGainLt30 = 21;
		const int LevelToXpBaseGainLte40 = 32;
		const int LevelToXpBaseGainLte45 = 45;

		/// <summary>
		/// Gets the Xp gain for Npc level
		/// </summary>
		public static int GetXpGainForKill(int playerLevel, int npcLevel)
		{
			if (npcLevel < 1 || playerLevel < 1)
				return 0;

			var baseXp = 0;
			if (npcLevel < 10)
				baseXp = LevelToXpBaseGainLt10;
			else if (npcLevel < 20)
				baseXp = LevelToXpBaseGainLt20;
			else if (npcLevel < 30)
				baseXp = LevelToXpBaseGainLt30;
			else if (npcLevel <= 40)
				baseXp = LevelToXpBaseGainLte40;
			else if (npcLevel <= GlobalGameSettings.MAX_NPC_LEVEL)
				baseXp = LevelToXpBaseGainLte45;

			var lvlDiff = npcLevel - playerLevel;
			if (lvlDiff > 0)
			{
				if (lvlDiff > 3)
					lvlDiff = 3;

				return baseXp + playerLevel * 4 + (lvlDiff * (npcLevel / 10 + 2));
			}

			if (lvlDiff < -5)
				return 0;

			return baseXp + playerLevel * 3 + (lvlDiff * (npcLevel / 10 + 1));
		}
	}
}
