using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class AuraData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public short AuraId { get; set; }
		public byte AuraType { get; set; }
		public string Name { get; set; }
		public byte Level { get; set; }
		public short FamilyId { get; set; }
		public short? Duration { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(short auraId)
		{
			return "AURA/" + auraId;
		}
	}
}
