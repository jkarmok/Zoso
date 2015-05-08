using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

using ExitGames.Concurrency.Core;
using ExitGames.Concurrency.Fibers;

using Photon.SocketServer;

using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Database;
using Karen90MmoFramework.Server.Game;
using Karen90MmoFramework.Server.Core;
using Karen90MmoFramework.Server.Config;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Events;
using Karen90MmoFramework.Server.ServerToServer.Operations;

namespace Karen90MmoFramework.Server.World
{
	public class WorldServerPeer : OutgoingMasterServerPeer, IWorldServer
	{
		#region Constants and Fields

		/// <summary>
		/// the application
		/// </summary>
		private readonly WorldApplication application;

		/// <summary>
		/// the configuration
		/// </summary>
		private readonly GameConfig configuration;

		/// <summary>
		/// the world
		/// </summary>
		private readonly MmoWorld world;

		/// <summary>
		/// the message fiber
		/// </summary>
		private readonly IFiber messageFiber;

		/// <summary>
		/// the sessions cache
		/// </summary>
		private readonly ConcurrentStorageMap<int, ISession> sessions;
		
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="WorldServerPeer"/> class
		/// </summary>
		/// <param name="initResponse"></param>
		/// <param name="application"></param>
		public WorldServerPeer(InitResponse initResponse, WorldApplication application)
			: base(initResponse, application)
		{
			this.application = application;

			this.messageFiber = new ThreadFiber(new DefaultQueue(), "WorldMessageFiber", true, ThreadPriority.Highest);
			this.messageFiber.Start();

			// the lock will wait indefinitely
			this.sessions = new ConcurrentStorageMap<int, ISession>(-1);

			this.configuration = GameConfig.Initialize(this.application.BinaryPath);
			this.world = MmoWorld.Instantiate(this, configuration, new WorldDescription());
			
			this.CreateWorldZones();
			this.RequestFiber.Enqueue(this.Register);
		}

		#endregion

		#region Implementation of IWorldServerPeer

		/// <summary>
		/// Gets the character database
		/// </summary>
		IDatabase IWorldServer.CharacterDatabase
		{
			get
			{
				return this.application.CharacterDatabase;
			}
		}

		/// <summary>
		/// Gets the world database
		/// </summary>
		IDatabase IWorldServer.WorldDatabase
		{
			get
			{
				return this.application.WorldDatabase;
			}
		}

		/// <summary>
		/// Gets the item database
		/// </summary>
		IDatabase IWorldServer.ItemDatabase
		{
			get
			{
				return this.application.ItemDatabase;
			}
		}

		/// <summary>
		/// Gets the global game time in milliseconds
		/// </summary>
		int IWorldServer.GlobalTime
		{
			get
			{
				return this.MasterTimeStamp;
			}
		}

		/// <summary>
		/// Gets the chat manager
		/// </summary>
		IChatManager IWorldServer.ChatManager
		{
			get
			{
				return OutgoingChatListenerPeer.Instance;
			}
		}

		/// <summary>
		/// Gets the social manager
		/// </summary>
		ISocialManager IWorldServer.SocialManager
		{
			get
			{
				return OutgoingSocialListenerPeer.Instance;
			}
		}

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
		void IWorldServer.KillSession(int sessionId)
		{
			this.SendOperationRequest(
				new OperationRequest((byte) ServerOperationCode.KillSession,
				                     new KillSession
					                     {
											 SessionId = sessionId
					                     }),
				new SendParameters());
		}

		/// <summary>
		/// Transfers a(n) <see cref="ISession"/> to another world
		/// </summary>
		void IWorldServer.TransferSession(int sessionId, int zoneId)
		{
			this.messageFiber.Enqueue(() =>
				{
					ISession session;
					if (sessions.TryGetValue(sessionId, out session))
					{
						sessions.Remove(sessionId);
						// destroy the session due to disconnect
						session.Destroy(DestroySessionReason.Transfer);
						// notifying the master to update the state so the client can start sending messages
						this.SendOperationRequest(
							new OperationRequest((byte) ServerOperationCode.TransferSession,
							                     new TransferSession
								                     {
									                     SessionId = session.SessionId,
									                     WorldId = zoneId,
								                     }),
							new SendParameters());
					}
					else
					{
						Logger.ErrorFormat("[TransferSession]: Session (Id={0}) cannot be found", sessionId);
					}
				});
		}

		#endregion

		#region Implementation of OutgoingMasterServerPeer

		/// <summary>
		/// Called when an event has been received.
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
			if(!gameOperationRequest.IsValid)
			{
#if MMO_DEBUG
				Logger.DebugFormat("[OnOperationRequest]: GameOperationRequest (OpCode={0}) is not valid", operationRequest.OperationCode);
#endif
				return;
			}

