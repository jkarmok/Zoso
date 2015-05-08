using Karen90MmoFramework.Server.Game.Objects;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Server.Game.Messages
{
    public sealed class AggroSnapshotRequest : RegionMessage
    {
		private readonly IEngager source;

        /// <summary>
		/// Creates a new <see cref="AggroSnapshotRequest"/>.
        /// </summary>
		public AggroSnapshotRequest(IEngager source)
        {
            this.source = source;
        }

        /// <summary>
		/// Gets the <see cref="IEngager"/>.
        /// </summary>
		public IEngager Source
        {
            get
            {
                return this.source;
            }
        }

		/// <summary>
		/// Called by the <see cref="InterestArea"/> when received.
		/// </summary>
		/// <param name="interestArea"></param>
        public override void OnInterestAreaReceive(InterestArea interestArea)
        {
        }

		/// <summary>
		/// Called by the <see cref="MmoObject"/> when received.
		/// </summary>
		/// <param name="gameObject"></param>
        public override void OnMmoObjectReceive(MmoObject gameObject)
        {
			this.source.ProcessThreat(gameObject);
        }
    }
}