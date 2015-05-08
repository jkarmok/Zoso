using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Events
{
	public class GroupDisbanded : DataContract
	{
		public GroupDisbanded(IRpcProtocol protocol, IEventData eventData)
			: base(protocol, eventData.Parameters)
		{
		}

		public GroupDisbanded()
		{
		}

		/// <summary>
		/// Group Guid
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Guid)]
		public long GroupGuid { get; set; }
	}
}
