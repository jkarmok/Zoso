using System.Collections;

namespace Karen90MmoFramework.Server
{
	public interface ISocialManager
	{
		/// <summary>
		/// Updates the profile of a session
		/// </summary>
		void UpdateProfile(int sessionId, Hashtable properties);

		/// <summary>
		/// Registers a world
		/// </summary>
		/// <param name="worldId"></param>
		void RegisterWorld(short worldId);

		/// <summary>
		/// Unregisters a world
		/// </summary>
		/// <param name="worldId"></param>
		void UnregisterWorld(short worldId);

		/// <summary>
		/// Joins the profile to a world
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="worldId"></param>
		void JoinWorld(int sessionId, short worldId);
		
		/// <summary>
		/// Leaves the profile from a world
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="worldId"></param>
		void LeaveWorld(int sessionId, short worldId);
	}
}
