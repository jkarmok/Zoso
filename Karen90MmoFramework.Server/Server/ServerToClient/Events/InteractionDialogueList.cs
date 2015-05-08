using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class InteractionDialogueList : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.InteractionDialogueList;
			}
		}

		/// <summary>
		/// Object Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long ObjectId { get; set; }

		/// <summary>
		/// Menu Items
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public MenuItemStructure[] MenuItems { get; set; }
	}
}