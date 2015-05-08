namespace Karen90MmoFramework.Editor
{
	public interface ILogger
	{
		/// <summary>
		/// Log
		/// </summary>
		void Log(string content);

		/// <summary>
		/// Log format
		/// </summary>
		void LogFormat(string format, params object[] arguments);

		/// <summary>
		/// Displays an error message
		/// </summary>
		void Error(string errorDescription);
	}
}
