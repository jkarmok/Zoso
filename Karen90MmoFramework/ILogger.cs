namespace Karen90MmoFramework
{
	public interface ILogger
	{
		/// <summary>
		/// Gets the name of the logger
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Logs a debug message
		/// </summary>
		void Debug(object message);

		/// <summary>
		/// Logs a debug message with format parameters
		/// </summary>
		void DebugFormat(string format, params object[] args);

		/// <summary>
		/// Logs an error message
		/// </summary>
		void Error(object message);

		/// <summary>
		/// Logs a error message with format parameters
		/// </summary>
		void ErrorFormat(string format, params object[] args);

		/// <summary>
		/// Logs a warning message
		/// </summary>
		void Warn(object message);

		/// <summary>
		/// Logs a warning message with format parameters
		/// </summary>
		void WarnFormat(string format, params object[] args);
	}
}
