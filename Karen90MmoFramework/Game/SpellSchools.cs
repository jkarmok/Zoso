namespace Karen90MmoFramework.Game
{
	/// <summary>
	/// Base school the spell belongs to
	/// </summary>
	[System.Flags]
	public enum SpellSchools : byte
	{
		None						= 0x0,
		Normal						= 0x1,				// physical, items, misc.
		Fire						= 0x2,
		Water						= 0x4,
		Wind						= 0x8,
		Earth						= 0x10,
		Void						= 0x20,

		/// <summary>
		/// Fire | Water | Wind | Earth | Void
		/// </summary>
		Magic = Fire | Water | Wind | Earth | Void,
	};
}