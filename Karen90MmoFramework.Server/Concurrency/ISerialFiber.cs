using System;

using ExitGames.Concurrency.Fibers;

namespace Karen90MmoFramework.Concurrency
{
	/// <summary>
	/// A type of fiber which executes tasks in a serial manner
	/// </summary>
	public interface ISerialFiber : IFiber
	{
		/// <summary>
		/// Schedule an action to be executed on a recurring interval.
		/// </summary>
		/// <returns>
		/// a handle to cancel the timer.
		/// </returns>
		IDisposable ScheduleOnInterval(Action action, long regularInMs);

		/// <summary>
		/// Schedule an action to be executed on a recurring interval.
		/// </summary>
		/// <returns>
		/// a handle to cancel the timer.
		/// </returns>
		[Obsolete("Use ISerialFiber.ScheduleOnInterval(Action action, long regularInMs) instead")]
		new IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs);
	}
}
