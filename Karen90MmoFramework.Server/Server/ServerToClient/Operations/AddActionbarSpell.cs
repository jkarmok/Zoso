using Photon.SocketServer.Rpc;
using Photon.SocketServer;

using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class AddActionbarSpell : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte)GameOperationCode.AddActionbarSpell;
			}
		}

		public AddActionbarSpell(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Index
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Index)]
		public byte Index { get; set; }

		/// <summary>
		/// Spell Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SpellId)]
		public short SpellId { get; set; }
		
		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
