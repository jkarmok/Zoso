using Photon.SocketServer.Rpc;
using Photon.SocketServer;

using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class DeleteCharacter : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte) GameOperationCode.DeleteCharacter;
			}
		}

		public DeleteCharacter(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Username
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Username)]
		public string Username { get; set; }

		/// <summary>
		/// Name of the character
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CharacterName)]
		public string CharacterName { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
