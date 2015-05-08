using ExitGames.Diagnostics.Counter;

using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Messages
{
	/// <summary>
    ///   Contains counters that keep track of the amount of messages sent and received from <see cref = "MmoObject" /> channels.
    /// </summary>
    public static class MessageCounters
    {
        #region Constants and Fields

        /// <summary>
        ///   Used to count how many messages were received by <see cref = "InterestArea">interest areas</see> (and sometimes <see cref = "MmoObject">items</see>).
        ///   Name: "ItemMessage.Receive"
        /// </summary>
        public static readonly CountsPerSecondCounter CounterReceive = new CountsPerSecondCounter("GameObjectMessage.Receive");

        /// <summary>
        ///   Used to count how many messages were sent by <see cref = "MmoObject">items</see> (and sometimes <see cref = "InterestArea">interest areas</see>).
        ///   Name: "ItemMessage.Send"
        /// </summary>
		public static readonly CountsPerSecondCounter CounterSend = new CountsPerSecondCounter("GameObjectMessage.Send");

        #endregion
    }
}