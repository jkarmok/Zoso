using System;
using System.Collections.Generic;
using System.Linq;

using ExitGames.Concurrency.Fibers;
using Photon.SocketServer.Concurrency;

using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Game.Messages;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game
{
	public class InterestArea : IDisposable
	{
		#region Constants and Fields

		/// <summary>
		///   Locking the sync root guarantees thread safe access.
		/// </summary>
		public readonly object SyncRoot = new object();

		/// <summary>
		///   The subscribed items.
		/// </summary>
		private readonly Dictionary<MmoObject, MmoObjectAutoSubscription> autoManagedItemSubscriptions;

		/// <summary>
		///   The item snap shot request
		/// </summary>
		private readonly MmoObjectSnapshotRequest snapShotRequest;

		/// <summary>
		///   The subscribedWorldRegions.
		/// </summary>
		private readonly Dictionary<Region, IDisposable> subscribedWorldRegions;

		/// <summary>
		///   The subscription management fiber.
		/// </summary>
		private readonly IFiber subscriptionManagementFiber;

		/// <summary>
		///   The world.
		/// </summary>
		private readonly MmoZone world;

		/// <summary>
		///   The world area
		/// </summary>
		private readonly Bounds worldArea;

		/// <summary>
		///   The current inner focus (region boundaries)
		/// </summary>
		private Bounds currentRegionInnerFocus;

		/// <summary>
		///   The current outer focus (region boundaries)
		/// </summary>
		private Bounds currentRegionOuterFocus;

		/// <summary>
		///   The item movement subscription.
		/// </summary>
		private IDisposable itemMovementSubscription;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///   Initializes static members of the <see cref = "InterestArea" /> class.
		/// </summary>
		static InterestArea()
		{
			GameObjectAutoUnsubcribeDelayMilliseconds = 5000;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref = "InterestArea" /> class.
		/// </summary>
		protected InterestArea(MmoZone world, IFiber fiber)
		{
			this.world = world;
			this.snapShotRequest = new MmoObjectSnapshotRequest(this);
			this.subscribedWorldRegions = new Dictionary<Region, IDisposable>();
			this.autoManagedItemSubscriptions = new Dictionary<MmoObject, MmoObjectAutoSubscription>();
			this.subscriptionManagementFiber = fiber;
			//this.subscriptionManagementFiber.Start();

			this.worldArea = world.Bounds;

			// make invalid
			this.currentRegionInnerFocus = new Bounds { Max = this.worldArea.Min, Min = this.worldArea.Max };
			this.currentRegionOuterFocus = this.currentRegionInnerFocus;
		}

		/// <summary>
		///   Finalizes an instance of the <see cref = "InterestArea" /> class.
		/// </summary>
		~InterestArea()
		{
			this.Dispose(false);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets ItemAutoUnsubcribeDelayMilliseconds.
		/// Default: 5000ms.
		/// </summary>
		public static int GameObjectAutoUnsubcribeDelayMilliseconds { get; set; }

		/// <summary>
		/// Gets the Owner
		/// </summary>
		public Player Owner { get; private set; }

		/// <summary>
		/// Gets or sets the interest area <see cref = "Vector3" /> position.
		/// This value is used for internal  management calculations.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Gets or sets the inner view distance (the item subscribe threshold).
		/// </summary>
		public Vector3 ViewDistanceEnter { get; set; }

		/// <summary>
		/// Gets or sets the outer view distance (the item unsubscribe threshold).
		/// </summary>
		public Vector3 ViewDistanceExit { get; set; }

		/// <summary>
		/// Gets the <see cref = "MmoZone" /> the interest area looks at.
		/// </summary>
		public MmoZone World
		{
			get
			{
				return this.world;
			}
		}

		#endregion

		#region IDisposable Implementation

		/// <summary>
		///   Calls <see cref = "Dispose(bool)" /> and suppresses finalization.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   Disposes the fiber used to manage the subscriptions, detaches any attached item and resolves all existing channel subscriptions.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.subscriptionManagementFiber.Dispose();

				// detach
				if (this.Owner != null)
				{
					this.itemMovementSubscription.Dispose();
					this.itemMovementSubscription = null;
					this.Owner = null;
				}

				this.ClearRegionSubscriptions();
				this.ClearAutoSubscriptions();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the owner of the <see cref="InterestArea"/>. This <see cref="Player"/> will be excluded in interest management.
		/// </summary>
		public void SetOwner(Player player)
		{
			if (this.Owner != null)
				throw new InvalidOperationException();

			this.Owner = player;
			this.Position = player.Position;

			if (this.CanAutoSubscribeMmoObject(player) == false)
			{
				MmoObjectAutoSubscription autoSubscription;
				if (this.autoManagedItemSubscriptions.TryGetValue(player, out autoSubscription))
				{
					this.AutoUnsubscribeMmoObject(autoSubscription);
				}
			}

			var disposeSubscription = player.DisposeChannel.Subscribe(this.subscriptionManagementFiber, this.AttachedItem_OnMmoObjectDisposed);

			// move camera when item moves
			var positionSubscription = player.PositionUpdateChannel.Subscribe(this.subscriptionManagementFiber, this.AttachedMmoObject_OnGameObjectPosition);
			this.itemMovementSubscription = new UnsubscriberCollection(disposeSubscription, positionSubscription);
		}

		/// <summary>
		///   Updates the <see cref = "Region" /> subscriptions that are used to detect <see cref = "MmoObject">Items</see> in the nearby <see cref = "MmoZone" />.
		///   This method should be called after changing the interest area's <see cref = "Position" />.
		/// </summary>
		public void UpdateInterestManagement()
		{
			// update unsubscribe area
			var focus = Bounds.CreateFromPoints(this.Position - this.ViewDistanceExit, this.Position + this.ViewDistanceExit);
			var outerFocus = focus.IntersectWith(this.worldArea);

			// get subscribe area
			focus = new Bounds { Min = this.Position - this.ViewDistanceEnter, Max = this.Position + this.ViewDistanceEnter };
			var innerFocus = focus.IntersectWith(this.worldArea);

			innerFocus = this.world.GetRegionAlignedBoundingBox(innerFocus);

			if (innerFocus != this.currentRegionInnerFocus)
			{
				if (innerFocus.IsValid())
				{
					HashSet<Region> regions = this.currentRegionInnerFocus.IsValid()
												  ? this.world.GetRegionsExcept(innerFocus, this.currentRegionInnerFocus)
												  : this.world.GetRegions(innerFocus);
					this.SubscribeRegions(regions);
				}

				this.currentRegionInnerFocus = innerFocus;
			}

			outerFocus = this.world.GetRegionAlignedBoundingBox(outerFocus);
			if (outerFocus != this.currentRegionOuterFocus)
			{
				if (outerFocus.IsValid())
				{
					IEnumerable<Region> regions = this.currentRegionOuterFocus.IsValid()
													  ? (IEnumerable<Region>)this.world.GetRegionsExcept(this.currentRegionOuterFocus, outerFocus)
													  : this.subscribedWorldRegions.Keys.Where(r => !outerFocus.Contains(r.Coordinate)).ToArray();
					this.currentRegionOuterFocus = outerFocus;
					this.UnsubscribeRegions(regions);
				}
				else
				{
					this.currentRegionOuterFocus = outerFocus;
				}
			}
		}

		/// <summary>
		/// Unsubscribes an <see cref="MmoObject"/> manually.
		/// </summary>
		/// <param name="mmoObject"></param>
		public void UnsubscribeManually(MmoObject mmoObject)
		{
			lock (this.SyncRoot)
			{
				MmoObjectAutoSubscription subscription;
				if (this.autoManagedItemSubscriptions.TryGetValue(mmoObject, out subscription))
				{
					this.AutoUnsubscribeMmoObject(subscription);
				}
			}
		}

		/// <summary>
		/// Automatically subscribes an <see cref="MmoObject"/> manually.
		/// </summary>
		/// <param name="mmoObject"></param>
		public void AutoSubscribeManually(MmoObject mmoObject)
		{
			this.ReceiveMmoObjectSnapshot(mmoObject.GetMmoObjectSnapshot());
		}

		#endregion

		#region Local Methods

		/// <summary>
		/// Receives the <see cref="MmoObjectSnapshot"/> auto subscribes item if necessary.
		/// </summary>
		internal void ReceiveMmoObjectSnapshot(MmoObjectSnapshot message)
		{
			lock (SyncRoot)
			{
				// auto subscribe item?
				if (CanAutoSubscribeMmoObject(message.Source) == false)
					return;

				MmoObjectAutoSubscription existingSubscription;
				if (autoManagedItemSubscriptions.TryGetValue(message.Source, out existingSubscription))
				{
					// dropped out of world, unsubscribe
					if (message.WorldRegion == null)
					{
						this.AutoUnsubscribeMmoObject(existingSubscription);
						return;
					}

					// update position
					existingSubscription.Position = message.Position;
					existingSubscription.WorldRegion = message.WorldRegion;
					return;
				}

				// item not in view
				if (message.WorldRegion == null || this.subscribedWorldRegions.ContainsKey(message.WorldRegion) == false)
					return;

				// unsubscribe if item is disposed
				var disposalSubscription = message.Source.DisposeChannel.Subscribe(this.subscriptionManagementFiber, this.AutoSubscribedMmoObject_OnMmoObjectDisposed);
				// unsubscribe if item moves out of range
				var positionSubscription = message.Source.PositionUpdateChannel.SubscribeToLast(this.subscriptionManagementFiber,
				                                                                                this.AutoSubscribedMmoObject_OnMmoObjectPosition,
				                                                                                GameObjectAutoUnsubcribeDelayMilliseconds);
				var itemSubscription = new MmoObjectAutoSubscription(message.Source, message.Position, message.WorldRegion,
				                                                     new UnsubscriberCollection(disposalSubscription, positionSubscription));

				this.autoManagedItemSubscriptions.Add(message.Source, itemSubscription);
				this.OnMmoObjectSubscribed(message);
			}
		}

		/// <summary>
		///   Checks whether to auto subscribe the <paramref name = "mmoObject" />.
		///   The default implementation ignores the <see cref = "Owner" />.
		///   Override to change or extend this behavior.
		/// </summary>
		protected virtual bool CanAutoSubscribeMmoObject(MmoObject mmoObject)
		{
			return mmoObject != this.Owner && mmoObject.IsHiddenFor(Owner.Guid) == false;
		}

		/// <summary>
		///   The clear auto subscriptions.
		/// </summary>
		protected void ClearAutoSubscriptions()
		{
			foreach (MmoObjectAutoSubscription subscription in this.autoManagedItemSubscriptions.Values)
			{
				subscription.Dispose();
				this.OnMmoObjectUnsubscribed(subscription.MmoObject);
			}

			this.autoManagedItemSubscriptions.Clear();
		}

		/// <summary>
		///   The clear focus.
		/// </summary>
		protected void ClearRegionSubscriptions()
		{
			foreach (IDisposable subscription in this.subscribedWorldRegions.Values)
			{
				subscription.Dispose();
			}

			this.subscribedWorldRegions.Clear();
		}

		/// <summary>
		/// Does nothing.
		/// Called after subscribing an <see cref = "MmoObject" />.
		/// </summary>
		protected virtual void OnMmoObjectSubscribed(MmoObjectSnapshot itemSnapshot)
		{
		}

		/// <summary>
		/// Does nothing.
		/// Called after subscribing an <see cref = "MmoObject" />.
		/// </summary>
		protected virtual void OnMmoObjectUnsubscribed(MmoObject item)
		{
		}

		/// <summary>
		/// The attached item disposed.
		/// </summary>
		private void AttachedItem_OnMmoObjectDisposed(MmoObjectDisposedMessage message)
		{
			MessageCounters.CounterReceive.Increment();

			lock (this.SyncRoot)
			{
				if (message.Source == this.Owner)
				{
					this.itemMovementSubscription.Dispose();
					this.itemMovementSubscription = null;

					this.Owner = null;
				}
			}
		}

		/// <summary>
		/// The on attached item position update.
		/// </summary>
		private void AttachedMmoObject_OnGameObjectPosition(MmoObjectPositionMessage message)
		{
			MessageCounters.CounterReceive.Increment();

			lock (this.SyncRoot)
			{
				if (this.Owner == message.Source)
				{
					this.Position = message.Position;
					this.UpdateInterestManagement();
				}
			}
		}

		/// <summary>
		///   The on auto subscribed item disposed.
		/// </summary>
		private void AutoSubscribedMmoObject_OnMmoObjectDisposed(MmoObjectDisposedMessage message)
		{
			MessageCounters.CounterReceive.Increment();

			lock (this.SyncRoot)
			{
				MmoObjectAutoSubscription subscription;
				if (this.autoManagedItemSubscriptions.TryGetValue(message.Source, out subscription))
				{
					this.AutoUnsubscribeMmoObject(subscription);
				}
			}
		}

		/// <summary>
		///   The on auto subscribed item position update.
		///   unsubscribes item if too far away
		/// </summary>
		private void AutoSubscribedMmoObject_OnMmoObjectPosition(MmoObjectPositionMessage message)
		{
			MessageCounters.CounterReceive.Increment();

			lock (this.SyncRoot)
			{
				MmoObjectAutoSubscription subscription;

				// not subscribed
				if (false == this.autoManagedItemSubscriptions.TryGetValue(message.Source, out subscription))
				{
					return;
				}

				subscription.Position = message.Position;

				// dropped out of world, unsubscribe
				if (message.WorldRegion == null)
				{
					this.AutoUnsubscribeMmoObject(subscription);
					return;
				}

				// region is still the same, don't evaluate further
				if (message.WorldRegion == subscription.WorldRegion)
				{
					return;
				}

				subscription.WorldRegion = message.WorldRegion;

				if (this.subscribedWorldRegions.ContainsKey(subscription.WorldRegion) == false)
				{
					// unsubscribe if item is out of range
					this.AutoUnsubscribeDistantMmoObject(subscription);
				}
			}
		}

		/// <summary>
		///   The auto unsubscribe distant item.
		/// </summary>
		private void AutoUnsubscribeDistantMmoObject(MmoObjectAutoSubscription subscription)
		{
			if (false == this.currentRegionOuterFocus.Contains(subscription.Position))
			{
				this.AutoUnsubscribeMmoObject(subscription);
			}
		}

		/// <summary>
		///   The auto unsubscribe item.
		/// </summary>
		private void AutoUnsubscribeMmoObject(MmoObjectAutoSubscription subscription)
		{
			subscription.Dispose();
			this.autoManagedItemSubscriptions.Remove(subscription.MmoObject);
			this.OnMmoObjectUnsubscribed(subscription.MmoObject);
		}

		/// <summary>
		///   The region receive message.
		/// </summary>
		private void Region_OnReceive(RegionMessage message)
		{
			message.OnInterestAreaReceive(this);
		}

		/// <summary>
		///   Subscribes the <paramref name = "regions" />.
		/// </summary>
		private void SubscribeRegions(IEnumerable<Region> regions)
		{
			foreach (Region region in regions)
			{
				if (this.subscribedWorldRegions.ContainsKey(region) == false)
				{
					var subscription = region.SubscribeListener(this.subscriptionManagementFiber, this.Region_OnReceive);
					this.subscribedWorldRegions.Add(region, subscription);
					region.Publish(this.snapShotRequest);
				}
			}
		}

		/// <summary>
		///   Unsubscribe the <paramref name = "regions" />.
		/// </summary>
		private void UnsubscribeRegions(IEnumerable<Region> regions)
		{
			foreach (Region region in regions)
			{
				IDisposable subscription;
				if (this.subscribedWorldRegions.TryGetValue(region, out subscription))
				{
					subscription.Dispose();
					this.subscribedWorldRegions.Remove(region);
				}
			}

			var itemSubscriptions = this.autoManagedItemSubscriptions.Values.Where(i => regions.Contains(i.WorldRegion)).ToArray();
			foreach (var subscription in itemSubscriptions)
			{
				this.AutoUnsubscribeDistantMmoObject(subscription);
			}
		}

		#endregion
	}
}