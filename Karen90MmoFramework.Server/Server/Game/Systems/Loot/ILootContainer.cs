using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface ILootContainer
	{
		/// <summary>
		/// Generates loot for (n)n <see cref="Player"/>.
		/// </summary>
		void GenerateLootFor(Player player);

		/// <summary>
		/// Tells whether the <see cref="Player"/> has any loot in the container.
		/// </summary>
		bool HasLootFor(Player player);

		/// <summary>
		/// Gets the loot for a certain <see cref="Player"/>.
		/// </summary>
		ILoot GetLootFor(Player player);

		/// <summary>
		/// Removes any generated loot for a certain <see cref="Player"/>.
		/// </summary>
		/// <param name="player"></param>
		void RemoveLootFor(Player player);

		/// <summary>
		/// Clear all generated loots
		/// </summary>
		void Clear();
	}
}
