using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class AcquireServerAddress : Operation
	{
		public AcquireServerAddress(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public AcquireServerAddress()
		{
		}

		/// <summary>
		/// Sub server type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ServerType)]
		public byte ServerType { get; set; }

		/// <summary>
		/// Sub server type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SubServerId, IsOptional = true)]
		public int? SubServerId { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public OperationResponse GetResponse(short returnCode, string message)
		{
			return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = returnCode, DebugMessage = message };
		}
	}
}
