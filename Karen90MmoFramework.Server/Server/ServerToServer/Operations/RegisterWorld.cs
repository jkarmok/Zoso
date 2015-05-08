using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class RegisterWorld : Operation
	{
		public RegisterWorld(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public RegisterWorld()
		{
		}
		
		/// <summary>
		/// World Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.WorldId)]
		public short WorldId { get; set; }
	}
}
