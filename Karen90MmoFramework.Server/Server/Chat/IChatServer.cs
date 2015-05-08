namespace Karen90MmoFramework.Server.Chat
{
	public interface IChatServer : IServer
	{
		/// <summary>
		/// Kills a session
		/// </summary>
		void KillSession(int sessionId);
	}
}
