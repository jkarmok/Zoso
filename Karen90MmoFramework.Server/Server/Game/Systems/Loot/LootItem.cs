using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public struct LootItem
	{
		public readonly short ItemId;
		public readonly int CountOrGold;
		public bool IsLooted;

		/// <summary>
		/// Creates a <see cref="LootItem"/> from a <see cref="LootItem"/>.
		/// </summary>
		public LootItem(LootItemData lootItemInfo)
		{
			this.ItemId = lootItemInfo.ItemId;
			this.CountOrGold = (byte) Utils.Rnd.Next(lootItemInfo.MinCount, lootItemInfo.MaxCount);
			this.IsLooted = false;
		}

		/// <summary>
		/// Creates a <see cref="LootItem"/> manually.
		/// </summary>
		public LootItem(short itemId, byte min, byte max)
		{
			this.ItemId = itemId;
			this.CountOrGold = (byte)Utils.Rnd.Next(min, max);
			this.IsLooted = false;
		}

		/// <summary>
		/// Creates a <see cref="LootItem"/> manually.
		/// </summary>
		public LootItem(short itemId, byte count)
		{
			this.ItemId = itemId;
			this.CountOrGold = count;
			this.IsLooted = false;
		}

		/// <summary>
		/// Creates a <see cref="LootItem"/> of gold.
		/// </summary>
		public LootItem(int gold)
		{
			ItemId = -1;
			CountOrGold = gold;

			IsLooted = false;
		}

		/// <summary>
		/// Returns the amount of gold if this is a gold item, 0 otherwise
		/// </summary>
		/// <returns></returns>
		public int TryGetGold()
		{
			return (ItemId == -1) ? CountOrGold : 0;
		}
	}
}
