using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class GroupInviteReceived : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.GroupInviteReceived;
			}
		}
		
		/// <summary>
		/// Inviter
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Sender)]
		public string Inviter { get; set; }
	}
}