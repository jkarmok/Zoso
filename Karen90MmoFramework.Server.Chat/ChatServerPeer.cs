using System.Threading;

using ExitGames.Concurrency.Core;
using ExitGames.Concurrency.Fibers;

using Photon.SocketServer;

using Karen90MmoFramework.Server.Core;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;
using Karen90MmoFramework.Server.ServerToServer.Events;

namespace Karen90MmoFramework.Server.Chat
{
	public class ChatServerPeer : OutgoingMasterServerPeer, IChatServer
	{
		#region Constants and Fields

		/// <summary>
		/// the application
		/// </summary>
		private readonly ChatApplication application;

		/// <summary>
		/// the chat
		/// </summary>
		private readonly MmoChat chat;

		/// <summary>
		/// the message fiber
		/// </summary>
		private readonly IFiber messageFiber;

		/// <summary>
		/// the sessions cache
		/// </summary>
		private readonly ConcurrentStorageMap<int, ISession> sessions;
		
		#endregion

		#region Properties

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
		/// Creates a new instance of the <see cref="ChatServerPeer"/> class.
		/// </summary>
		/// <param name="initResponse"></param>
		/// <param name="application"></param>
		public ChatServerPeer(InitResponse initResponse, ChatApplication application)
			: base(initResponse, application)
		{
			this.application = application;

			this.messageFiber = new ThreadFiber(new DefaultQueue(), "ChatMessageFiber", true, ThreadPriority.Highest);
			this.messageFiber.Start();

			this.chat = MmoChat.Instantiate(this);
			// the lock will wait indefinitely
			this.sessions = new ConcurrentStorageMap<int, ISession>(-1);

			this.RequestFiber.Enqueue(this.Register);
		}

		#endregion

		#region Implementation of IChatServerPeer

		/// <summary>
		/// Kills the client of a session
		/// </summary>
		/// <param name="sessionId"></param>
		void IServer.KillClient(int sessionId)
		{
			this.SendOperationRequest(
				new OperationRequest((byte)ServerOperationCode.KillClient,
									 new KillClient
									 {
										 ClientId = sessionId
									 }),
				new SendParameters());
		}

		/// <summary>
		/// Kills a session
		/// </summary>
		void IChatServer.KillSession(int sessionId)
		{
			this.SendOperationRequest(
				new OperationRequest((byte)ServerOperationCode.KillSession,
									 new KillSession
									 {
										 SessionId = sessionId
									 }),
				new SendParameters());
		}

		#endregion

		#region Implementation of OutgoingMasterServerPeer

		/// <summary>
		/// Called when an event has been received from the master server
		/// </summary>
		protected override void OnServerEvent(IEventData eventData, SendParameters sendParameters)
		{
			switch (eventData.Code)
			{
				case (byte)ServerEventCode.AddSession:
					{
						this.HandleEventAddSession(eventData, sendParameters);
					}
					break;

				case (byte)ServerEventCode.RemoveSession:
					{
						this.HandleEventRemoveSession(eventData, sendParameters);
					}
					break;
			}
		}

		/// <summary>
		/// Called when an operation request has been received.
		/// </summary>
		/// <remarks>An operation request is sent when a client sends an operation request to the master. So all operation requests are from game clients</remarks>
		protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
		{
			var gameOperationRequest = new GameOperationRequest(operationRequest.Parameters);
			if (!gameOperationRequest.IsValid)
			{
#if MMO_DEBUG
				Logger.DebugFormat("[OnOperationRequest]: GameOperationRequest (OpCode={0}) is not valid", operationRequest.OperationCode);
#endif
				return;
			}

			ISession session;
			if (sessions.TryGetValue(gameOperationRequest.ClientId, out session))
			{
				var messageParameters = new MessageParameters { ChannelId = sendParameters.ChannelId, Encrypted = sendParameters.Encrypted };
				session.ReceiveOperationRequest(gameOperationRequest, messageParameters);
			}
			else
			{
				Logger.ErrorFormat("[OnOperationRequest]: Session (Id={0}) cannot be found", gameOperationRequest.ClientId);
			}
		}

