using System.Collections.Generic;

namespace Karen90MmoFramework.Server.Master
{
	public sealed class ClientCollection : Dictionary<int, MasterClientPeer>
	{
		#region Constants and Fields

		/// <summary>
		/// sync root to synchronize operation between threads
		/// </summary>
		private readonly object syncRoot = new object();

		#endregion

		#region Public Methods

		/// <summary>
		/// Call this when a client is connected
		/// </summary>
		public void OnConnect(MasterClientPeer peer)
		{
			lock (this.syncRoot)
			{
				MasterClientPeer existingPeer;
				if (TryGetValue(peer.ClientId, out existingPeer))
				{
					if (existingPeer == peer)
						return;

					existingPeer.Disconnect();
					this.Remove(existingPeer.ClientId);
				}

				this.Add(peer.ClientId, peer);
			}
		}

		/// <summary>
		/// Tries to retrieve a <see cref="MasterClientPeer"/>.
		/// </summary>
		public bool TryGetClient(int clientId, out MasterClientPeer peer)
		{
			lock(this.syncRoot)
			{
				return this.TryGetValue(clientId, out peer);
			}
		}

		/// <summary>
		/// Call this when a client is disconnected
		/// </summary>
		public void OnDisconnect(MasterClientPeer peer)
		{
			lock (this.syncRoot)
			{
				this.Remove(peer.ClientId);
			}
		}

		#endregion
	}
}
