using System;

using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game
{
    internal class MmoObjectAutoSubscription : IDisposable
    {
        #region Constants and Fields

        private readonly MmoObject mmoObject;
        private readonly IDisposable subscription;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref = "MmoObjectAutoSubscription" /> class.
        /// </summary>
		public MmoObjectAutoSubscription(MmoObject mmoObject, Vector3 position, Region region, IDisposable subscription)
        {
			this.Position = position;
            this.mmoObject = mmoObject;
            this.subscription = subscription;
			this.WorldRegion = region;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="MmoObject"/>.
        /// </summary>
        public MmoObject MmoObject
        {
            get
            {
                return this.mmoObject;
            }
        }
		
        /// <summary>
        /// Gets or sets the <see cref="MmoObject"/>'s position.
        /// </summary>
		public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets WorldRegion.
        /// </summary>
        public Region WorldRegion { get; set; }

        #endregion

		#region Implementation of IDisposable

		/// <summary>
        /// Disposes the subscription.
        /// </summary>
        public void Dispose()
        {
            this.subscription.Dispose();
        }

        #endregion
    }
}