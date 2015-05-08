using Photon.SocketServer;
using Photon.SocketServer.Rpc;

using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class GetProperties : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte) GameOperationCode.GetProperties;
			}
		}

		public GetProperties(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}
		
		/// <summary>
		/// Item Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long ObjectId { get; set; }

		/// <summary>
		/// Flags
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Flags)]
		public int Flags { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
