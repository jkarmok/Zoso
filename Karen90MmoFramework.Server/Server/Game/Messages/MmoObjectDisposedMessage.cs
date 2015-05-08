using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Messages
{
    /// <summary>
    ///   This type of message is pubished by <see cref = "MmoObject">items</see> sent through the item <see cref = "MmoObject.DisposeChannel" />. 
    ///   <see cref = "InterestArea">Interest areas</see> unsubscribe the sender when they receive this message.
    /// </summary>
    public sealed class MmoObjectDisposedMessage
    {
        #region Constants and Fields

        /// <summary>
        ///   The source.
        /// </summary>
        private readonly MmoObject source;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MmoObjectDisposedMessage" /> class.
        /// </summary>
        /// <param name = "source">
        ///   The source.
        /// </param>
        public MmoObjectDisposedMessage(MmoObject source)
        {
            this.source = source;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the source <see cref = "MmoObject" />.
        /// </summary>
        public MmoObject Source
        {
            get
            {
                return this.source;
            }
        }

        #endregion
    }
}