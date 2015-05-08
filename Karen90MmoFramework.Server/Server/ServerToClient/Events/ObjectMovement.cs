using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ObjectMovement : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get { return (byte)ClientEventCode.ObjectMovement; }
		}

		/// <summary>
		/// Object Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long ObjectId { get; set; }

		/// <summary>
		/// Speed
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Speed, IsOptional = true)]
		public byte? Speed { get; set; }

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

		/// <summary>
		/// Movement state
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.MovementState, IsOptional = true)]
		public byte? State { get; set; }

		/// <summary>
		/// Movement direction
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.MovementDirection, IsOptional = true)]
		public byte? Direction { get; set; }
	}
}