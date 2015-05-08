using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class YouLevelUp : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.YouLevelUp;
			}
		}

		/// <summary>
		/// Xp
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Xp)]
		public int Xp { get; set; }
	}
}
