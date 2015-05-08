using Karen90MmoFramework.Server.ServerToClient;

namespace Karen90MmoFramework.Server.Game
{
	public class WorldSessionReference : IPeer
	{
		#region Constants and Fields

		private readonly IServer server;
		private readonly int sessionId;
		private readonly string name;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="WorldSessionReference"/> class,
		/// </summary>
		public WorldSessionReference(int sessionId, string name, IServer server)
		{
			this.server = server;
			this.sessionId = sessionId;
			this.name = name;
		}

		#endregion

		#region Implementation of IClientSession

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
		/// Sends an operation response to the client
		/// </summary>
		public void SendOperationResponse(GameOperationResponse response, MessageParameters parameters)
		{
			this.server.SendOperationResponse(sessionId, response, parameters);
		}

		/// <summary>
		/// Sends an operation response to the client
		/// </summary>
		public void SendEvent(GameEvent eventData, MessageParameters parameters)
		{
			this.server.SendEvent(sessionId, eventData, parameters);
		}

		/// <summary>
		/// Disconnects the client
		/// </summary>
		public void Disconnect()
		{
			this.server.KillClient(sessionId);
		}

		#endregion
	}
}
