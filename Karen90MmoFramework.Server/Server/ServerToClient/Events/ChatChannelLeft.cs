using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ChatChannelLeft : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.ChatChannelLeft;
			}
		}

		/// <summary>
		/// Channel type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelType)]
		public byte ChannelType { get; set; }

		/// <summary>
		/// Channel name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelName, IsOptional = true)]
		public string ChannelName { get; set; }
	}
}