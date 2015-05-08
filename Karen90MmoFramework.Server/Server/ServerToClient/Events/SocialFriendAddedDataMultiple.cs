using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialFriendAddedDataMultiple : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialFriendAddedDataMultiple;
			}
		}

		/// <summary>
		/// Profiles
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public ProfileStructure[] Profiles { get; set; }
	}
}