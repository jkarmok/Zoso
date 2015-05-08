using System;
using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class UserData : IDataObject
	{
		public string Id { get; set; }
		
		public string Username { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }

		public bool IsBanned { get; set; }

		public DateTime CreatedOn { get; set; }
		public DateTime? LastLogin { get; set; }

		/// <summary>
		/// Generates an id for a(n) <see cref="Karen90MmoFramework.Database.IDataObject"/>.
		/// </summary>
		public static string GenerateId(string username)
		{
			return "USER/" + username.ToUpper();
		}
	}
}
