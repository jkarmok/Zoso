using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class InventoryItemAdded : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.InventoryItemAdded;
			}
		}
		
		/// <summary>
		/// Container Item Data
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Data)]
		public ContainerItemStructure ItemData { get; set; }
	}
}