using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Messages
{
    public sealed class MmoObjectSnapshotRequest : RegionMessage
    {
        /// <summary>
        ///   The source.
        /// </summary>
        private readonly InterestArea source;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MmoObjectSnapshotRequest" /> class.
        /// </summary>
        public MmoObjectSnapshotRequest(InterestArea source)
        {
            this.source = source;
        }

        /// <summary>
        ///   Gets the source.
        /// </summary>
        public InterestArea Source
        {
            get
            {
                return this.source;
            }
        }

        /// <summary>
        ///   Called by the <see cref = "InterestArea" /> when received.
        ///   Increments <see cref = "MessageCounters.CounterReceive" />.
        /// </summary>
        public override void OnInterestAreaReceive(InterestArea interestArea)
        {
        }

        /// <summary>
        ///   Called by the <see cref = "MmoObject" /> when received.
        ///   Increments <see cref = "MessageCounters.CounterReceive" /> and publishes an <see cref = "MmoObjectPositionMessage" /> in the <paramref name = "item" />'s <see cref = "MmoObject.CurrentRegion" />.
        /// </summary>
        public override void OnMmoObjectReceive(MmoObject gameObject)
        {
			MmoObjectSnapshot gameObjectSnapshot = gameObject.GetMmoObjectSnapshot();
			this.source.ReceiveMmoObjectSnapshot(gameObjectSnapshot);
        }
    }
}