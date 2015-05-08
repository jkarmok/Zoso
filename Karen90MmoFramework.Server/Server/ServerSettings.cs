namespace Karen90MmoFramework.Server
{
	public static class ServerSettings
	{
		/// <summary>
		/// Operation retry interval in milli seconds
		/// </summary>
		public const int OPERATION_RETRY_INTERVAL = 2000;

		/// <summary>
		/// Server re-connect interval in case connection fails
		/// </summary>
		public const int SERVER_RECONNECT_INTERVAL = 5000;
	}
}
