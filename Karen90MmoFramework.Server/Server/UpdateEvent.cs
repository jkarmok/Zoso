using System;

using Karen90MmoFramework.Concurrency;

namespace Karen90MmoFramework.Server
{
	internal class UpdateEvent : IDisposable
	{
		#region Constants and Fields

		/// <summary>
		/// fiber
		/// </summary>
		private readonly ISerialFiber fiber;

		/// <summary>
		/// time handler
		/// </summary>
		private readonly IClock timeHandler;

		/// <summary>
		/// interval
		/// </summary>
		private readonly long interval;

		/// <summary>
		/// action
		/// </summary>
		private readonly Action<int> action;

		/// <summary>
		/// last update time
		/// </summary>
		private int lastTime;

		/// <summary>
		/// disposable
		/// </summary>
		private IDisposable updateSubscription;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of <see cref="UpdateEvent"/>.
		/// </summary>
		public UpdateEvent(ISerialFiber fiber, IClock timeHandler, Action<int> action, long intervalInMs)
		{
			this.fiber = fiber;
			this.timeHandler = timeHandler;
			this.action = action;
			this.interval = intervalInMs;
		}

		~UpdateEvent()
		{
			this.Dispose(false);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Starts the current event
		/// </summary>
		public IDisposable Start()
		{
			if (updateSubscription == null)
			{
				this.lastTime = timeHandler.GlobalTime;
				return this.updateSubscription = this.fiber.ScheduleOnInterval(this.Update, this.interval);
			}

			return this.updateSubscription;
		}

		/// <summary>
		/// Updates the event
		/// </summary>
		public void Update()
		{
			this.action(timeHandler.GlobalTime - this.lastTime);
			this.lastTime = timeHandler.GlobalTime;
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (updateSubscription != null)
				this.updateSubscription.Dispose();
		}

		#endregion
	}
}
