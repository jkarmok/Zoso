using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class KillClient : Operation
	{
		public KillClient(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public KillClient()
		{
		}

		/// <summary>
		/// Client Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int ClientId { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public OperationResponse GetResponse(short returnCode, string message)
		{
			return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = returnCode, DebugMessage = message };
		}
	}
}
