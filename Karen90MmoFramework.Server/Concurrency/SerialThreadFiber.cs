using System;
using System.Threading;

using ExitGames.Concurrency.Core;
using ExitGames.Concurrency.Fibers;

namespace Karen90MmoFramework.Concurrency
{
	/// <summary>
	/// A wrapper around the <see cref="ThreadFiber"/> class which makes sure that tasks enqueued or scheduled will be executed in a serial manner.
	/// </summary>
	public class SerialThreadFiber : ISerialFiber
	{
		private readonly ThreadFiber fiber;

		/// <summary>
		/// Creates a new instance of <see cref="SerialThreadFiber"/> class
		/// </summary>
		public SerialThreadFiber() 
			: this(ThreadPriority.Normal)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="SerialThreadFiber"/> class
		/// </summary>
		public SerialThreadFiber(ThreadPriority threadPriority)
		{
			this.fiber = new ThreadFiber(new DefaultQueue(), "SerialThreadFiber", true, threadPriority);
		}

		/// <summary>
		/// Start consuming actions.
		/// </summary>
		public void Start()
		{
			this.fiber.Start();
		}

		/// <summary>
		/// Enqueue a single action.
		/// </summary>
		/// <param name="action"/>
		public void Enqueue(Action action)
		{
			this.fiber.Enqueue(action);
		}

		/// <summary>
		/// Schedules an action to be executed once.
		/// </summary>
		/// <returns>
		/// a handle to cancel the timer.
		/// </returns>
		public IDisposable Schedule(Action action, long firstInMs)
		{
			return this.fiber.Schedule(action, firstInMs);
		}

		/// <summary>
		/// Schedule an action to be executed on a recurring interval.
		/// </summary>
		/// <returns>
		/// a handle to cancel the timer.
		/// </returns>
		public IDisposable ScheduleOnInterval(Action action, long regularInMs)
		{
			return this.fiber.ScheduleOnInterval(action, regularInMs, regularInMs);
		}

		/// <summary>
		/// Schedule an action to be executed on a recurring interval.
		/// </summary>
		/// <returns>
		/// a handle to cancel the timer.
		/// </returns>
		public IDisposable ScheduleOnInterval(Action action, long firstInMs, long regularInMs)
		{
			return this.fiber.ScheduleOnInterval(action, regularInMs, regularInMs);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			this.fiber.Dispose();
		}

		/// <summary>
		/// Deregister a subscription.
		/// </summary>
		/// <param name="toRemove"/>
		/// <returns/>
		public bool DeregisterSubscription(IDisposable toRemove)
		{
			return this.fiber.DeregisterSubscription(toRemove);
		}

		/// <summary>
		/// Register subscription to be unsubcribed from when the fiber is disposed.
		/// </summary>
		/// <param name="toAdd"/>
		public void RegisterSubscription(IDisposable toAdd)
		{
			this.fiber.RegisterSubscription(toAdd);
		}
	}
}
