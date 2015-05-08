using System.Collections.Generic;

using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;

namespace Karen90MmoFramework.Server.Chat
{
	public sealed class IncomingChatListenerPeer : PeerBase
	{
		#region Constants and Fields

		/// <summary>
		/// the logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();
		
		/// <summary>
		/// the application
		/// </summary>
		private readonly ChatApplication application;

		/// <summary>
		/// the chat
		/// </summary>
		private readonly MmoChat chat;

		/// <summary>
		/// holds the channels created by this listener
		/// </summary>
		private readonly Dictionary<int, Channel> channels;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="IncomingChatListenerPeer"/> class.
		/// </summary>
		/// <param name="initRequest"></param>
		/// <param name="application"> </param>
		public IncomingChatListenerPeer(InitRequest initRequest, ChatApplication application)
			: base(initRequest.Protocol, initRequest.PhotonPeer)
		{
			this.application = application;
			this.chat = MmoChat.Instance;
			this.channels = new Dictionary<int, Channel>();
		}

		#endregion

		#region Implementation of PeerBase

		protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
		{
			if(chat.Disposed)
				return;

			foreach (var channel in channels.Values)
				this.chat.RemoveChannel(channel);
			this.channels.Clear();
		}

		protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
		{
			OperationResponse response;
			switch ((ServerOperationCode) operationRequest.OperationCode)
			{
				case ServerOperationCode.CreateChannel:
					response = this.HandleOperationCreateChatChannel(operationRequest);
					break;

				case ServerOperationCode.RemoveChannel:
					response = this.HandleOperationRemoveChatChannel(operationRequest);
					break;

				case ServerOperationCode.JoinChannel:
					response = this.HandleOperationJoinChatChannel(operationRequest);
					break;

				case ServerOperationCode.LeaveChannel:
					response = this.HandleOperationLeaveChatChannel(operationRequest);
					break;

				default:
					response = new OperationResponse(operationRequest.OperationCode) {ReturnCode = (short) ResultCode.OperationNotAvailable};
					break;
			}

			if (response != null)
				this.SendOperationResponse(response, new SendParameters());
		}

		#endregion

		#region Operation Handlers

		private OperationResponse HandleOperationCreateChatChannel(OperationRequest operationRequest)
		{
			var operation = new CreateChannel(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			var channel = !string.IsNullOrEmpty(operation.ChannelName)
				                ? this.chat.CreateChannel((ChannelType) operation.ChannelType, operation.ChannelName)
				                : this.chat.CreateChannel((ChannelType) operation.ChannelType);
			
			this.channels.Add(channel.Id, channel);
			return new OperationResponse(operationRequest.OperationCode,
			                             new CreateChannelResponse
				                             {
					                             ChannelId = channel.Id,
					                             CallbackId = operation.CallbackId
				                             });
		}

		private OperationResponse HandleOperationRemoveChatChannel(OperationRequest operationRequest)
		{
			var operation = new RemoveChannel(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			Channel channel;
			if(chat.Channels.TryGetValue(operation.ChannelId, out channel))
			{
				if (!chat.RemoveChannel(channel))
				{
					// this wont happen
					_logger.ErrorFormat("[HandleOperationRemoveChatChannel]: Cannot remove chat channel");
					channel.Dispose();
				}
				this.channels.Remove(channel.Id);
			}
			return null;
		}

		private OperationResponse HandleOperationJoinChatChannel(OperationRequest operationRequest)
		{
			var operation = new JoinChannel(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			Channel channel;
			if(channels.TryGetValue(operation.ChannelId, out channel))
			{
				ChatSession session;
				if (chat.SessionCache.TryGetSessionBySessionId(operation.SessionId, out session))
					channel.Join(session);
			}
			return null;
		}

		private OperationResponse HandleOperationLeaveChatChannel(OperationRequest operationRequest)
		{
			var operation = new LeaveChannel(this.Protocol, operationRequest);
			if (!operation.IsValid)
				return new OperationResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			Channel channel;
			if (channels.TryGetValue(operation.ChannelId, out channel))
			{
				ChatSession session;
				if (chat.SessionCache.TryGetSessionBySessionId(operation.SessionId, out session))
					channel.Leave(session);
			}
			return null;
		}

		#endregion
	}
}
