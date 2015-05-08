using System;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class RegisterServer : Operation
	{
		public RegisterServer(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public RegisterServer()
		{
		}
		
		/// <summary>
		/// Server Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ServerId)]
		public int ServerId { get; set; }

		/// <summary>
		/// Sub server id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SubServerId)]
		public byte SubServerId { get; set; }

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

		/// <summary>
		/// Generates an error response
		/// </summary>
		public OperationResponse GetResponse(short returnCode)
		{
			return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = returnCode };
		}

		/// <summary>
		/// Generates an error response
		/// </summary>
		public OperationResponse GetResponse(short returnCode, string debugMessage)
		{
			return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = returnCode, DebugMessage = debugMessage};
		}
	}
}
