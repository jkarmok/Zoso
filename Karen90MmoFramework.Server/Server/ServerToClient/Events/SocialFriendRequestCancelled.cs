using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialFriendRequestCancelled : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialFriendRequestCancelled;
			}
		}
		
		/// <summary>
		/// Requester
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Sender)]
		public string Requester { get; set; }
	}
}