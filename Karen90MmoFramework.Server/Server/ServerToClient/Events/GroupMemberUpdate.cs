using System.Collections;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class GroupMemberUpdate : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.GroupMemberUpdate;
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
		[DataMember(Code = (byte)ParameterCode.Data)]
		public Hashtable Properties { get; set; }
	}
}