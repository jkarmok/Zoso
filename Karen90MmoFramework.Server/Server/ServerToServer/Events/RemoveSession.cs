using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Events
{
	public class RemoveSession : DataContract
	{
		public RemoveSession(IRpcProtocol protocol, IEventData eventData)
			: base(protocol, eventData.Parameters)
		{
		}

		public RemoveSession()
		{
		}

		/// <summary>
		/// Session Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int SessionId { get; set; }
	}
}
