using Photon.SocketServer;

using Karen90MmoFramework.Server.Core;

namespace Karen90MmoFramework.Server.Chat
{
	public class ChatApplication : SubServerApplication
	{
		#region Constructors and Destructors

		public ChatApplication()
			: base(SubServerType.Chat)
		{
		}

		#endregion

		#region SubServer Overrides

		protected override OutgoingMasterServerPeer CreateOutgoingMasterPeer(InitResponse initResponse)
		{
			return new ChatServerPeer(initResponse, this);
		}

		protected override PeerBase CreatePeer(InitRequest initRequest)
		{
			return this.IsGameServer(initRequest) ? new IncomingChatListenerPeer(initRequest, this) : null;
		}

		#endregion
	}
}
