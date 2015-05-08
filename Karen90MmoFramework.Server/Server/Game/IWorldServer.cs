using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Game
{
	public interface IWorldServer : IServer
	{
		/// <summary>
		/// Gets the chat manager
		/// </summary>
		IChatManager ChatManager { get; }

		/// <summary>
		/// Gets the social manager
		/// </summary>
		ISocialManager SocialManager { get; }

		/// <summary>
		/// Gets the character database
		/// </summary>
		IDatabase CharacterDatabase { get; }

		/// <summary>
		/// Gets the world database
		/// </summary>
		IDatabase WorldDatabase { get; }

		/// <summary>
		/// Gets the item database
		/// </summary>
		IDatabase ItemDatabase { get; }

		/// <summary>
		/// Gets the global game time in milliseconds
		/// </summary>
		int GlobalTime { get; }

		/// <summary>
		/// Kills a session
		/// </summary>
		void KillSession(int sessionId);
		
		/// <summary>
		/// Transfers a session to a world
		/// </summary>
		void TransferSession(int sessionId, int zoneId);
	}
}
