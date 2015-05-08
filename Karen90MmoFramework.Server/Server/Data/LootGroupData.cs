using System.Globalization;
using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public struct LootItemData
	{
		public short ItemId { get; set; }
		public byte MinCount { get; set; }
		public byte MaxCount { get; set; }
		public float DropChance { get; set; }
	}

	public class LootGroupData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public short GroupId { get; set; }
		public LootItemData[] LootItems { get; set; }
		public int MinGold { get; set; }
		public int MaxGold { get; set; }
		public float GoldChance { get; set; }
		
		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(short groupId)
		{
			return "LOOTGROUP/" + groupId.ToString(CultureInfo.InvariantCulture);
		}
	}
}
