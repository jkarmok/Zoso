using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialIgnoreAddedName : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialIgnoreAddedName;
			}
		}
		
		/// <summary>
		/// Name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CharacterName)]
		public string Name { get; set; }
	}
}