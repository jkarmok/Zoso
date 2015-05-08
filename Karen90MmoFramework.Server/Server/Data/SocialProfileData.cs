using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class SocialProfileData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public string Name { get; set; }
		public int Guid { get; set; }
		public byte Level { get; set; }

		public byte GmLevel { get; set; }

		public string[] Friends { get; set; }
		public string[] Ignores { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(int guid)
		{
			return "PROFILE/" + guid;
		}
	}
}
