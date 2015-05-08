using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Game.Messages
{
	public struct SocialStatusChangedMessage
	{
		private readonly MmoGuid guid;
		private readonly SocialStatus status;

		/// <summary>
		/// Creates a new instance of the <see cref="SocialStatusChangedMessage"/> message
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="status"></param>
		public SocialStatusChangedMessage(MmoGuid guid, SocialStatus status)
		{
			this.guid = guid;
			this.status = status;
		}

		/// <summary>
		/// Gets the guid
		/// </summary>
		public MmoGuid Guid
		{
			get
			{
				return this.guid;
			}
		}

		/// <summary>
		/// Gets social status
		/// </summary>
		public SocialStatus Status
		{
			get
			{
				return this.status;
			}
		}
	}
}
