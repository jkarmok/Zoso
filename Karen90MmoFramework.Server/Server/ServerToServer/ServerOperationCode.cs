namespace Karen90MmoFramework.Server.ServerToServer
{
	public enum ServerOperationCode : byte
	{
		// PingRequest = 0,

		// general
		RegisterServer					= 10,
		AcquireServerAddress			= 11,
		KillSession						= 12,
		KillClient						= 13,
		// login
		AckClientUserLogin				= 14,
		AckClientCharacterLogin			= 15,
		// world
		AckClientPlayerTransferWorld	= 16,
		TransferSession					= 17,
		// chat
		CreateChannel					= 18,
		RemoveChannel					= 19,
		JoinChannel						= 20,
		LeaveChannel					= 21,
		// social
		UpdateSocialProfile				= 22,
		RegisterWorld					= 23,
		UnregisterWorld					= 24,
		JoinWorld						= 25,
		LeaveWorld						= 26,
	}
}
