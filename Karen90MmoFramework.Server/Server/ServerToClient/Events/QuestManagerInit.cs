using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class QuestManagerInit : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.QuestManagerInit;
			}
		}
		
		/// <summary>
		/// Quests
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public ActiveQuestStructure[] Quests { get; set; }
	}
}