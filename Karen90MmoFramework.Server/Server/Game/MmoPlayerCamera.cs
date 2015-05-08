using System;
using System.Collections.Generic;

using ExitGames.Concurrency.Fibers;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Game.Objects;
using Karen90MmoFramework.Server.Game.Messages;
using Karen90MmoFramework.Server.ServerToClient.Events;

namespace Karen90MmoFramework.Server.Game
{
	public class MmoPlayerCamera : InterestArea
	{
		#region Constants and Fields

		private readonly Dictionary<MmoObject, IDisposable> eventChannelSubscriptions;
		private readonly IFiber fiber;
		private readonly IPeer receiver;

		#endregion

		#region Constructors and Destructors

		public MmoPlayerCamera(WorldSession player, MmoZone zone)
			: base(zone, player.Fiber)
		{
			this.receiver = player;
			this.fiber = player.Fiber;
			this.eventChannelSubscriptions = new Dictionary<MmoObject, IDisposable>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Moves the camera
		/// </summary>
		public void Move(Vector3 position)
		{
			this.Position = position;
			this.UpdateInterestManagement();
		}

		#endregion

		#region InterestArea Callbacks

		/// <summary>
		/// Subscribes the player on the item's event channel and notifies the client
		/// </summary>
		protected override void OnMmoObjectSubscribed(MmoObjectSnapshot snapshot)
		{
			var mmoObject = snapshot.Source;

			// publish event messages
			var eventSubscription = mmoObject.EventChannel.Subscribe(this.fiber, this.SubscribedItem_OnGameObjectEvent);
			this.eventChannelSubscriptions.Add(mmoObject, eventSubscription);

			var eventSubscribed = new ObjectSubscribed
				{
					ObjectId = mmoObject.Guid,
					Position = snapshot.Coordinate,
					Orientation = snapshot.Rotation.Y,
				};

			var objectFlags = mmoObject.Flags;
			if ((objectFlags & MmoObjectFlags.HasProperties) == MmoObjectFlags.HasProperties)
			{
				eventSubscribed.Revision = snapshot.Revision;
				if ((objectFlags & MmoObjectFlags.SendPropertiesOnSubscribe) == MmoObjectFlags.SendPropertiesOnSubscribe)
					lock(mmoObject) // locking is important
						eventSubscribed.Properties = mmoObject.GetProperties();
			}

			switch ((ObjectType)mmoObject.Guid.Type)
			{
				case ObjectType.Gameobject:
				case ObjectType.Npc:
				case ObjectType.Dynamic:
					{
						var interestFlags = this.Owner.GetFlags(mmoObject);
						if (interestFlags != InterestFlags.None)
							eventSubscribed.Flags = (int) interestFlags;
					}
					break;
			}

			this.receiver.SendEvent(eventSubscribed, new MessageParameters { ChannelId = PeerSettings.MmoObjectEventChannel });
			this.Owner.OnSubscribe(mmoObject);
		}

		/// <summary>
		/// Unsubscribes the player from the item's event channel and notifies the client
		/// </summary>
		protected override void OnMmoObjectUnsubscribed(MmoObject mmoObject)
		{
			IDisposable messageReceiver = this.eventChannelSubscriptions[mmoObject];
			this.eventChannelSubscriptions.Remove(mmoObject);
			messageReceiver.Dispose();

			// when there is no owner there is no need to send an event to client
			if (Owner == null)
				return;

			this.receiver.SendEvent(new ObjectUnsubscribed {ObjectId = mmoObject.Guid}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			this.Owner.OnUnsubscribe(mmoObject);
		}

		#endregion

		#region Helper Methods

		/// <summary>
		///   The subscribed item event.
		/// </summary>
		private void SubscribedItem_OnGameObjectEvent(MmoObjectEventMessage message)
		{
			this.receiver.SendEvent(message.EventData, message.Parameters);
		}

		#endregion
	}
}
