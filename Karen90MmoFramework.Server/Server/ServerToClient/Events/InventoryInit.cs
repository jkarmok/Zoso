using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class InventoryInit : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte) ClientEventCode.InventoryInit;
			}
		}

		/// <summary>
		/// Size
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Size)]
		public byte Size { get; set; }
		
		/// <summary>
		/// Container Items
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection, IsOptional = true)]
		public ContainerItemStructure[] Items { get; set; }
	}
}