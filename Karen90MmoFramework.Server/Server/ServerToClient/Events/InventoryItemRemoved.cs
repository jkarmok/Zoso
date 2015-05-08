using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class InventoryItemRemoved : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.InventoryItemRemoved;
			}
		}

		/// <summary>
		/// Index
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Index)]
		public byte Index { get; set; }

		/// <summary>
		/// Quantity
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Quantity)]
		public byte Quantity { get; set; }
	}
}