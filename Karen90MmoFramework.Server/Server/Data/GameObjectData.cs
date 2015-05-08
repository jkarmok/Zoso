using System.Globalization;
using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class GameObjectData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public int Guid { get; set; }
		public string Name { get; set; }
		public byte GOType { get; set; }
		public short GroupId { get; set; }
		public short LootGroupId { get; set; }
		public float[] Position { get; set; }
		public float Orientation { get; set; }
		public int ZoneId { get; set; }

		public short[] StartQuests { get; set; }
		public short[] CompleteQuests { get; set; }

		/// <summary>
		/// Generates an id for this DataMember
		/// </summary>
		public static string GenerateId(short guid)
		{
			return "GO/" + guid;
		}
	}
}
