namespace Karen90MmoFramework.Server.ServerToServer
{
	public enum ServerEventCode : byte
	{
		AddSession					= 10,
		RemoveSession				= 11,
		// group
		GroupFormed					= 12,
		GroupMemberAdded			= 13,
		GroupMemberAddedSession		= 14,
		GroupMemberOnline			= 15,
		GroupMemberOffline			= 16,
		GroupMemberRemoved			= 17,
		GroupDisbanded				= 18,
	}
}
