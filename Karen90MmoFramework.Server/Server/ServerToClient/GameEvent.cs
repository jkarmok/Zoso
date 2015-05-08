namespace Karen90MmoFramework.Server.ServerToClient
{
	public abstract class GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public abstract byte EventCode { get; }
	}
}
