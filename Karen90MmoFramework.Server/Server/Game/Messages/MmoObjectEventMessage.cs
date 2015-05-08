using Photon.SocketServer;

using ExitGames.Diagnostics.Counter;

using Karen90MmoFramework.Server.Game.Objects;
using Karen90MmoFramework.Server.ServerToClient;

namespace Karen90MmoFramework.Server.Game.Messages
{
    /// <summary>
    ///   This message type contains <see cref = "EventData" /> to be sent to clients.
    ///   <see cref = "MmoObjectEventMessage">ItemEventMessages</see> are published through the item <see cref = "MmoObject.EventChannel" />.
    /// </summary>
    public class MmoObjectEventMessage
    {
        #region Constants and Fields

        /// <summary>
        ///   The event data.
        /// </summary>
        private readonly GameEvent eventData;

        /// <summary>
        ///   The send parameters.
        /// </summary>
        private readonly MessageParameters parameters;

        /// <summary>
        ///   The source.
        /// </summary>
        private readonly MmoObject source;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MmoObjectEventMessage" /> class.
        /// </summary>
        /// <param name = "source">
        ///   The source.
        /// </param>
        /// <param name = "eventData">
        ///   The event data.
        /// </param>
		/// <param name = "parameters">
        ///   The message parameters
        /// </param>
        public MmoObjectEventMessage(MmoObject source, GameEvent eventData, MessageParameters parameters)
        {
            this.source = source;
            this.eventData = eventData;
			this.parameters = parameters;
        }

        #endregion

        #region Properties

        /// <summary>
		///   Gets the <see cref = "GameEvent" /> to be sent to the client.
        /// </summary>
        public GameEvent EventData
        {
            get
            {
                return this.eventData;
            }
        }

        /// <summary>
        ///   Gets the message parameters.
        /// </summary>
        public MessageParameters Parameters
        {
            get
            {
				return this.parameters;
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

        #endregion
    }
}