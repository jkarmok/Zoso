using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class AcquireServerResponse : DataContract
	{
		public AcquireServerResponse(IRpcProtocol protocol, OperationResponse response)
			: base(protocol, response.Parameters)
		{
		}

		public AcquireServerResponse()
		{
		}

		/// <summary>
		/// Sub server type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ServerType)]
		public byte ServerType { get; set; }

		/// <summary>
		/// Sub server IP
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Address)]
		public string Address { get; set; }

		/// <summary>
		/// Tcp Port
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.TcpPort)]
		public int TcpPort { get; set; }

		/// <summary>
		/// Udp Port
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.UdpPort)]
		public int UdpPort { get; set; }
	}
}
