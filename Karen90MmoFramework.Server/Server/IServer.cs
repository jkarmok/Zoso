using Photon.SocketServer;
using Karen90MmoFramework.Server.ServerToClient;

namespace Karen90MmoFramework.Server
{
	public interface IServer
	{
		/// <summary>
		/// Gets the <see cref="Photon.SocketServer.IRpcProtocol"/> for the client.
		/// </summary>
		IRpcProtocol Protocol { get; }

		/// <summary>
		/// Sends a(n) <see cref="GameOperationResponse"/> to a session
		/// </summary>
		void SendOperationResponse(int sessionId, GameOperationResponse response, MessageParameters parameters);

		/// <summary>
		/// Sends a(n) <see cref="GameEvent"/> to a session
		/// </summary>
		void SendEvent(int sessionId, GameEvent eventData, MessageParameters parameters);

		/// <summary>
		/// Kills the client of a session
		/// </summary>
		/// <param name="sessionId"></param>
		void KillClient(int sessionId);
	}
}
