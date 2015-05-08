using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Server.Game.Items
{
	public class MmoItem : IMmoItem, IStackable
	{
		#region Constants and Fields

		public const int INFINITE = 0;
		
		private readonly short id;
		private readonly MmoItemType type;
		private readonly short level;
		private readonly int buyoutPrice;
		private readonly int sellPrice;
		private readonly Rarity rareness;
		private readonly int maxStack;
		private readonly UseLimit useLimit;
		private readonly short spellId;

		private int count;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the item id
		/// </summary>
		public short Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the item type
		/// </summary>
		public MmoItemType Type
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// Gets the item level
		/// </summary>
		public int Level
		{
			get
			{
				return this.level;
			}
		}

		/// <summary>
		/// Gets the buyout price
		/// </summary>
		public int BuyoutPrice
		{
			get
			{
				return this.buyoutPrice;
			}
		}

		/// <summary>
		/// Gets the sell price
		/// </summary>
		public int SellPrice
		{
			get
			{
				return this.sellPrice;
			}
		}

		/// <summary>
		/// Gets the item rarity
		/// </summary>
		public Rarity Rareness
		{
			get
			{
				return this.rareness;
			}
		}

		/// <summary>
		/// Gets the spell id
		/// </summary>
		public short SpellId
		{
			get
			{
				return this.spellId;
			}
		}

		/// <summary>
		/// Gets the usable status
		/// </summary>
		public bool IsUsable
		{
			get
			{
				return this.spellId > 0;
			}
		}

		/// <summary>
		/// Gets the use limit
		/// </summary>
		public UseLimit UseLimit
		{
			get
			{
				return this.useLimit;
			}
		}

		/// <summary>
		/// Gets the difference between current count and the max
		/// </summary>
		public int MaxDifference
		{
			get
			{
				return this.maxStack - this.count;
			}
		}

		/// <summary>
		/// Gets whether the item can be stacked more or not
		/// </summary>
		public bool CanStackMore
		{
			get
			{
				return this.IsStackable && !this.IsStackMaxed;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="MmoItem"/> class from <see cref="GameItemData"/>.
		/// </summary>
		public MmoItem(GameItemData itemData)
		{
			this.id = itemData.ItemId;
			this.type = (MmoItemType) itemData.ItemType;
			this.level = itemData.Level;
			this.rareness = (Rarity) itemData.Rarity;
			
			this.sellPrice = itemData.SellPrice;
			this.buyoutPrice = itemData.BuyoutPrice;

			this.maxStack = itemData.MaxStack;
			this.useLimit = (UseLimit) itemData.UseLimit;
			this.spellId = itemData.UseSpellId;

			this.count = 1;
		}

		#endregion

		#region ICollectible Implementation

		/// <summary>
		/// Gets the max stack count
		/// </summary>
		public int MaxStack
		{
			get
			{
				return this.maxStack;
			}
		}

		/// <summary>
		/// Gets the item  count
		/// </summary>
		public int StackCount
		{
			get
			{
				return this.count;
			}
		}

		/// <summary>
		/// Determines whether this item can be stacked
		/// </summary>
		public bool IsStackable
		{
			get
			{
				return this.maxStack > 1;
			}
		}

		/// <summary>
		/// Determines whether this item is at its max stack count
		/// </summary>
		public bool IsStackMaxed
		{
			get
			{
				return this.count == this.maxStack;
			}
		}

		/// <summary>
		/// Determines whether a certain amount can be stacked
		/// </summary>
		/// <param name="amount"> The amount to stack </param>
		/// <returns> Returns the result </returns>
		public bool CanStack(int amount)
		{
			// stacking 0 ?
			if (amount <= 0)
				return false;
			// determining whether can stack more or not
			return this.CanStackMore && this.count + amount <= this.maxStack;
		}

		/// <summary>
		/// Stacks a certain amount
		/// <param name="amount"> The amount to be stacked. Provide 0 to stack all. </param>
		/// </summary>
		/// <returns> Returns the number of items stacked </returns>
		public int Stack(int amount)
		{
			// we cannot add negative items
			if (amount < 0)
				return 0;
			// we cannot stack more?
			if (!CanStackMore)
				return 0;
			// set the added to max amount we can stack
			// this way if they want all items to be stacked or the amount is greater than max stack
			// we can just return the added value
			var added = this.maxStack - count;
			// if the amout  is positive and less than max items we can stack, then stack the amount
			if(amount > 0 && amount <= added)
				added = amount;
			// stacking the item
			this.count += added;
			return added;
		}

		/// <summary>
		/// Determines whether a certain amount can be destacked
		/// </summary>
		/// <param name="amount"> The amount to destack </param>
		/// <returns> Returns the result </returns>
		public bool CanDestack(int amount)
		{
			// stacking 0 ?
			if (amount <= 0)
				return false;
			// determining whether can destack or not
			return this.count <= amount;
		}

		/// <summary>
		/// Destacks a certain amount
		/// <param name="amount"> The amount to be stacked. Provide 0 to stack all. </param>
		/// </summary>
		/// <returns> Returns the number of items destacked </returns>
		public int Destack(int amount)
		{
			// we cannot remove negative items
			if (amount < 0)
				return 0;
			// not (de)stackable?
			if (!IsStackable)
				return 0;
			// set the removed to all the items on the stack
			// this way if they want all items to be removed or the amount is greater than count
			// we can just return the removed value
			var removed = this.count;
			// if the amout  is positive and less than count, we can remove the amount
			if (amount > 0 && amount <= removed)
				removed = amount;
			// destacking the item
			this.count -= removed;
			return removed;
		}

		#endregion
	}
}
