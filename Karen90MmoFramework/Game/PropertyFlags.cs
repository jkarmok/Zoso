namespace Karen90MmoFramework.Game
{
	[System.Flags]
	public enum PropertyFlags
	{
		None			= 0x0,	// none
		Name			= (1 << (PropertyCode.Name + 1)),
		Species			= (1 << (PropertyCode.Species + 1)),
		Race			= (1 << (PropertyCode.Race + 1)),
		Origin			= (1 << (PropertyCode.Origin + 1)),
		Level			= (1 << (PropertyCode.Level + 1)),
		Alignment		= (1 << (PropertyCode.Alignment + 1)),
		NpcType			= (1 << (PropertyCode.NpcType + 1)),
		MaxHp			= (1 << (PropertyCode.MaxHp + 1)),
		MaxPow			= (1 << (PropertyCode.MaxPow + 1)),
		CurrHp			= (1 << (PropertyCode.CurrHp + 1)),
		CurrPow			= (1 << (PropertyCode.CurrPow + 1)),
		ConversationId	= (1 << (PropertyCode.ConversationId + 1)),
		Money			= (1 << (PropertyCode.Money + 1)),
		FlagXp			= (1 << (PropertyCode.Xp + 1)),
		Stats			= (1 << (PropertyCode.Stats + 1)),
		UnitState		= (1 << (PropertyCode.UnitState + 1)),
		GoType			= (1 << (PropertyCode.GoType + 1)),

		/// <summary>
		/// actor properties which are static
		/// </summary>
		ActorStatic = Name | Species | Race | Origin,

		/// <summary>
		/// actor properties which are not static
		/// </summary>
		ActorNonStatic = Level | MaxHp | CurrHp | UnitState,

		/// <summary>
		/// all global actor properties
		/// </summary>
		ActorAll = ActorStatic | ActorNonStatic,

		/// <summary>
		/// npc properties which are static. these will be stored inside client.
		/// </summary>
		NpcStatic = Name | Level | Species | Race | Alignment | NpcType,

		/// <summary>
		/// npc properties which are not static
		/// </summary>
		NpcNonStatic = MaxHp | CurrHp | UnitState,

		/// <summary>
		/// all global npc properties
		/// </summary>
		NpcAll = NpcStatic | NpcNonStatic,

		GameobjectStatic = None,
		GameobjectNonStatic = None,
		GameobjectAll = None,

		ElementStatic = None,
		ElementNonStatic = None,
		ElementAll = None,
	}
}
