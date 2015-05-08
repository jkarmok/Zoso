using System.Collections.Generic;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Editor
{
	public interface IGameDataHandler
	{
		/// <summary>
		/// Loaded Game Items
		/// </summary>
		Dictionary<int, GameItemData> MmoItems { get; }

		/// <summary>
		/// Loaded Auras
		/// </summary>
		Dictionary<short, AuraData> Auras { get; }

		/// <summary>
		/// Loaded Spells
		/// </summary>
		Dictionary<short, SpellData> Spells { get; }

		/// <summary>
		/// Loaded Quests
		/// </summary>
		Dictionary<short, QuestData> Quests { get; }

		/// <summary>
		/// Loaded Npcs
		/// </summary>
		Dictionary<int, NpcData> Npcs { get; }

		/// <summary>
		/// Loaded Game Objects
		/// </summary>
		Dictionary<int, GameObjectData> GameObjects { get; }

		/// <summary>
		/// Loaded Loot Groups
		/// </summary>
		Dictionary<short, LootGroupData> LootGroups { get; }

		/// <summary>
		/// Stores an MmoItem to DB
		/// </summary>
		bool StoreMmoItem(GameItemData mmoItem);

		/// <summary>
		/// Removes an MmoItem from DB
		/// </summary>
		bool RemoveMmoItem(int itemId);

		/// <summary>
		/// Stores an Npc to DB
		/// </summary>
		bool StoreNpc(NpcData npc);

		/// <summary>
		/// Removes an Npc from DB
		/// </summary>
		bool RemoveNpc(int npcId);

		/// <summary>
		/// Stores a GameObject to DB
		/// </summary>
		bool StoreGO(GameObjectData go);

		/// <summary>
		/// Removes a GameObject from DB
		/// </summary>
		bool RemoveGO(int goId);

		/// <summary>
		/// Stores an Aura to DB
		/// </summary>
		bool StoreAura(AuraData aura);

		/// <summary>
		/// Removes an Aura from DB
		/// </summary>
		bool RemoveAura(short auraId);

		/// <summary>
		/// Stores a Spell to DB
		/// </summary>
		bool StoreSpell(SpellData spell);

		/// <summary>
		/// Removes a Spell from DB
		/// </summary>
		bool RemoveSpell(short spellId);

		/// <summary>
		/// Stores a Quest to DB
		/// </summary>
		bool StoreQuest(QuestData quest);

		/// <summary>
		/// Removes a Quest from DB
		/// </summary>
		bool RemoveQuest(short questId);

		/// <summary>
		/// Stores a loot group to DB
		/// </summary>
		bool StoreLootGroup(LootGroupData lootGroup);

		/// <summary>
		/// Removes a loot group from DB
		/// </summary>
		bool RemoveLootGroup(short groupId);
	}
}
