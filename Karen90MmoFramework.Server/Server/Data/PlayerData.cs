using System;
using System.Collections.Generic;

using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Server.Data
{
	/// <summary>
	/// Id = PLAYER/($PlayerName | Upper)
	/// </summary>
	public class PlayerData : CharacterData
	{
		public string Username { get; set; }
		public bool InitLogin { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime? LastPlayed { get; set; }

		public byte Origin { get; set; }
		public int Money { get; set; }
		public int Xp { get; set; }
		public int CurrHealth { get; set; }
		public int CurrMana { get; set; }

		public byte GmLevel { get; set; }
		
		public InventoryData Inventory { get; set; }
		public SpellsData Spells { get; set; }
		public ActionBarData ActionBar { get; set; }

		public short[] Stats { get; set; }

		public short[] FinishedQuests { get; set; }
		public Dictionary<short, QuestProgression> CurrentQuests { get; set; }

		public long? GroupGuid { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(int playerId)
		{
			return "PLAYER/" + playerId;
		}
	}
}
