using System.Collections;
using Photon.SocketServer.Rpc;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ObjectPropertyMultiple : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte) ClientEventCode.ObjectPropertyMultiple;
			}
		}

		/// <summary>
		/// Object Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long ObjectId { get; set; }

		/// <summary>
		/// Properties
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Properties)]
		public Hashtable Properties { get; set; }

		/// <summary>
		/// Properties Revision
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Revision)]
		public int Revision { get; set; }
	}
}