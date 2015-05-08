using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ObjectProperty : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte) ClientEventCode.ObjectProperty;
			}
		}

		/// <summary>
		/// Object Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId, IsOptional = true)]
		public long? ObjectId { get; set; }

		/// <summary>
		/// Properties Code
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.PropertyCode)]
		public byte PropertiesCode { get; set; }

		/// <summary>
		/// Event Data
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Data)]
		public object EventData { get; set; }
	}
}