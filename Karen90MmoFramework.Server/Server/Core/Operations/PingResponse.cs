using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.Core.Operations
{
	internal class PingResponse : DataContract
	{
		public PingResponse(IRpcProtocol protocol, OperationResponse response)
			: base(protocol, response.Parameters)
		{
		}

		/// <summary>
		/// Master server time stamp
		/// </summary>
		[DataMember(Code = (byte)4, IsOptional = false)]
		public int MasterTimeStamp { get; set; }

		/// <summary>
		/// Sent server time stamp
		/// </summary>
		[DataMember(Code = (byte)5, IsOptional = false)]
		public int ServerTimeStamp { get; set; }
	}
}
