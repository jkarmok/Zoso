using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface ILoot
	{
		/// <summary>
		/// Gets the loot items
		/// First index will ALWAYS contain GOLD.
		/// </summary>
		LootItem[] LootItems { get; }

		/// <summary>
		/// Determines whether the loot contains a certain item
		/// </summary>
		bool HasItem(short itemId);

		/// <summary>
		/// Collects a <see cref="LootItem"/> from an index and notifies the looter
		/// </summary>
		void CollectLootItem(int index, Player collector);

		/// <summary>
		/// Collects all the loot item and notifies the looter
		/// </summary>
		void CollectAll(Player collector);
	}
}
