using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialProfileUpdate : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialProfileUpdate;
			}
		}
		
		/// <summary>
		/// Profile Info
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Data)]
		public ProfileStructure ProfileInfo { get; set; }
	}
}