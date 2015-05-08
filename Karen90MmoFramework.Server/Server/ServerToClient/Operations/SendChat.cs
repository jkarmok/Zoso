using Photon.SocketServer.Rpc;
using Photon.SocketServer;

using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class SendChat : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get { return (byte)GameOperationCode.SendChat; }
		}

		public SendChat(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}
		
		/// <summary>
		/// Message type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.MessageType)]
		public byte MessageType { get; set; }

		/// <summary>
		/// Message
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Message)]
		public string Message { get; set; }

		/// <summary>
		/// Message Receiver
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Receiver, IsOptional = true)]
		public string Receiver { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
