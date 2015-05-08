using Photon.SocketServer.Rpc;
using Photon.SocketServer;

using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class MoveInventorySlot : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte)GameOperationCode.MoveInventorySlot;
			}
		}

		public MoveInventorySlot(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Index
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Index)]
		public byte IndexTo { get; set; }
		
		/// <summary>
		/// Index From
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.IndexFrom)]
		public byte IndexFrom { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
