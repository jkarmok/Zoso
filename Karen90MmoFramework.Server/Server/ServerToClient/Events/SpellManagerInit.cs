using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SpellManagerInit : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get { return (byte)ClientEventCode.SpellManagerInit; }
		}
		
		/// <summary>
		/// Spells
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Collection)]
		public short[] Spells { get; set; }
	}
}