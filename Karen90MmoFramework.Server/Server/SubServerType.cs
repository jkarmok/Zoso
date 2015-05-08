namespace Karen90MmoFramework.Server
{
	[System.Flags]
	public enum SubServerType : byte
	{
		/// <summary>
		/// Login Server
		/// </summary>
		Login			= 0x1,

		/// <summary>
		/// Chat Server
		/// </summary>
		Chat			= 0x2,

		/// <summary>
		/// World Server
		/// </summary>
		World			= 0x4,

		/// <summary>
		/// Social Server
		/// </summary>
		Social			= 0x8
	}
}
