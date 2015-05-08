using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class GroupInviteDeclined : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.GroupInviteDeclined;
			}
		}
		
		/// <summary>
		/// Invited
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Receiver)]
		public string Invited { get; set; }
	}
}