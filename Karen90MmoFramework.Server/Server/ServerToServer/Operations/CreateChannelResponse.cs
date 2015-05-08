using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class CreateChannelResponse : DataContract
	{
		public CreateChannelResponse(IRpcProtocol protocol, OperationResponse response)
			: base(protocol, response.Parameters)
		{
		}

		public CreateChannelResponse()
		{
		}

		/// <summary>
		/// Channel Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelId)]
		public int ChannelId { get; set; }

		/// <summary>
		/// Callback Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CallbackId)]
		public long CallbackId { get; set; }
	}
}
