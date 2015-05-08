using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public static class QuestHelper
	{
		public static ActiveQuestStructure ToActiveQuestInfo(this QuestProgression activeQuest, short questId)
		{
			var length = 0;
			if (activeQuest.ItemCount != null)
				length += activeQuest.ItemCount.Length;

			if (activeQuest.NpcCount != null)
				length += activeQuest.NpcCount.Length;

			if (activeQuest.PlayerCount > 0)
				length++;

			var counts = new byte[length]; // 1 + len(count)
			var status = (byte) activeQuest.Status;

			int index = 0;
			if (activeQuest.ItemCount != null)
			{
				activeQuest.ItemCount.CopyTo(counts, index);
				index += activeQuest.ItemCount.Length;
			}

			if (activeQuest.NpcCount != null)
			{
				activeQuest.NpcCount.CopyTo(counts, index);
				index += activeQuest.NpcCount.Length;
			}

			if (activeQuest.PlayerCount > 0)
				counts[index] = activeQuest.PlayerCount;

			return new ActiveQuestStructure(questId, status, counts);
		}
	}
}
