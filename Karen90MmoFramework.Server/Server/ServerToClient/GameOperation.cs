using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient
{
	public abstract class GameOperation : DataContract
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public abstract byte OperationCode { get; }

		protected GameOperation(IRpcProtocol protocol, GameOperationRequest operationRequest)
			: base(protocol, operationRequest.Parameters)
		{
		}

		/// <summary>
		/// Generates an error response
		/// </summary>
		public abstract GameOperationResponse GetErrorResponse(short errorReturnCode, string message);

		/// <summary>
		/// Generates an error response
		/// </summary>
		public GameOperationResponse GetErrorResponse(short errorReturnCode)
		{
			return GetErrorResponse(errorReturnCode, null);
		}
	}
}
