namespace Karen90MmoFramework.Server
{
	public interface IClock
	{
		/// <summary>
		/// Gets the time in milliseconds
		/// </summary>
		int GlobalTime { get; }
	}
}
