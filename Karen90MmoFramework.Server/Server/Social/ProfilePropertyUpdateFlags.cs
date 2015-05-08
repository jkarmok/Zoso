namespace Karen90MmoFramework.Server.Social
{
	[System.Flags]
	public enum ProfilePropertyUpdateFlags : byte
	{
		None		= 0x0,
		Name		= 0x1,
		Class		= 0x2,
		Level		= 0x4,
		GmLevel		= 0x8,
		WorldId		= 0x10,
	}
}
