using System.Globalization;
using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class GroupData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or Sets the group Guid
		/// </summary>
		public long Guid { get; set; }

		/// <summary>
		/// Gets or Sets the leader
		/// </summary>
		public string Leader { get; set; }

		/// <summary>
		/// Gets or Sets the members
		/// </summary>
		public string[] Members { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(long guid)
		{
			return "GROUP/" + guid.ToString(CultureInfo.InvariantCulture);
		}
	}
}
