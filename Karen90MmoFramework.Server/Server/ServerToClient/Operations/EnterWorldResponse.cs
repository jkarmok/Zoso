using System.Collections;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class EnterWorldResponse : GameOperationResponse
	{
		public EnterWorldResponse(byte operationCode)
			: base(operationCode)
		{
		}

		/// <summary>
		/// Zone Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.WorldId)]
		public int WorldId { get; set; }

		/// <summary>
		/// Message of the day
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Message, IsOptional = true)]
		public string Motd { get; set; }

		/// <summary>
		/// Player Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long PlayerId { get; set; }

		/// <summary>
		/// Player position
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Position)]
		public float[] Position { get; set; }

		/// <summary>
		/// Player orientation
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Orientation)]
		public float Orientation { get; set; }
	}
}
