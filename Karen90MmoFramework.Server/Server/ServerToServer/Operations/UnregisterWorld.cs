using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class UnregisterWorld : Operation
	{
		public UnregisterWorld(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public UnregisterWorld()
		{
		}
		
		/// <summary>
		/// World Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.WorldId)]
		public short WorldId { get; set; }
	}
}
