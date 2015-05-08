using System.Collections.Generic;
using System.Linq;

namespace Karen90MmoFramework.Server.Master
{
	public sealed class ServerCollection : Dictionary<int, IncomingSubServerPeer>
	{
		#region Constants and Fields

		/// <summary>
		/// sync root to synchronize operation between threads
		/// </summary>
		private readonly object syncRoot = new object();

		#endregion

		#region Properties

		/// <summary>
		/// Gets the login server
		/// </summary>
		public IncomingSubServerPeer LoginServer { get; private set; }

		/// <summary>
		/// Gets the chat server
		/// </summary>
		public IncomingSubServerPeer ChatServer { get; private set; }

		/// <summary>
		/// Gets the social server
		/// </summary>
		public IncomingSubServerPeer SocialServer { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Call this when a server is connected and registered
		/// </summary>
		public void OnConnect(IncomingSubServerPeer peer)
		{
			lock (syncRoot)
			{
				// no server id means the server has not been registered (yet) so we can return
				if (!peer.ServerId.HasValue)
					return;

				IncomingSubServerPeer existingPeer;
				// if an exisiting peer exisits, disconnect it
				if (this.TryGetValue(peer.ServerId.Value, out existingPeer))
				{
					// if both peers are the same return (the peer trying to add itself twice?)
					if (peer == existingPeer)
						return;
					// disconnect the existing peer
					existingPeer.Disconnect();
					// this will remove it from the collection
					this.OnDisconnect(existingPeer);
				}

				// setting the login server
				if((peer.ServerType & SubServerType.Login) == SubServerType.Login)
				{
					var loginServer = LoginServer;
					if(loginServer != null)
					{
						// if the current login server isn't a dedicated login server
						// and the connected server is a dedicated login server replace the current server
						if(peer.ServerType == SubServerType.Login)
						{
							// if the server that was connected is also a dedicated login server disconnect the old server
							if (loginServer.ServerType == SubServerType.Login)
								loginServer.Disconnect();

							this.LoginServer = peer;
						}
					}
					else
					{
						this.LoginServer = peer;
					}
				}

				// setting the chat server
				if ((peer.ServerType & SubServerType.Chat) == SubServerType.Chat)
				{
					var chatServer = ChatServer;
					if (chatServer != null)
					{
						// if the current chat server isn't a dedicated chat server
						// and the connected server is a dedicated chat server replace the current server
						if (peer.ServerType == SubServerType.Chat)
						{
							// if the server that was connected is also a dedicated chat server disconnect the old server
							if (chatServer.ServerType == SubServerType.Chat)
								chatServer.Disconnect();

							this.ChatServer = peer;
						}
					}
					else
					{
						this.ChatServer = peer;
					}
				}

				// setting the social server
				if ((peer.ServerType & SubServerType.Social) == SubServerType.Social)
				{
					var socialServer = SocialServer;
					if (socialServer != null)
					{
						// if the current social server isn't a dedicated social server
						// and the connected server is a dedicated social server replace the current server
						if (peer.ServerType == SubServerType.Social)
						{
							// if the server that was connected is also a dedicated social server disconnect the old server
							if (socialServer.ServerType == SubServerType.Social)
								socialServer.Disconnect();

							this.SocialServer = peer;
						}
					}
					else
					{
						// TODO: Connect all players to this new social server
						this.SocialServer = peer;
					}
				}

				// add the server to our collection
				this.Add(peer.ServerId.Value, peer);
			}
		}

		/// <summary>
		/// Call this when a server disconnects
		/// </summary>
		public void OnDisconnect(IncomingSubServerPeer peer)
		{
			lock (syncRoot)
			{
				// no server id means the server has not been registered (yet) so we can return
				if (!peer.ServerId.HasValue)
					return;

				// finding a replacement login server
				if (peer == LoginServer)
				{
					this.LoginServer = null;
					// TODO: Get the one with less load
					// searching for a pure login server if none found, find a temporary server which is partly a login server
					var replacementPeer = this.Values.FirstOrDefault(server => (server.ServerType == SubServerType.Login)) ??
					                      this.Values.FirstOrDefault(server => (server.ServerType & SubServerType.Login) == SubServerType.Login);
					if (replacementPeer != null)
						this.LoginServer = replacementPeer;
				}

				// finding a replacement chat server
				if (peer == ChatServer)
				{
					this.ChatServer = null;
					// TODO: Get the one with less load
					// searching for a pure chat server if none found, find a temporary server which is partly a chat server
					var replacementPeer = this.Values.FirstOrDefault(server => (server.ServerType == SubServerType.Chat)) ??
										  this.Values.FirstOrDefault(server => (server.ServerType & SubServerType.Chat) == SubServerType.Chat);
					if (replacementPeer != null)
						this.ChatServer = replacementPeer;
				}

				// finding a replacement social server
				if (peer == SocialServer)
				{
					this.SocialServer = null;
					// TODO: Get the one with less load
					// searching for a pure social server if none found, find a temporary server which is partly a social server
					var replacementPeer = this.Values.FirstOrDefault(server => (server.ServerType == SubServerType.Social)) ??
										  this.Values.FirstOrDefault(server => (server.ServerType & SubServerType.Social) == SubServerType.Social);
					if (replacementPeer != null)
						this.SocialServer = replacementPeer;
				}

				this.Remove(peer.ServerId.Value);
			}
		}

		/// <summary>
		/// Tries to get any <see cref="Karen90MmoFramework.Server.Master.IncomingSubServerPeer"/> by its server type.
		/// </summary>
		/// <returns>Returns true if found and false otherwise</returns>
		public bool TryGetSubserverByTypeAny(SubServerType serverType, out IncomingSubServerPeer peer)
		{
			lock (syncRoot)
				return (peer = this.Values.FirstOrDefault(server => (server.ServerType & serverType) == serverType)) != null;
		}

		/// <summary>
		/// Tries to get any <see cref="Karen90MmoFramework.Server.Master.IncomingSubServerPeer"/> by its server type.
		/// </summary>
		/// <returns>Returns true if found and false otherwise</returns>
		public bool TryGetSubserverByTypeAny(SubServerType serverType, int subServerId, out IncomingSubServerPeer peer)
		{
			lock (syncRoot)
				return (peer = this.Values.FirstOrDefault(server => (server.ServerType & serverType) == serverType && server.SubServerId == subServerId)) != null;
		}

		/// <summary>
		/// Tries to get a(n) <see cref="Karen90MmoFramework.Server.Master.IncomingSubServerPeer"/> by its server type.
		/// </summary>
		/// <returns>Returns true if found and false otherwise</returns>
		public bool TryGetSubserverByType(SubServerType serverType, out IncomingSubServerPeer peer)
		{
			lock (syncRoot)
				return (peer = this.Values.FirstOrDefault(server => server.ServerType == serverType)) != null;
		}

		/// <summary>
		/// Tries to get a(n) <see cref="Karen90MmoFramework.Server.Master.IncomingSubServerPeer"/> by its server type.
		/// </summary>
		/// <returns>Returns true if found and false otherwise</returns>
		public bool TryGetSubserverByType(SubServerType serverType, int subServerId, out IncomingSubServerPeer peer)
		{
			lock (syncRoot)
				return (peer = this.Values.FirstOrDefault(server => server.ServerType == serverType && server.SubServerId == subServerId)) != null;
		}

		/// <summary>
		/// Gets all the <see cref="Karen90MmoFramework.Server.Master.IncomingSubServerPeer"/> of a specific type
		/// </summary>
		/// <param name="subServerType"></param>
		/// <returns></returns>
		public IEnumerable<IncomingSubServerPeer> GetSubServersOfType(SubServerType subServerType)
		{
			lock (syncRoot)
				return Values.Where(server => server.ServerType == subServerType).ToArray();
		}

		#endregion
	}
}
