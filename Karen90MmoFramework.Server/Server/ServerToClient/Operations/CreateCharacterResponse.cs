using Photon.SocketServer.Rpc;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class CreateCharacterResponse : GameOperationResponse
	{
		public CreateCharacterResponse(byte operationCode)
			: base(operationCode)
		{
		}

		/// <summary>
		/// Character Data
		/// </summary>
		[DataMember(Code=(byte)ParameterCode.Data)]
		public CharacterStructure CharacterData { get; set; } 
	}
}
