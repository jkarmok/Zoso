using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class LootItem : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte) GameOperationCode.LootItem;
			}
		}

		public LootItem(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}
		
		/// <summary>
		/// Purchase Info
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Index)]
		public byte Index { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