		/// <summary>
		/// Called when an operation response has been received from the master server
		/// </summary>
		protected override void OnServerOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
		{
#if MMO_DEBUG
			Logger.DebugFormat("[OnServerOperationResponse]: OperationResponse (OpCode={0}) is not handled", operationResponse.OperationCode);
#endif
		}

		/// <summary>
		/// Called right before disconnecting the peer. A good place to notify and kick connected clients.
		/// </summary>
		protected override void OnApplicationStopping()
		{
			// kicking all sessions out
			foreach (var session in sessions)
				session.Destroy(DestroySessionReason.KickedByServer);
			base.OnApplicationStopping();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.sessions.Dispose();
				// disposes the chat
				this.chat.Dispose();
				this.messageFiber.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Event Handlers

		protected void HandleEventAddSession(IEventData eventData, SendParameters sendParameters)
		{
			var serverEvent = new AddSession(Protocol, eventData);
			if (!serverEvent.IsValid)
			{
				Logger.ErrorFormat("[HandleEventAddSession]: Event (Code={0}) is invalid (Error={1})", eventData.Code, serverEvent.GetErrorMessage());
			}
			else
			{
				// using a seperate thread to avoid message throttling
				this.messageFiber.Enqueue(() => this.HandleAddSession(serverEvent));
			}
		}

		protected void HandleEventRemoveSession(IEventData eventData, SendParameters sendParameters)
		{
			var serverEvent = new RemoveSession(Protocol, eventData);
			if (!serverEvent.IsValid)
			{
				Logger.ErrorFormat("[HandleEventRemoveSession]: Event (Code={0}) is invalid (Error={1})", eventData.Code, serverEvent.GetErrorMessage());
			}
			else
			{
				// using a seperate thread to avoid message throttling
				this.messageFiber.Enqueue(() => this.HandleRemoveSession(serverEvent));
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Adds a(n) <see cref="ISession"/>.
		/// </summary>
		/// <param name="addSession"></param>
		private void HandleAddSession(AddSession addSession)
		{
			ISession existingSession;
			// making sure we dont already have a session with the same session id
			if (sessions.TryGetValue(addSession.SessionId, out existingSession))
			{
				// since adding sessions is controlled by the master rather than the client
				// we will not notify the client rather throw out an error
				Logger.ErrorFormat("[HandleAddSession]: Destroying existing Session (Name={0})", existingSession.Name);
				// destroy the exisiting session
				// we dont need to destroy the requested session since they both have the same session id, both (if possible) will be destroyed
				existingSession.Destroy(DestroySessionReason.KickedByExistingSession);
				return;
			}

			ISession session = new ChatSession(addSession.SessionId, addSession.CharacterName, this, chat);
			// if a lock wait time is used (not -1) use a while loop to counter lock timeouts
			if (!sessions.Add(addSession.SessionId, session))
			{
				// this will not happen but Murphy's law states otherwise
				Logger.ErrorFormat("[HandleAddSession]: Session (Name={0}) cannot be added", addSession.CharacterName);
				session.Destroy(DestroySessionReason.KickedByServer);
			}
		}

		/// <summary>
		/// Removes a(n) <see cref="ISession"/>.
		/// </summary>
		/// <param name="removeSession"> </param>
		private void HandleRemoveSession(RemoveSession removeSession)
		{
			ISession session;
			if (sessions.TryGetValue(removeSession.SessionId, out session))
			{
				sessions.Remove(removeSession.SessionId);
				// destroy the session due to disconnect
				session.Destroy(DestroySessionReason.Disconnect);
			}
			else
			{
				Logger.ErrorFormat("[HandleRemoveSession]: Session (Id={0}) cannot be found", removeSession.SessionId);
			}
		}

		#endregion
	}
}
