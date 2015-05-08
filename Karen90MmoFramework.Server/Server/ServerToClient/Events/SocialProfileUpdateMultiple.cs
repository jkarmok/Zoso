using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialProfileUpdateMultiple : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialProfileUpdateMultiple;
			}
		}

		/// <summary>
		/// Profile Infos
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public ProfileStructure[] ProfileInfos { get; set; }
	}
}