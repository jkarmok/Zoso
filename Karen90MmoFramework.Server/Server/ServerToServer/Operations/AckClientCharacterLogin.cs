using Karen90MmoFramework.Rpc;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class AckClientCharacterLogin : Operation
	{
		public AckClientCharacterLogin(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public AckClientCharacterLogin()
		{
		}

		/// <summary>
		/// Session Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int SessionId { get; set; }

		/// <summary>
		/// World Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SubServerId)]
		public int ZoneId { get; set; }

		/// <summary>
		/// Character Name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CharacterName)]
		public string CharacterName { get; set; }

		/// <summary>
		/// Character Guid
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Guid)]
		public int Guid { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public OperationResponse GetResponse(short returnCode, string message)
		{
			return new OperationResponse(this.OperationRequest.OperationCode) { ReturnCode = returnCode, DebugMessage = message };
		}
	}
}
