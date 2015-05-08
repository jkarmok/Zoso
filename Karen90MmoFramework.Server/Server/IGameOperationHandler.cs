using Karen90MmoFramework.Server.ServerToClient;

namespace Karen90MmoFramework.Server
{
	public interface IGameOperationHandler
	{
		/// <summary>
		/// Handles an operation request
		/// </summary>
		void OnOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters);
	}
}
