using System;
using System.Collections.Generic;
using System.Threading;

namespace Karen90MmoTests
{
	class ThreadingDemo : IDemo
	{
		#region Implementation of IDemo

		public void Run()
		{
			
		}

		#endregion
	}

	class MmoThread : IDisposable
	{
		private readonly Thread thread;
		private readonly Queue<Action> actions;
		private readonly object locker;

		public MmoThread(string name, bool isBackground, ThreadPriority priority)
		{
			this.thread = new Thread(o => this.Run()) {Name = name, IsBackground = isBackground, Priority = priority};

			this.actions = new Queue<Action>();
			this.locker = new object();
		}

		public void Start()
		{
			this.thread.Start();
		}

		public void Enqueue(Action action)
		{
			Monitor.Enter(locker);
			try
			{
				this.actions.Enqueue(action);
				Monitor.Pulse(locker);
			}
			finally
			{
				Monitor.Exit(locker);
			}
		}

		void Run()
		{
			Monitor.Enter(locker);
			try
			{
				while (true)
				{
					if (actions.Count == 0)
						Monitor.Wait(locker);

					var action = actions.Dequeue();
					if (action == null)
						break;
					action();
				}
			}
			finally
			{
				Monitor.Exit(locker);
			}
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			this.Enqueue(null);
		}

		#endregion
	}
}
