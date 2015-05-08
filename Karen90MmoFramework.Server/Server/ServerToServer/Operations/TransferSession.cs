using Karen90MmoFramework.Rpc;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class TransferSession : Operation
	{
		public TransferSession(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public TransferSession()
		{
		}

		/// <summary>
		/// Session Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int SessionId { get; set; }

		/// <summary>
		/// World Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SubServerId)]
		public int WorldId { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public OperationResponse GetResponse(short returnCode, string message)
		{
			return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = returnCode, DebugMessage = message };
		}
	}
}
