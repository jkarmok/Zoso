using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class CreateChannel : Operation
	{
		public CreateChannel(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public CreateChannel()
		{
		}

		/// <summary>
		/// Channel Type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelType)]
		public byte ChannelType { get; set; }
		
		/// <summary>
		/// Callback Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CallbackId)]
		public long CallbackId { get; set; }
		
		/// <summary>
		/// Channel Name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelName, IsOptional = true)]
		public string ChannelName { get; set; }

		/// <summary>
		/// Gets the <see cref="OperationResponse"/> with the result code.
		/// </summary>
		/// <param name="channelId"> </param>
		/// <param name="returnCode"></param>
		/// <returns></returns>
		public OperationResponse GetOperationResponse(int channelId, short returnCode)
		{
			var responseObject = new CreateChannelResponse
				{
					ChannelId = channelId,
					CallbackId = CallbackId
				};
			return new OperationResponse(OperationRequest.OperationCode, responseObject) {ReturnCode = returnCode};
		}
	}
}
