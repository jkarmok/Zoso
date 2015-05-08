using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Events
{
	public class AddSession : DataContract
	{
		public AddSession(IRpcProtocol protocol, IEventData eventData)
			: base(protocol, eventData.Parameters)
		{
		}

		public AddSession()
		{
		}

		/// <summary>
		/// Session Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int SessionId { get; set; }

		/// <summary>
		/// Character Name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CharacterName)]
		public string CharacterName { get; set; }
		
		/// <summary>
		/// Is Transfer
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Condition)]
		public bool IsTransfer { get; set; }
	}
}
