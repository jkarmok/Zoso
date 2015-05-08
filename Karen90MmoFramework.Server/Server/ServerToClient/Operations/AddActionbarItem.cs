using Photon.SocketServer.Rpc;
using Photon.SocketServer;

using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class AddActionbarItem : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte)GameOperationCode.AddActionbarItem;
			}
		}

		public AddActionbarItem(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Index
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Index)]
		public byte Index { get; set; }

		/// <summary>
		/// Item Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ItemId)]
		public short ItemId { get; set; }
		
		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
