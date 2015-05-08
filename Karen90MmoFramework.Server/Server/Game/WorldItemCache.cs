using System.Collections.Generic;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Server.Game
{
	public interface IWorldItemCacheAccessor
	{
		/// <summary>
		/// Tries to retrieve a <see cref="LootGroupData"/>.
		/// </summary>
		bool TryGetLootGroupData(short lootGroupId, out LootGroupData lootInfo);

		/// <summary>
		/// Tries to retrieve a <see cref="GameItemData"/>.
		/// </summary>
		bool TryGetGameItemData(short mmoItemId, out GameItemData mmoItem);

		/// <summary>
		/// Tries to retrieve a <see cref="SpellData"/>.
		/// </summary>
		bool TryGetSpellData(short spellId, out SpellData spell);

		/// <summary>
		/// Tries to retrieve a <see cref="QuestData"/>.
		/// </summary>
		bool TryGetQuestData(short questId, out QuestData quest);
	}

	/// <summary>
	/// A non-thread safe collection for frequently used database data designed to be fast because of the non-thread safe model.
	/// Items should be added at the beginning of the game and should not be modified after its added. If items need to added after zone is active use
	/// <see cref="ConcurrentStorageMap{TKey0, TKey1, TValue}"/> instead.
	/// </summary>
	public class WorldItemCache : IWorldItemCacheAccessor
	{
		#region Constants and Fields

		private readonly Dictionary<short, GameItemData> gameItemCache;
		private readonly Dictionary<short, SpellData> spellCache;
		private readonly Dictionary<short, QuestData> questCache;
		private readonly Dictionary<short, LootGroupData> lootGroupCache;

		#endregion

		#region Constructors and Destructors

		public WorldItemCache()
		{
			this.gameItemCache = new Dictionary<short, GameItemData>();
			this.spellCache = new Dictionary<short, SpellData>();
			this.questCache = new Dictionary<short, QuestData>();
			this.lootGroupCache = new Dictionary<short, LootGroupData>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a <see cref="LootGroupData"/>.
		/// </summary>
		public void AddLootGroupData(short lootGroupId, LootGroupData lootGroupData)
		{
			this.lootGroupCache.Add(lootGroupId, lootGroupData);
		}

		/// <summary>
		/// Tries to retrieve a <see cref="LootGroupData"/>.
		/// </summary>
		public bool TryGetLootGroupData(short lootGroupId, out LootGroupData lootGroupData)
		{
			return this.lootGroupCache.TryGetValue(lootGroupId, out lootGroupData);
		}

		/// <summary>
		/// Removes a <see cref="LootGroupData"/>.
		/// </summary>
		public void RemoveLootGroupData(short lootGroupId)
		{
			this.lootGroupCache.Remove(lootGroupId);
		}

		/// <summary>
		/// Adds a <see cref="GameItemData"/>.
		/// </summary>
		public void AddGameItemData(short gameItemId, GameItemData gameItemData)
		{
			this.gameItemCache.Add(gameItemId, gameItemData);
		}

		/// <summary>
		/// Tries to retrieve a <see cref="GameItemData"/>.
		/// </summary>
		public bool TryGetGameItemData(short gameItemId, out GameItemData mmoItem)
		{
			return this.gameItemCache.TryGetValue(gameItemId, out mmoItem);
		}

		/// <summary>
		/// Adds a <see cref="GameItemData"/>.
		/// </summary>
		public void RemoveGameItemData(short gameItemId)
		{
			this.gameItemCache.Remove(gameItemId);
		}

		/// <summary>
		/// Adds a <see cref="SpellData"/>.
		/// </summary>
		public void AddSpellData(short spellId, SpellData spellData)
		{
			this.spellCache.Add(spellId, spellData);
		}

		/// <summary>
		/// Tries to retrieve a <see cref="SpellData"/>.
		/// </summary>
		public bool TryGetSpellData(short spellId, out SpellData spellData)
		{
			return this.spellCache.TryGetValue(spellId, out spellData);
		}

		/// <summary>
		/// Adds a <see cref="SpellData"/>.
		/// </summary>
		public void RemoveSpellData(short spellId)
		{
			this.spellCache.Remove(spellId);
		}

		/// <summary>
		/// Adds a <see cref="QuestData"/>.
		/// </summary>
		public void AddQuestData(short questId, QuestData questData)
		{
			this.questCache.Add(questId, questData);
		}

		/// <summary>
		/// Tries to retrieve a <see cref="QuestData"/>.
		/// </summary>
		public bool TryGetQuestData(short questId, out QuestData questData)
		{
			return this.questCache.TryGetValue(questId, out questData);
		}

		/// <summary>
		/// Adds a <see cref="QuestData"/>.
		/// </summary>
		public void RemoveQuestData(short questId)
		{
			this.questCache.Remove(questId);
		}

		#endregion
	}
}
