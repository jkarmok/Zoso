using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class InventoryData : IDataField
	{
		public int Size { get; set; }
		public int[] Items { get; set; }
	}
}
