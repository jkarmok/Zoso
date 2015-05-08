using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class SpellsData : IDataField
	{
		public short[] ClientSpells { get; set; }
		public short[] UsableItems { get; set; }
	}
}
