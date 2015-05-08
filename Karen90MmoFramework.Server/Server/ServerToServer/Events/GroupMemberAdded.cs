using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Events
{
	public class GroupMemberAdded : DataContract
	{
		public GroupMemberAdded(IRpcProtocol protocol, IEventData eventData)
			: base(protocol, eventData.Parameters)
		{
		}

		public GroupMemberAdded()
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
		[DataMember(Code = (byte)ParameterCode.Data)]
		public GroupMemberStructure MemberInfo { get; set; }
	}
}
