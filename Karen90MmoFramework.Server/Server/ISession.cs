using Karen90MmoFramework.Server.ServerToClient;

namespace Karen90MmoFramework.Server
{
	public interface ISession
	{
		/// <summary>
		/// Gets the session Id
		/// </summary>
		int SessionId { get; }

		/// <summary>
		/// Gets the character name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Queues an <see cref="GameOperationRequest"/> to be processed
		/// </summary>
		void ReceiveOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters);

		/// <summary>
		/// Destroys the session
		/// </summary>
		/// <param name="destroyReason"></param>
		void Destroy(DestroySessionReason destroyReason);
	}
}
