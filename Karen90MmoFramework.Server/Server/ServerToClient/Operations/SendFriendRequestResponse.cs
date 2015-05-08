using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class SendFriendRequestResponse : GameOperationResponse
	{
		public SendFriendRequestResponse(byte operationCode)
			: base(operationCode)
		{
		}

		/// <summary>
		/// Character name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CharacterName)]
		public string CharacterName { get; set; }
	}
}
