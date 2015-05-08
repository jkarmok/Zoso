using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Game.Items
{
	public interface IMmoItem
	{
		/// <summary>
		/// Gets the item id
		/// </summary>
		short Id { get; }

		/// <summary>
		/// Gets the item type
		/// </summary>
		MmoItemType Type { get; }

		/// <summary>
		/// Gets the item level
		/// </summary>
		int Level { get; }

		/// <summary>
		/// Gets the buyout price
		/// </summary>
		int BuyoutPrice { get; }

		/// <summary>
		/// Gets the sell price
		/// </summary>
		int SellPrice { get; }

		/// <summary>
		/// Gets the item rarity
		/// </summary>
		Rarity Rareness { get; }

		/// <summary>
		/// Gets the max stack count
		/// </summary>
		int MaxStack { get; }

		/// <summary>
		/// Gets the item  count
		/// </summary>
		int StackCount { get; }

		/// <summary>
		/// Gets the usable status
		/// </summary>
		bool IsUsable { get; }

		/// <summary>
		/// Gets the use limit
		/// </summary>
		UseLimit UseLimit { get; }

		/// <summary>
		/// Gets the spell id
		/// </summary>
		short SpellId { get; }
	}
}
