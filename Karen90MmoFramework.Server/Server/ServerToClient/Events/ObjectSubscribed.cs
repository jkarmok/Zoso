using System.Collections;
using Photon.SocketServer.Rpc;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ObjectSubscribed : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get { return (byte)ClientEventCode.ObjectSubscribed; }
		}

		/// <summary>
		/// Object Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long ObjectId { get; set; }

		/// <summary>
		/// Position
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Position)]
		public float[] Position { get; set; }

		/// <summary>
		/// Rotation
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Orientation, IsOptional = true)]
		public float? Orientation { get; set; }

		/// <summary>
		/// Properties
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Properties, IsOptional = true)]
		public Hashtable Properties { get; set; }

		/// <summary>
		/// Properties Revision
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Revision, IsOptional = true)]
		public int? Revision { get; set; }

		/// <summary>
		/// Flags
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Flags, IsOptional = true)]
		public int? Flags { get; set; }
	}
}