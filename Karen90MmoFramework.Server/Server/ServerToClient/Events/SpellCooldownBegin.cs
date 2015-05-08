using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SpellCooldownBegin : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SpellCooldownBegin;
			}
		}

		/// <summary>
		/// Spell Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SpellId)]
		public short SpellId { get; set; }
	}
}