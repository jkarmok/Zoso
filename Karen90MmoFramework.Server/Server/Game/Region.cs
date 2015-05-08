using System;
using System.Threading;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer.Concurrency;

using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Game.Messages;

namespace Karen90MmoFramework.Server.Game
{
	/// <summary>
	/// Represents a region used for region-based interest management. 
	/// A Region is a subclass of <see cref = "MessageChannel{T}" /> and requires messages of type <see cref = "RegionMessage" />.
	/// </summary>
	public class Region : MessageChannel<RegionMessage>, IRegion
	{
		#region Constants and Fields

		private readonly Vector3 coordinate;
		private readonly int hashCode;

		private int numListeners;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref = "Region" /> class.
		/// </summary>
		public Region(Vector3 coordinate)
			: base(MessageCounters.CounterSend)
		{
			this.coordinate = coordinate;
			this.hashCode = this.coordinate.GetHashCode();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the coordinate.
		/// </summary>
		public Vector3 Coordinate
		{
			get
			{
				return this.coordinate;
			}
		}

		/// <summary>
		/// Gets the number of listeners
		/// </summary>
		public int NumListeners
		{
			get
			{
				return this.numListeners;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Compares with another object.
		/// </summary>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Subscribes a listener
		/// </summary>
		/// <param name="fiber"></param>
		/// <param name="receive"></param>
		/// <returns></returns>
		public IDisposable SubscribeListener(IFiber fiber, Action<RegionMessage> receive)
		{
			return new ListenerDisposable(this, Subscribe(fiber, receive));
		}

		/// <summary>
		///   Gets the hash code.
		/// </summary>
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		#endregion

		private class ListenerDisposable : IDisposable
		{
			private readonly Region region;
			private readonly IDisposable disposable;

			public ListenerDisposable(Region region, IDisposable disposable)
			{
				this.region = region;
				this.disposable = disposable;
				Interlocked.Increment(ref region.numListeners);
			}

			#region Implementation of IDisposable

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose()
			{
				disposable.Dispose();
				Interlocked.Decrement(ref region.numListeners);
			}

			#endregion
		}
	}
}