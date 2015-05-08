using System;
using System.Collections.Generic;

namespace Karen90MmoFramework.Server.Master
{
	public class Lobby : IDisposable
	{
		#region Constants and Fields

		private readonly ServerCollection subServers;

		private readonly ClientCollection clients;

		private readonly UserCollection users;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the sub server collection
		/// </summary>
		public ServerCollection SubServers
		{
			get
			{
				return this.subServers;
			}
		}

		/// <summary>
		/// Gets the clients
		/// </summary>
		public ClientCollection Clients
		{
			get
			{
				return this.clients;
			}
		}

		/// <summary>
		/// Gets the logged in users
		/// </summary>
		public UserCollection Users
		{
			get
			{
				return this.users;
			}
		}

		#endregion

		#region Constructors and Destructors

		public Lobby()
		{
			this.subServers = new ServerCollection();
			this.clients = new ClientCollection();
			this.users = new UserCollection();
		}

		#endregion

		#region Public Methods

		public void OnServerConnected(IncomingSubServerPeer peer)
		{
			this.subServers.OnConnect(peer);
		}

		public void OnServerDisconnected(IncomingSubServerPeer peer)
		{
			this.subServers.OnDisconnect(peer);
		}

		public void OnClientConnected(MasterClientPeer peer)
		{
			this.clients.OnConnect(peer);
		}

		public void OnClientDisconnected(MasterClientPeer peer)
		{
			this.clients.OnDisconnect(peer);
		}

		public void Dispose()
		{
			var clientPeers = new Dictionary<int, MasterClientPeer>(this.clients);
			foreach (var client in clientPeers.Values)
			{
				client.Disconnect();
			}

			this.clients.Clear();

			var servers = new Dictionary<int, IncomingSubServerPeer>(this.subServers);
			foreach (var server in servers.Values)
			{
				server.Disconnect();
			}

			this.subServers.Clear();
		}

		#endregion
	}
}
