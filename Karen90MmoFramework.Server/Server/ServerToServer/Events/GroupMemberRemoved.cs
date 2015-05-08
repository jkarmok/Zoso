using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Events
{
	public class GroupMemberRemoved : DataContract
	{
		public GroupMemberRemoved(IRpcProtocol protocol, IEventData eventData)
			: base(protocol, eventData.Parameters)
		{
		}

		public GroupMemberRemoved()
		{
		}

		/// <summary>
		/// Group Guid
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Guid)]
		public long GroupGuid { get; set; }
		
		/// <summary>
		/// Member Info
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long MemberGuid { get; set; }
	}
}
