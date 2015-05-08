using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Messages
{
    /// <summary>
    ///   This message contains the current position of the <see cref = "MmoObject" />. 
    ///   This type of message is published by <see cref = "MmoObject">items</see> through the <see cref = "MmoObject.PositionUpdateChannel" />. 
    ///   <para>
    ///     <see cref = "InterestArea">Interest areas</see> that receive this message use it to either follow the sender (the attached item) or to decide whether to unsubscribe.
    ///   </para>
    /// </summary>
    public class MmoObjectPositionMessage
    {
        #region Constants and Fields

		private readonly MmoObject source;
		private readonly Region worldRegion;

		private readonly Vector3 position;

        #endregion

        #region Constructors and Destructors

        /// <summary>
		/// Creates a new <see cref="MmoObjectPositionMessage"/>.
        /// </summary>
		public MmoObjectPositionMessage(MmoObject source, Vector3 position, Region worldRegion)
        {
            this.source = source;
            this.position = position;
            this.worldRegion = worldRegion;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the <see cref = "Source" /> item's position.
        /// </summary>
		public Vector3 Position
        {
            get
            {
                return this.position;
            }
        }

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

        /// <summary>
        ///   Gets current <see cref = "Region" /> where the <see cref = "Source" /> item is located.
        /// </summary>
        public Region WorldRegion
        {
            get
            {
                return this.worldRegion;
            }
        }

        #endregion
    }
}