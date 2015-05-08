using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class InventoryItemAddedMultiple : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte) ClientEventCode.InventoryItemAddedMultiple;
			}
		}

		/// <summary>
		/// Container Items
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public ContainerItemStructure[] Items { get; set; }
	}
}