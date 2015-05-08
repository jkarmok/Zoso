using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class QuestFinished : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte) ClientEventCode.QuestFinished;
			}
		}

		/// <summary>
		/// Quest Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.QuestId)]
		public short QuestId { get; set; }
	}
}