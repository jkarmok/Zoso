namespace Karen90MmoFramework.Server.Master
{
	public enum ClientPeerState : sbyte
	{
		Disconnect		= -3,
		Disconnecting	= -2,
		WorldTransfer	= -1,
		Connect			= 0,
		Login			= 1,
		WorldEnter		= 2,
	}
}
