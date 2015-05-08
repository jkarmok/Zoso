using Karen90MmoFramework.Database;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Server.Data
{
	public class QuestData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public short QuestId { get; set; }
		public string Name { get; set; }
		public QuestFlags Flags { get; set; }
		public short Level { get; set; }

		public short NextQuest { get; set; }
		public short PrevQuest { get; set; }

		// Requirements
		public ItemRequirement[] ItemRequirements { get; set; }
		public NpcRequirement[] NpcRequirements { get; set; } // count == 0 is for talk
		public byte PlayerRequirements { get; set; }
		
		// Rewards
		public int RewardXp { get; set; }
		public int RewardMoney { get; set; }
		public int RewardRenown { get; set; }
		public short[] RewardSpells { get; set; }
		public ItemStructure[] RewardItems { get; set; }
		public ItemStructure[] RewardOptionalItems { get; set; }

		// Conversations
		public ConversationStructure[] QuestConversations { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(short questId)
		{
			return "QUEST/" + questId;
		}
	}
}
