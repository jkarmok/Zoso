using System;
using System.Collections.Generic;

namespace Karen90MmoFramework.Server.Master
{
	public sealed class UserCollection
	{
		#region Constants and Fields

		/// <summary>
		/// sync root to synchronize operation between threads
		/// </summary>
		private readonly object syncRoot = new object();

		/// <summary>
		/// contains player clients
		/// </summary>
		private readonly Dictionary<string, MasterClientPeer> collection = new Dictionary<string, MasterClientPeer>(StringComparer.CurrentCultureIgnoreCase);

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a user
		/// </summary>
		public void Add(string username, MasterClientPeer peer)
		{
			lock (this.syncRoot)
				this.collection.Add(username, peer);
		}

		/// <summary>
		/// Tries to retrieve a User
		/// </summary>
		public bool TryGetValue(string username, out MasterClientPeer peer)
		{
			lock (this.syncRoot)
				return this.collection.TryGetValue(username, out peer);
		}

		/// <summary>
		/// Removes a user
		/// </summary>
		public void Remove(string username)
		{
			lock (this.syncRoot)
				this.collection.Remove(username);
		}

		#endregion
	}
}
