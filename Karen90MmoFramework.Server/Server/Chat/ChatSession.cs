using System.Collections.Generic;
using System.Threading;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToClient.Events;
using Karen90MmoFramework.Server.ServerToClient.Operations;

namespace Karen90MmoFramework.Server.Chat
{
	public class ChatSession : IPeer, ISession, IListener, IPublisher
	{
		#region Constants and Fields

		/// <summary>
		/// logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		private readonly int sessionId;
		private readonly string name;

		private readonly MmoChat chat;
		private readonly IChatServer server;

		private readonly List<IChannel> joinedChannels;

		private int sessionState;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the session state
		/// </summary>
		public ChatSessionState SessionState
		{
			get
			{
				return (ChatSessionState)sessionState;
			}

			private set
			{
				Interlocked.Exchange(ref sessionState, (int)value);
			}
		}

		/// <summary>
		/// Gets the chat
		/// </summary>
		public MmoChat Chat
		{
			get
			{
				return this.chat;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="ChatSession"/> class,
		/// </summary>
		public ChatSession(int sessionId, string name, IChatServer server, MmoChat chat)
		{
			this.server = server;
			this.chat = chat;
			this.sessionId = sessionId;
			this.name = name;

			this.joinedChannels = new List<IChannel>();
			this.SessionState = ChatSessionState.Login;

			this.chat.SyncFiber.Enqueue(DoAddSession);
		}

		#endregion

		#region Implementation of IPeer

		/// <summary>
		/// Sends an operation response to the client
		/// </summary>
		public void SendOperationResponse(GameOperationResponse response, MessageParameters parameters)
		{
			this.server.SendOperationResponse(this.SessionId, response, parameters);
		}

		/// <summary>
		/// Sends an operation response to the client
		/// </summary>
		public void SendEvent(GameEvent eventData, MessageParameters parameters)
		{
			this.server.SendEvent(this.SessionId, eventData, parameters);
		}

		/// <summary>
		/// Disconnects the client
		/// </summary>
		public void Disconnect()
		{
			this.server.KillClient(sessionId);
		}

		#endregion

		#region Implementation of ISession

		/// <summary>
		/// Gets the session Id
		/// </summary>
		public int SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		/// <summary>
		/// Gets the character name
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Queues an <see cref="GameOperationRequest"/> to be processed
		/// </summary>
		public void ReceiveOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters)
		{
			this.chat.SyncFiber.Enqueue(() => this.OnOperationRequest(operationRequest, messageParameters));
		}

		/// <summary>
		/// Destroys the session
		/// </summary>
		/// <param name="destroyReason"></param>
		public void Destroy(DestroySessionReason destroyReason)
		{
			this.chat.SyncFiber.Enqueue(() =>
				{
					// making a copy of our current state since this can change
					var currentState = this.SessionState;
					// setting the session state right before doing cleanup to update the value immediately
					this.SessionState = ChatSessionState.Destroyed;
					// do not move it outside the thread
					// because this will be ignored if there are two destroy calls queued at the same time
					if (currentState == ChatSessionState.Destroyed)
						return;

					lock (joinedChannels)
					{
						var channels = joinedChannels.ToArray();
						foreach (var channel in channels)
							channel.Leave(this);
						this.joinedChannels.Clear();
					}

					// remove us from the chat
					while (!chat.RemoveSession(this))
					{
						ChatSession existingSession;
						if (!chat.SessionCache.TryGetSessionBySessionId(SessionId, out existingSession))
							return;
					}
				});
		}

		#endregion

		#region Implementation of IListener

		/// <summary>
		/// Receives a tell from another <see cref="ChatSession"/>.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"> </param>
		void ReceiveTell(ChatSession sender, string message)
		{
			// sending back the message to the sender (acknowledges message sent)
			var sentMessage = new ChatMessageReceived
				{
					MessageType = (byte) MessageType.Tell,
					Sender = sender.name,
					Receiver = this.Name,
					Message = message
				};
			sender.SendEvent(sentMessage, new MessageParameters {ChannelId = PeerSettings.ChatEventChannel});

			// sending the receiver the message
			var receivedMessage = new ChatMessageReceived
				{
					MessageType = (byte) MessageType.Tell,
					Sender = sender.name,
					Message = message
				};
			this.SendEvent(receivedMessage, new MessageParameters {ChannelId = PeerSettings.ChatEventChannel});
		}

		/// <summary>
		/// Receives a chat message
		/// </summary>
		/// <param name="message"></param>
		void IListener.ReceiveMessage(ChannelMessage message)
		{
			var receivedMessage = new ChatMessageReceived {Message = message.Message};
			switch (message.Channel.Type)
			{
				case ChannelType.Guild:
					receivedMessage.MessageType = (byte) MessageType.Guild;
					break;

				case ChannelType.Group:
					receivedMessage.MessageType = (byte) MessageType.Group;
					break;

				case ChannelType.Local:
					receivedMessage.MessageType = (byte) MessageType.Local;
					break;

				case ChannelType.Trade:
					receivedMessage.MessageType = (byte) MessageType.Trade;
					break;

				case ChannelType.Custom:
					receivedMessage.MessageType = (byte) MessageType.Channel;
					receivedMessage.ChannelName = message.Channel.Name;
					break;

			}

			if (message.Publisher != null)
				receivedMessage.Sender = message.Publisher.Name;

			this.SendEvent(receivedMessage, new MessageParameters { ChannelId = PeerSettings.ChatEventChannel });
		}

		/// <summary>
		/// Called after joining the channel
		/// </summary>
		/// <param name="channel"></param>
		void IListener.OnJoinedChannel(IChannel channel)
		{
			lock (joinedChannels)
				this.joinedChannels.Add(channel);

			var joinedChannel = new ChatChannelJoined {ChannelType = (byte) channel.Type};
			if (!string.IsNullOrEmpty(channel.Name))
				joinedChannel.ChannelName = channel.Name;

			this.SendEvent(joinedChannel, new MessageParameters {ChannelId = PeerSettings.ChatEventChannel});
		}

		/// <summary>
		/// Called after leaving a channel
		/// </summary>
		/// <param name="channel"></param>
		void IListener.OnLeftChannel(IChannel channel)
		{
			lock (joinedChannels)
				this.joinedChannels.Remove(channel);

			var leftChannel = new ChatChannelLeft {ChannelType = (byte) channel.Type};
			if (!string.IsNullOrEmpty(channel.Name))
				leftChannel.ChannelName = channel.Name;

			this.SendEvent(leftChannel, new MessageParameters {ChannelId = PeerSettings.ChatEventChannel});
		}

		#endregion

		#region Operation Handlers

		private GameOperationResponse HandleOperationSendChat(GameOperationRequest operationRequest)
		{
			var operation = new SendChat(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			switch ((MessageType) operation.MessageType)
			{
				case MessageType.Group:
					{
						lock (joinedChannels)
						{
							var channel = this.joinedChannels.Find(chnl => chnl.Type == ChannelType.Group);
							if (channel == null)
								return new SendChatResponse(operation.OperationCode) {ReturnCode = (short) ResultCode.ChannelNotAvailable, MessageType = operation.MessageType};

							// TODO: Normalize content and prevent spam
							var message = operation.Message;
							if (string.IsNullOrEmpty(message))
								return operation.GetErrorResponse((short) ResultCode.MessageIsEmpty);

							channel.Publish(message, this);
						}
					}
					break;

				case MessageType.Guild:
					{
						lock (joinedChannels)
						{
							var channel = this.joinedChannels.Find(chnl => chnl.Type == ChannelType.Guild);
							if (channel == null)
								return new SendChatResponse(operation.OperationCode) {ReturnCode = (short) ResultCode.ChannelNotAvailable, MessageType = operation.MessageType};

							// TODO: Normalize content and prevent spam
							var message = operation.Message;
							if (string.IsNullOrEmpty(message))
								return operation.GetErrorResponse((short) ResultCode.MessageIsEmpty);

							channel.Publish(message, this);
						}
					}
					break;

				case MessageType.Local:
					{
						lock (joinedChannels)
						{
							var channel = this.joinedChannels.Find(chnl => chnl.Type == ChannelType.Local);
							if (channel == null)
								return new SendChatResponse(operation.OperationCode) {ReturnCode = (short) ResultCode.ChannelNotAvailable, MessageType = operation.MessageType};

							// TODO: Normalize content and prevent spam
							var message = operation.Message;
							if (string.IsNullOrEmpty(message))
								return operation.GetErrorResponse((short) ResultCode.MessageIsEmpty);

							channel.Publish(message, this);
						}
					}
					break;

				case MessageType.Trade:
					{
						lock (joinedChannels)
						{
							var channel = this.joinedChannels.Find(chnl => chnl.Type == ChannelType.Trade);
							if (channel == null)
								return new SendChatResponse(operation.OperationCode) {ReturnCode = (short) ResultCode.ChannelNotAvailable, MessageType = operation.MessageType};

							// TODO: Normalize content and prevent spam
							var message = operation.Message;
							if (string.IsNullOrEmpty(message))
								return operation.GetErrorResponse((short) ResultCode.MessageIsEmpty);

							channel.Publish(message, this);
						}
					}
					break;

				case MessageType.Tell:
					{
						var receiverName = operation.Receiver;
						if (string.IsNullOrEmpty(receiverName))
							return operation.GetErrorResponse((short) ResultCode.PlayerNotFound);

						ChatSession receiver;
						if (!chat.SessionCache.TryGetSessionBySessionName(receiverName, out receiver))
							return operation.GetErrorResponse((short) ResultCode.PlayerNotFound);

						// TODO: Normalize content and prevent spam
						var message = operation.Message;
						if (string.IsNullOrEmpty(message))
							return operation.GetErrorResponse((short) ResultCode.MessageIsEmpty);

						receiver.ReceiveTell(this, message);
					}
					break;

				default:
					{
#if MMO_DEBUG
						_logger.DebugFormat("[HandleOperationSendChat]: Session (Name={0}) sent an unhandled Message (Type={1})", name,
						                    (MessageType) operation.MessageType);
#endif
					}
					break;
			}

			return null;
		}

		/// <summary>
		/// Call this to handle client operation requests
		/// </summary>
		void OnOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters)
		{
			GameOperationResponse response;
			switch ((GameOperationCode)operationRequest.OperationCode)
			{
				case GameOperationCode.SendChat:
					response = this.HandleOperationSendChat(operationRequest);
					break;

				default:
					response = new GameErrorResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.OperationNotAvailable };
					break;
			}

			if (response != null)
				this.SendOperationResponse(response, messageParameters);
		}

		#endregion

		#region Helper Methods

		void DoAddSession()
		{
			// adding session to the chat
			while (!chat.AddSession(this))
			{
				ChatSession exisitingSession;
				if (chat.SessionCache.TryGetSessionBySessionId(sessionId, out exisitingSession))
				{
					// this shouldn't happen because the master won't let the same player login twice it will disconnect the exisitng player
					// we are handling it to satisfy Murphy's Law
#if MMO_DEBUG
					_logger.DebugFormat("[DoAddSession]: ChatSession (Name={0}) cannot be added because an existing ChatSession (Name={1}) is found",
										this.Name, exisitingSession.Name);
#endif
					exisitingSession.Disconnect();
					this.Disconnect();
					this.Destroy(DestroySessionReason.KickedByExistingSession);
					return;
				}
			}
		}

		#endregion
	}
}
