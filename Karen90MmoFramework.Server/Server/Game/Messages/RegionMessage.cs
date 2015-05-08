using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Messages
{
    /// <summary>
    ///   Abstract class for messages that are sent through <see cref = "Region">regions</see>. 
    ///   These messages are received by <see cref = "MmoObject">items</see> and <see cref = "InterestArea">interest areas</see>. 
	///   The receiver does not know what kind of message he receives and calls either <see cref = "OnMmoObjectReceive" /> or <see cref = "OnInterestAreaReceive" /> to dispatch it.
    /// </summary>
    public abstract class RegionMessage
    {
        #region Public Methods

        /// <summary>
        ///   Called by the <see cref = "InterestArea" /> when received.
        /// </summary>
        public abstract void OnInterestAreaReceive(InterestArea interestArea);

        /// <summary>
        ///   Called by the <see cref = "MmoObject" /> when received.
        /// </summary>
		public abstract void OnMmoObjectReceive(MmoObject gameObject);

        #endregion
    }
}