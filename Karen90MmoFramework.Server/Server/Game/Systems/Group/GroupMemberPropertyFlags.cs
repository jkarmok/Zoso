namespace Karen90MmoFramework.Server.Game.Systems
{
	[System.Flags]
	public enum GroupMemberPropertyFlags
	{
		PROPERTY_FLAG_NONE			= 0x00000000,
		PROPERTY_FLAG_NAME			= 0x00000001,	// string
		PROPERTY_FLAG_LEVEL			= 0x00000002,	// short
		PROPERTY_FLAG_MAX_HP		= 0x00000004,	// int
		PROPERTY_FLAG_CURR_HP		= 0x00000008,	// int
		PROPERTY_FLAG_AURAS			= 0x00000010,	// short[]
		PROPERTY_FLAG_POSITION		= 0x00000020,	// short, short
		PROPERTY_FLAG_STATUS		= 0x00000040,	// byte
		PROPERTY_FLAG_ALL			= 0x0000007F	// all
	};
}