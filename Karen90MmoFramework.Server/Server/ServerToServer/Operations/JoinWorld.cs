using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class JoinWorld : Operation
	{
		public JoinWorld(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public JoinWorld()
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
		public short WorldId { get; set; }
	}
}
