using Karen90MmoFramework.Server.ServerToClient;

namespace Karen90MmoFramework.Server
{
	public interface IPeer
	{
		/// <summary>
		/// Sends an operation response
		/// </summary>
		void SendOperationResponse(GameOperationResponse response, MessageParameters parameters);

		/// <summary>
		/// Sends an event
		/// </summary>
		void SendEvent(GameEvent eventData, MessageParameters parameters);

		/// <summary>
		/// Disconnects the player
		/// </summary>
		void Disconnect();
	}
}
