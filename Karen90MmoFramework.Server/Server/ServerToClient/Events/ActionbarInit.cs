using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ActionbarInit : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.ActionbarInit;
			}
		}
		
		/// <summary>
		/// Action items
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public ActionItemStructure[] ActionItems { get; set; }
	}
}