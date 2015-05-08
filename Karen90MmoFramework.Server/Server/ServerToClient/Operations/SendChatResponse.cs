using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class SendChatResponse : GameOperationResponse
	{
		public SendChatResponse(byte operationCode)
			: base(operationCode)
		{
		}

		/// <summary>
		/// Chat message type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.MessageType)]
		public byte MessageType { get; set; }
	}
}