			ISession session;
			if(sessions.TryGetValue(gameOperationRequest.ClientId, out session))
			{
				var messageParameters = new MessageParameters {ChannelId = sendParameters.ChannelId, Encrypted = sendParameters.Encrypted};
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
		/// Called when the server is registered on the master
		/// </summary>
		protected override void OnServerRegistered()
		{
			base.OnServerRegistered();

			OutgoingChatListenerPeer.Instance.Connect();
			OutgoingSocialListenerPeer.Instance.Connect();
		}

		/// <summary>
		/// Called right before disconnecting the peer. A good place to notify and kick connected clients.
		/// </summary>
		protected override void OnApplicationStopping()
		{
			OutgoingChatListenerPeer.Instance.Disconnect();
			OutgoingSocialListenerPeer.Instance.Disconnect();

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
				// disposes the world
				this.world.Dispose();
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
			if(sessions.TryGetValue(addSession.SessionId, out existingSession))
			{
				// since adding sessions is controlled by the master rather than the client
				// we will not notify the client rather throw out an error
				Logger.ErrorFormat("[HandleAddSession]: Destroying existing Session (Name={0})", existingSession.Name);
				// destroy the exisiting session
				// we dont need to destroy the requested session since they both have the same session id, both (if possible) will be destroyed
				existingSession.Destroy(DestroySessionReason.KickedByExistingSession);
				return;
			}

			ISession session = new WorldSession(addSession.SessionId, addSession.CharacterName, this, world, addSession.IsTransfer);
			// if a lock wait time is used (not -1) use a while loop to counter lock timeouts
			if(!sessions.Add(addSession.SessionId, session))
			{
				// this will not happen but Murphy's law states otherwise
				Logger.ErrorFormat("[HandleAddSession]: Session (Name={0}) cannot be added", addSession.CharacterName);
				session.Destroy(DestroySessionReason.KickedByServer);
				return;
			}

			// notifying the master to update the state so the client can start sending messages
			this.SendOperationRequest(
				new OperationRequest((byte) ServerOperationCode.AckClientPlayerTransferWorld,
				                     new AckClientPlayerEnterWorld
					                     {
						                     SessionId = session.SessionId,
					                     }),
				new SendParameters());
		}

		/// <summary>
		/// Removes a(n) <see cref="ISession"/>.
		/// </summary>
		/// <param name="removeSession"> </param>
		private void HandleRemoveSession(RemoveSession removeSession)
		{
			ISession session;
			if(sessions.TryGetValue(removeSession.SessionId, out session))
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

		/// <summary>
		/// Loads the world
		/// </summary>
		protected void CreateWorldZones()
		{
			WorldConfig.ZoneConfig zoneConfig;
			if (false == this.configuration.world.TryGetZoneConfig(this.application.SubServerId, out zoneConfig))
				throw new NullReferenceException("ZoneConfigNotFound");

			MmoZone existingZone;
			if(world.Zones.TryGetValue(zoneConfig.id, out existingZone))
				return;

			var terrainDataPath = Path.Combine(this.application.BinaryPath, "Resources", zoneConfig.terrainDataFile);
			using (var stream = new FileStream(terrainDataPath, FileMode.Open, FileAccess.Read))
			{
				var buffer = new byte[sizeof(int)];
				stream.Read(buffer, 0, buffer.Length);
				var columns = BitConverter.ToInt32(buffer, 0);

				buffer = new byte[sizeof(int)];
				stream.Read(buffer, 0, buffer.Length);
				var rows = BitConverter.ToInt32(buffer, 0);

				var heights = new float[columns * rows];
				for (var z = 0; z < rows; z++)
				{
					for (var x = 0; x < columns; x++)
					{
						buffer = new byte[sizeof(float)];
						stream.Read(buffer, 0, sizeof(float));

						heights[z * columns + x] = BitConverter.ToSingle(buffer, 0);
					}
				}

				var terrainCollidersPath = Path.Combine(this.application.BinaryPath, "Resources", zoneConfig.colliderDataFile);
				var reader = new StreamReader(terrainCollidersPath);
				var deserializer = new XmlSerializer(typeof(Colliders));

				var colliders = (Colliders)deserializer.Deserialize(reader);
				reader.Close();
				stream.Close();

				// the bounds of the current zone
				var bounds = new Bounds { Min = new Vector3(zoneConfig.xMin, -5000, zoneConfig.zMin), Max = new Vector3(zoneConfig.xMax, 5000, zoneConfig.zMax) };
				// the tile dimension of the current zone
				var tileDimensions = new Vector3(zoneConfig.tileSize, 0, zoneConfig.tileSize);
				var zoneDescription = new ZoneDescription(columns, rows, heights, colliders);

				// creating the zone
				world.CreateZone((short) zoneConfig.id, zoneConfig.name, bounds, tileDimensions, zoneDescription);
			}
		}

		#endregion
	}
}
