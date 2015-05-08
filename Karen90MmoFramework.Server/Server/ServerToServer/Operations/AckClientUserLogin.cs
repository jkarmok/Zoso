using Karen90MmoFramework.Rpc;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class AckClientUserLogin : Operation
	{
		public AckClientUserLogin(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public AckClientUserLogin()
		{
		}

		/// <summary>
		/// Session Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int SessionId { get; set; }

		/// <summary>
		/// Username
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Username)]
		public string Username { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public OperationResponse GetResponse(short returnCode, string message)
		{
			return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = returnCode, DebugMessage = message };
		}
	}
}
