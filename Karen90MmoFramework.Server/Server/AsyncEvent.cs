using System;
using System.Threading;

using ExitGames.Concurrency.Core;
using ExitGames.Concurrency.Fibers;

namespace Karen90MmoFramework.Server
{
	// Based off of ExitGames.Concurrency.Channels.Channel{T}
	public sealed class AsyncEvent
	{
		#region Constants and Fields

		private readonly object syncRoot = new object();
		private Action method;

		#endregion

		#region Properties

		/// <summary>
		/// Tells whether there are any active subscriptions available or not
		/// </summary>
		public bool HasSubscriptions
		{
			get
			{
				return this.method != null;
			}
		}

		/// <summary>
		/// Gets the number of subscribers
		/// </summary>
		public int NumSubscribers
		{
			get
			{
				return this.method != null ? this.method.GetInvocationList().Length : 0;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Clears all subscriptions
		/// </summary>
		public void ClearSubscribers()
		{
			this.method = null;
		}

		/// <summary>
		/// Subscribes on the event
		/// </summary>
		/// <param name="fiber"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public IDisposable Subscribe(IFiber fiber, Action action)
		{
			return this.Subscribe(() => fiber.Enqueue(action), fiber);
		}

		/// <summary>
		/// Invokes the event
		/// </summary>
		public void Invoke()
		{
			var action = this.method;
			if (action != null)
				action();
		}

		private IDisposable Subscribe(Action action, ISubscriptionRegistry subscriptionRegistery)
		{
			Monitor.Enter(syncRoot);
			try
			{
				this.Add(action);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}

			var subscriptionDisposable = new EventSubscriptionDisposable(action, this, subscriptionRegistery);
			subscriptionRegistery.RegisterSubscription(subscriptionDisposable);
			return subscriptionDisposable;
		}

		internal void Unsubscribe(Action action)
		{
			Monitor.Enter(syncRoot);
			try
			{
				this.Remove(action);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		private void Add(Action toAdd)
		{
			var action = this.method;
			Action comparand;
			do
			{
				comparand = action;
				action = Interlocked.CompareExchange(ref method, (Action) Delegate.Combine(comparand, toAdd), comparand);
			} while (action != comparand);
		}

		private void Remove(Action toRemove)
		{
			var action = this.method;
			Action comparand;
			do
			{
				comparand = action;
				action = Interlocked.CompareExchange(ref method, (Action) Delegate.Remove(comparand, toRemove), comparand);
			} while (action != comparand);
		}

		#endregion

		private class EventSubscriptionDisposable : IDisposable
		{
			private readonly Action action;
			private readonly AsyncEvent asyncEvent;
			private readonly ISubscriptionRegistry subscriptionRegistry;

			public EventSubscriptionDisposable(Action action, AsyncEvent asyncEvent, ISubscriptionRegistry subscriptionRegistry)
			{
				this.action = action;
				this.asyncEvent = asyncEvent;
				this.subscriptionRegistry = subscriptionRegistry;
			}

			public void Dispose()
			{
				this.asyncEvent.Unsubscribe(action);
				this.subscriptionRegistry.DeregisterSubscription(this);
			}
		}
	}

	// Based off of ExitGames.Concurrency.Channels.Channel{T}
	public sealed class AsyncEvent<T>
	{
		#region Constants and Fields

		private readonly object syncRoot = new object();
		private Action<T> method;

		#endregion

		#region Properties

		/// <summary>
		/// Tells whether there are any active subscriptions available or not
		/// </summary>
		public bool HasSubscriptions
		{
			get
			{
				return this.method != null;
			}
		}

		/// <summary>
		/// Gets the number of subscribers
		/// </summary>
		public int NumSubscribers
		{
			get
			{
				return this.method != null ? this.method.GetInvocationList().Length : 0;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Clears all subscriptions
		/// </summary>
		public void ClearSubscribers()
		{
			this.method = null;
		}

		/// <summary>
		/// Subscribes on the event
		/// </summary>
		/// <param name="fiber"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public IDisposable Subscribe(IFiber fiber, Action<T> action)
		{
			return this.Subscribe(arg => fiber.Enqueue(() => action(arg)), fiber);
		}

		/// <summary>
		/// Invokes the event
		/// </summary>
		public void Invoke(T arg)
		{
			var action = this.method;
			if (action != null)
				action(arg);
		}

		private IDisposable Subscribe(Action<T> action, ISubscriptionRegistry subscriptionRegistery)
		{
			Monitor.Enter(syncRoot);
			try
			{
				this.Add(action);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}

			var subscriptionDisposable = new EventSubscriptionDisposable(action, this, subscriptionRegistery);
			subscriptionRegistery.RegisterSubscription(subscriptionDisposable);
			return subscriptionDisposable;
		}

		internal void Unsubscribe(Action<T> action)
		{
			Monitor.Enter(syncRoot);
			try
			{
				this.Remove(action);
			}
			finally
			{
				Monitor.Exit(syncRoot);
			}
		}

		private void Add(Action<T> toAdd)
		{
			var action = this.method;
			Action<T> comparand;
			do
			{
				comparand = action;
				action = Interlocked.CompareExchange(ref method, (Action<T>)Delegate.Combine(comparand, toAdd), comparand);
			} while (action != comparand);
		}

		private void Remove(Action<T> toRemove)
		{
			var action = this.method;
			Action<T> comparand;
			do
			{
				comparand = action;
				action = Interlocked.CompareExchange(ref method, (Action<T>)Delegate.Remove(comparand, toRemove), comparand);
			} while (action != comparand);
		}

		#endregion

		private class EventSubscriptionDisposable : IDisposable
		{
			private readonly Action<T> action;
			private readonly AsyncEvent<T> asyncEvent;
			private readonly ISubscriptionRegistry subscriptionRegistry;

			public EventSubscriptionDisposable(Action<T> action, AsyncEvent<T> asyncEvent, ISubscriptionRegistry subscriptionRegistry)
			{
				this.action = action;
				this.asyncEvent = asyncEvent;
				this.subscriptionRegistry = subscriptionRegistry;
			}

			public void Dispose()
			{
				this.asyncEvent.Unsubscribe(action);
				this.subscriptionRegistry.DeregisterSubscription(this);
			}
		}
	}
}
