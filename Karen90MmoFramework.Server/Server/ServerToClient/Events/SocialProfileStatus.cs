using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialProfileStatus : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialProfileStatus;
			}
		}

		/// <summary>
		/// Flags
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CharacterName)]
		public string NameOfProfile { get; set; }
		
		/// <summary>
		/// Status
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Status)]
		public byte Status { get; set; }
	}
}