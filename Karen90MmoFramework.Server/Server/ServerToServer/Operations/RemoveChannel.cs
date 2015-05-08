using Karen90MmoFramework.Rpc;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class RemoveChannel : Operation
	{
		public RemoveChannel(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public RemoveChannel()
		{
		}

		/// <summary>
		/// Chat Channel Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelId)]
		public int ChannelId { get; set; }
	}
}
