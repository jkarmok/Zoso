using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Messages
{
	public class MmoObjectSnapshot : RegionMessage
	{
		private readonly MmoObject source;
		private readonly Region region;

		private readonly Vector3 position;
		private readonly Vector3 rotation;
		private readonly float[] coordinate;
		private readonly int revision;

        /// <summary>
		/// Creates a new instance of the <see cref="MmoObjectSnapshot"/> class.
        /// </summary>
		public MmoObjectSnapshot(MmoObject source, Vector3 position, Vector3 rotation, int revision, Region region)
		{
			this.source = source;
			this.position = position;
			this.rotation = rotation;
			this.revision = revision;
			this.region = region;

			this.coordinate = position.ToFloatArray(3);
		}

        /// <summary>
        /// Gets the <see cref = "Source" /> item's position.
        /// </summary>
		public Vector3 Position
        {
            get
            {
                return this.position;
            }
        }

		/// <summary>
		/// Gets the <see cref = "Source" /> item's rotation.
		/// </summary>
		public Vector3 Rotation
		{
			get
			{
				return this.rotation;
			}
		}

		/// <summary>
		/// Coordinate
		/// </summary>
		public float[] Coordinate
		{
			get
			{
				return this.coordinate;
			}
		}

        /// <summary>
        /// Gets the source <see cref = "MmoObject" />.
        /// </summary>
        public MmoObject Source
        {
            get
            {
                return this.source;
            }
        }

		/// <summary>
		/// Gets the properties revision. Revision of <value>0</value> means the <see cref="MmoObject"/> has no properties.
		/// </summary>
		public int Revision
		{
			get
			{
				return this.revision;
			}
		}

        /// <summary>
        /// Gets the current <see cref = "Region" /> where the <see cref = "Source" /> item is located.
        /// </summary>
        public Region WorldRegion
        {
            get
            {
                return this.region;
            }
        }

        /// <summary>
        ///   Increments <see cref = "MessageCounters.CounterReceive" /> and subscribes the <see cref = "InterestArea" /> to the <see cref = "Source" /> item if it not already subscribed or attached.
        ///   Called by the <see cref = "InterestArea" /> when received.
        /// </summary>
        public override void OnInterestAreaReceive(InterestArea interestArea)
        {
            interestArea.ReceiveMmoObjectSnapshot(this);
        }

        /// <summary>
        ///   Increments <see cref = "MessageCounters.CounterReceive" />.
        ///   Called by the <see cref = "MmoObject" /> when received.
        /// </summary>
		public override void OnMmoObjectReceive(MmoObject gameObject)
        {
        }
    }
}