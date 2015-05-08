using System;

namespace Karen90MmoFramework
{
	public class Reference<T> : IDisposable
	{
		private readonly T obj;
		private readonly IDisposable[] subscriptions;

		/// <summary>
		/// Creates a new instance of the <see cref="Reference{T}"/> class.
		/// </summary>
		/// <param name="obj"> </param>
		/// <param name="subscriptions"></param>
		public Reference(T obj, params IDisposable[] subscriptions)
		{
			this.obj = obj;
			this.subscriptions = subscriptions;
		}

		/// <summary>
		/// Gets the referenced object
		/// </summary>
		public T Object
		{
			get
			{
				return this.obj;
			}
		}
		
		/// <summary>
		/// Disposes all the subscriptions
		/// </summary>
		public void Dispose()
		{
			if(this.subscriptions == null)
				return;

			foreach (var subscription in this.subscriptions)
				subscription.Dispose();
		}
	}
}
