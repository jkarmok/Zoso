using System.Globalization;

namespace Karen90MmoFramework.Server.Data
{
	public class NpcData : CharacterData
	{
		/// <summary>
		/// Group Id
		/// </summary>
		public short GroupId { get; set; }

		/// <summary>
		/// Npc Type
		/// </summary>
		public byte NpcType { get; set; }

		/// <summary>
		/// Alignment
		/// </summary>
		public byte Alignment { get; set; }

		/// <summary>
		/// Gets the max health
		/// </summary>
		public int MaxHealth { get; set; }

		/// <summary>
		/// Gets the max mana
		/// </summary>
		public int MaxMana { get; set; }

		public short LootGroupId { get; set; }

		public short[] Items { get; set; }
		public short[] StartQuests { get; set; }
		public short[] CompleteQuests { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(int npcId)
		{
			return "NPC/" + npcId;
		}
	}
}
