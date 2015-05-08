using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class QuestProgress : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte) ClientEventCode.QuestProgress;
			}
		}
		
		/// <summary>
		/// Quest Progress Data
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Data)]
		public QuestProgressStructure QuestProgressData { get; set; }
	}
}