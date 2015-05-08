using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Social
{
	public interface ISocialServer : IServer
	{
		/// <summary>
		/// Gets the character database
		/// </summary>
		DatabaseFactory CharacterDatabase { get; }

		/// <summary>
		/// Gets the chat manager
		/// </summary>
		IChatManager ChatManager { get; }

		/// <summary>
		/// Kills a session
		/// </summary>
		void KillSession(int sessionId);
	}
}
