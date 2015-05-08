using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server
{
	public delegate void ChannelCreatedCallback(int channelId);

	public interface IChatManager
	{
		/// <summary>
		/// Creates a chat channel
		/// </summary>
		void CreateChannel(string name, ChannelType channelType, ChannelCreatedCallback callback);
		
		/// <summary>
		/// Removes a chat channel
		/// </summary>
		void RemoveChannel(int channelId);

		/// <summary>
		/// Joins the session on a specific chat channel
		/// </summary>
		void JoinChannel(int sessionId, int channelId);

		/// <summary>
		/// Leaves the session from a specific chat channel
		/// </summary>
		void LeaveChannel(int sessionId, int channelId);
	}
}
