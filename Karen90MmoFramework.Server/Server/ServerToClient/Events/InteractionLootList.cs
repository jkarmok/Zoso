using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class InteractionLootList : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.InteractionLootList;
			}
		}

		/// <summary>
		/// Object Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId)]
		public long ObjectId { get; set; }

		/// <summary>
		/// Gold
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Gold, IsOptional = true)]
		public int? Gold { get; set; }

		/// <summary>
		/// Container Items
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection, IsOptional = true)]
		public ContainerItemStructure[] Items { get; set; }
	}
}