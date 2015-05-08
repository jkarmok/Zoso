namespace Karen90MmoFramework.Game
{
	[System.Flags]
	public enum InteractionFlags : byte
	{
		None				= 0x0,
		SendLoot			= 0x1,
		SendInventory		= 0x2,
		SendDialogue		= 0x4,
		SendAll				= SendLoot | SendInventory | SendDialogue,
	}
}
