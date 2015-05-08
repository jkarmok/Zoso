using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialFriendAddedNameMultiple : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialFriendAddedNameMultiple;
			}
		}
		
		/// <summary>
		/// Names
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public string[] Names { get; set; }
	}
}