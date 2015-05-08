namespace Karen90MmoFramework.Rpc
{
	public enum GameOperationCode : byte
	{
		// system
		InvalidOperation,
		IgnoreOperation,
		// login
		LoginUser,
		LogoutUser,
		CreateUser,
		// character
		LoginCharacter,
		LogoutCharacter,
		CreateCharacter,
		DeleteCharacter,
		RetrieveCharacters,
		// world
		EnterWorld,
		Move,
		Rotate,
		GetProperties,
		// interaction
		Interact,
		CloseInteraction,
		PurchaseItem,
		SellItem,
		LootItem,
		LootGold,
		LootAll,
		SelectDialogue,
		AcceptQuest,
		TurnInQuest,
		// inventory
		MoveInventorySlot,
		UseInventorySlot,
		// actionbar
		AddActionbarItem,
		AddActionbarSpell,
		MoveActionbarSlot,
		UseActionbarSlot,
		// spell
		CastSpell,
		// chat
		SendChat,
		// social
		SendFriendRequest,
		AcceptFriendRequest,
		DeclineFriendRequest,
		RemoveFriend,
		// group
		SendGroupInvite,
		AcceptGroupInvite,
		DeclineGroupInvite,
		KickGroupMember,
		LeaveGroup,
		DisbandGroup,
		// special
		SpHeal,
		SpDamage,
		SpAddPower,
		SpRemovePower,
		SpKill,
		SpAddXp,
		SpAddSpell,
		SpRemoveSpell,
		SpAddGold,
		SpRemoveGold,
	}
}
