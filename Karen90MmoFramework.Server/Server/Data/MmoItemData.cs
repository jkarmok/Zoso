using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class GameItemData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public short ItemId { get; set; }
		public byte ItemType { get; set; }
		public short Level { get; set; }
		public byte Rarity { get; set; }

		public int BuyoutPrice { get; set; }
		public int SellPrice { get; set; }
		
		public int MaxStack { get; set; }
		public bool IsTradable { get; set; }
		public short UseSpellId { get; set; }
		public byte UseLimit { get; set; }

		public string Name { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(short itemId)
		{
			return "ITEM/" + itemId;
		}
	}
}
