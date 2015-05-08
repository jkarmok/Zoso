namespace Karen90MmoFramework.Server.Game
{
	[System.Flags]
	public enum MmoObjectFlags
	{
		/// <summary>
		/// No flags
		/// </summary>
		None						= 0x0,

		/// <summary>
		/// The object has properties
		/// </summary>
		HasProperties				= 0x1,

		/// <summary>
		/// Send the properties to the client right after subscription
		/// </summary>
		SendPropertiesOnSubscribe	= 0x2,
	}
}
