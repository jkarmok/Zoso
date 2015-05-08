using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.Master
{
	internal class PingRequest : Operation
	{
		public PingRequest(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		/// <summary>
		/// Sent server time stamp
		/// </summary>
		[DataMember(Code = (byte)5, IsOptional = false)]
		public int ServerTimeStamp { get; set; }
	}
}
