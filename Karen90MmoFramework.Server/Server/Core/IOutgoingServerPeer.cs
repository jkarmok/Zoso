using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;

namespace Karen90MmoFramework.Server.Core
{
	public interface IOutgoingServerPeer
	{
		void OnConnect(S2SPeerBase serverPeerBase);
		void OnDisconnect(S2SPeerBase serverPeerBase);

		void OnOperationResponse(IRpcProtocol protocol, OperationResponse operationResponse, SendParameters sendParameters);
		void OnEvent(IRpcProtocol protocol, IEventData eventData, SendParameters sendParameters);
	}
}
