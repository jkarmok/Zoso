using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class InventoryItemMoved : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.InventoryItemMoved;
			}
		}

		/// <summary>
		/// Index From
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.IndexFrom)]
		public byte IndexFrom { get; set; }
		
		/// <summary>
		/// Index To
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Index)]
		public byte IndexTo { get; set; }
	}
}