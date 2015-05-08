using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class LeaveChannel : Operation
	{
		public LeaveChannel(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public LeaveChannel()
		{
		}

		/// <summary>
		/// Session Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int SessionId { get; set; }

		/// <summary>
		/// Character Name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelId)]
		public int ChannelId { get; set; }
	}
}
