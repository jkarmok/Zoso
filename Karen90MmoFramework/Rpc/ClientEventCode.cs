namespace Karen90MmoFramework.Rpc
{
	public enum ClientEventCode : byte
	{
		// login
		UserLoggedIn,
		UserLoggedOut,
		CharacterLoggedIn,
		CharacterLoggedOut,
		PlayerTransferring,
		PlayerTransferred,
		// world
		ObjectDestroyed,
		ObjectSubscribed,
		ObjectUnsubscribed,
		ObjectMovement,
		ObjectTransform,
		ObjectProperty,
		ObjectPropertyMultiple,
		ObjectFlagsSet,
		ObjectFlagsUnset,
		ObjectLevelUp,
		// self
		YouLevelUp,
		// interaction
		InteractionShop,
		InteractionShopList,
		InteractionLoot,
		InteractionLootList,
		InteractionDialogue,
		InteractionDialogueList,
		// inventory
		InventoryInit,
		InventoryItemAdded,
		InventoryItemAddedMultiple,
		InventoryItemRemoved,
		InventoryItemMoved,
		// actionbar
		ActionbarInit,
		ActionbarSlotRemoved,
		// spell
		SpellManagerInit,
		SpellAdded,
		SpellRemoved,
		SpellCastBegin,
		SpellCastEnd,
		SpellCooldownBegin,
		SpellCooldownEnd,
		// loot
		LootInit,
		LootClear,
		LootItemRemoved,
		LootGoldRemoved,
		// quest
		QuestManagerInit,
		QuestGiverStatus,
		QuestStarted,
		QuestFinished,
		QuestProgress,
		QuestRegress,
		QuestAdded,
		// chat
		ChatChannelJoined,
		ChatChannelLeft,
		ChatMessageReceived,
		// social
		SocialFriendAddedName,
		SocialFriendAddedNameMultiple,
		SocialFriendAddedData,
		SocialFriendAddedDataMultiple,
		SocialIgnoreAddedName,
		SocialIgnoreAddedNameMultiple,
		SocialProfileUpdate,
		SocialProfileUpdateMultiple,
		SocialProfileRemoved,
		SocialProfileStatus,
		SocialFriendRequestReceived,
		SocialFriendRequestCancelled,
		// group
		GroupInit,
		GroupMemberAdded,
		GroupMemberAddedGuid,
		GroupMemberAddedProperties,
		GroupMemberAddedInactive,
		GroupMemberRemoved,
		GroupMemberUpdate,
		GroupMemberDisconnected,
		GroupUninvited,
		GroupDisbanded,
		GroupInviteReceived,
		GroupInviteCancelled,
		GroupInviteDeclined,
	}
}
