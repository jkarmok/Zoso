using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Events
{
	public class GroupFormed : DataContract
	{
		public GroupFormed(IRpcProtocol protocol, IEventData eventData)
			: base(protocol, eventData.Parameters)
		{
		}

		public GroupFormed()
		{
		}

		/// <summary>
		/// Group Guid
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Guid)]
		public long GroupGuid { get; set; }
		
		/// <summary>
		/// Leader Info
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Data)]
		public GroupMemberStructure LeaderInfo { get; set; }
	}
}
