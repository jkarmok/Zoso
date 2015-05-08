using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ObjectTransform : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get { return (byte)ClientEventCode.ObjectTransform; }
		}

		/// <summary>
		/// Object Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId, IsOptional = true)]
		public long? ObjectId { get; set; }

		/// <summary>
		/// Position
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Position, IsOptional = true)]
		public float[] Position { get; set; }

		/// <summary>
		/// Orientation
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Orientation, IsOptional = true)]
		public float? Orientation { get; set; }

		/// <summary>
		/// Pitch
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Pitch, IsOptional = true)]
		public float? Pitch { get; set; }
	}
}
