using Photon.SocketServer.Rpc;
using Photon.SocketServer;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class CreateCharacter : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte) GameOperationCode.CreateCharacter;
			}
		}

		public CreateCharacter(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Username
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Username)]
		public string Username { get; set; }

		/// <summary>
		/// Character Data
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Data)]
		public CharacterStructure CharacterData { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
