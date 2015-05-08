using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class ChatMessageReceived : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.ChatMessageReceived;
			}
		}

		/// <summary>
		/// Message Type
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.MessageType)]
		public byte MessageType { get; set; }

		/// <summary>
		/// Chat Message
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Message)]
		public string Message { get; set; }

		/// <summary>
		/// Channel Name
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ChannelName, IsOptional = true)]
		public string ChannelName { get; set; }
		
		/// <summary>
		/// Sender
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Sender, IsOptional = true)]
		public string Sender { get; set; }

		/// <summary>
		/// Receiver
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Receiver, IsOptional = true)]
		public string Receiver { get; set; }
	}
}