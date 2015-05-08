using System;
using System.Linq;
using System.Collections.Generic;

using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public class Actionbar
	{
		#region Constants and Fields

		public struct ActionItem
		{
			public static readonly ActionItem Null = new ActionItem {ItemId = -1, Type = 0};

			public ActionItemType Type;
			public short ItemId;
		}

		struct Slot : ISlotView<ActionItem>
		{
			#region Implementation of ISlotView<out ActionItem>

			/// <summary>
			/// Gets the item
			/// </summary>
			public ActionItem Item { get; set; }

			/// <summary>
			/// Determines whether the slot is empty or not
			/// </summary>
			public bool IsEmpty { get; set; }

			#endregion
		}

		/// <summary>
		/// the action item slots
		/// </summary>
		private readonly Slot[] slots;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the container size
		/// </summary>
		public int Size
		{
			get
			{
				return slots.Length;
			}
		}

		/// <summary>
		/// Gets the filled slots count
		/// </summary>
		public int FilledSlots { get; private set; }

		/// <summary>
		/// Tells whether the container is empty or not
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.FilledSlots == 0;
			}
		}

		/// <summary>
		/// Tells whether the inventory is full or not(only counting slots)
		/// </summary>
		public bool IsFull
		{
			get
			{
				return this.FilledSlots == this.Size;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating whether adding items is allowed to this <see cref="Actionbar"/>.
		/// </summary>
		public bool CanAdd { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether removing items is allowed from this <see cref="Actionbar"/>.
		/// </summary>
		public bool CanRemove { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether moving items to this <see cref="Actionbar "/> from another is allowed.
		/// </summary>
		public bool CanMoveTo { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether moving items from this <see cref="Actionbar"/> to another is allowed.
		/// </summary>
		public bool CanMoveFrom { get; set; }

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Creates a new instance of the <see cref="Actionbar"/> class.
		/// </summary>
		public Actionbar(int size, bool canAdd = true, bool canRemove = true, bool canMoveTo = true, bool canMoveFrom = true)
		{
			if (size < 1)
				throw new ArgumentException("InvalidParameter", "size");

			this.slots = new Slot[size];
			for (var i = 0; i < slots.Length; i++)
				this.slots[i].IsEmpty = true;
			
			this.FilledSlots = 0;

			this.CanAdd = canAdd;
			this.CanRemove = canRemove;
			this.CanMoveTo = canMoveTo;
			this.CanMoveFrom = canMoveFrom;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a(n) <see cref="ActionItem"/> to the next empty slot.
		/// </summary>
		/// <param name="itemType"> The type of the item </param>
		/// <param name="itemId"> The id of the item </param>
		/// <returns> The result </returns>
		public bool AddItem(ActionItemType itemType, short itemId)
		{
			ActionbarOperationResult result;
			return this.AddItem(itemType, itemId, out result);
		}

		/// <summary>
		/// Adds a(n) <see cref="ActionItem"/> to the next empty slot.
		/// </summary>
		/// <param name="itemType"> The type of the item </param>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="result"> Description of the result </param>
		/// <returns> The result </returns>
		public bool AddItem(ActionItemType itemType, short itemId, out ActionbarOperationResult result)
		{
			result = ActionbarOperationResult.Success;
			// rules check
			if(!CanAdd)
			{
				result = ActionbarOperationResult.NotAllowed;
				return false;
			}
			// if the actionbar is full just return
			if(IsFull)
			{
				result = ActionbarOperationResult.ActionbarFull;
				return false;
			}
			// try to find an empty slot
			for (var i = 0; i < this.Size; i++)
			{
				// if the slot isnt empty skip
				if (!slots[i].IsEmpty)
					continue;
				// add the item at an empty slot
				this.slots[i] = new Slot { Item = new ActionItem { Type = itemType, ItemId = itemId }, IsEmpty = false };
				this.FilledSlots++;
				return true;
			}
			// this should not happen
			result = ActionbarOperationResult.Fail;
			return false;
		}

		/// <summary>
		/// Adds a(n) <see cref="ActionItem"/> at a specific slot.
		/// </summary>
		/// <param name="itemType"> The type of the item </param>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="slot"> The slot to add to </param>
		/// <returns> The result </returns>
		public bool AddItemAt(ActionItemType itemType, short itemId, int slot)
		{
			ActionbarOperationResult result;
			return this.AddItemAt(itemType, itemId, slot, out result);
		}

		/// <summary>
		/// Adds a(n) <see cref="ActionItem"/> at a specific slot.
		/// </summary>
		/// <param name="itemType"> The type of the item </param>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="slot"> The slot to add to </param>
		/// <param name="result"> Description of the result </param>
		/// <returns> The result </returns>
		public bool AddItemAt(ActionItemType itemType, short itemId, int slot, out ActionbarOperationResult result)
		{
			result = ActionbarOperationResult.Success;
			// rules check
			if (!CanAdd)
			{
				result = ActionbarOperationResult.NotAllowed;
				return false;
			}
			// range check
			if (slot < 0 || slot >= this.Size)
			{
				result = ActionbarOperationResult.Fail;
				return false;
			}
			// add the item at that slot
			this.slots[slot] = new Slot { Item = new ActionItem { Type = itemType, ItemId = itemId }, IsEmpty = false };
			this.FilledSlots++;
			return true;
		}

		/// <summary>
		/// Removes a specific item
		/// </summary>
		/// <param name="itemType"> The type of the item </param>
		/// <param name="itemId"> The id of the item </param>
		/// <returns> The result </returns>
		public bool RemoveItem(ActionItemType itemType, short itemId)
		{
			ActionbarOperationResult result;
			return this.RemoveItem(itemType, itemId, out result);
		}

		/// <summary>
		/// Removes a specific item
		/// </summary>
		/// <param name="itemType"> The type of the item </param>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="result"> Description of the result </param>
		/// <returns> The result </returns>
		public bool RemoveItem(ActionItemType itemType, short itemId, out ActionbarOperationResult result)
		{
			result = ActionbarOperationResult.Success;
			// rules check
			if (!CanRemove)
			{
				result = ActionbarOperationResult.NotAllowed;
				return false;
			}
			// actionbar is empty
			if (IsEmpty)
			{
				result = ActionbarOperationResult.ItemNotFound;
				return false;
			}
			// searching for the item
			for (var i = 0; i < this.Size; i++)
			{
				// if the slot is empty skip it
				var slot = slots[i];
				if (slot.IsEmpty)
					continue;
				// matching the item id and the type
				var item = slot.Item;
				if (item.Type != itemType || item.ItemId != itemId)
					continue;
				// remove the item
				this.slots[i].IsEmpty = true;
				this.FilledSlots--;
				return true;
			}
			// this should not happen
			result = ActionbarOperationResult.Fail;
			return false;
		}

		/// <summary>
		/// Removes an item at a specific slot
		/// </summary>
		public bool RemoveItemAt(int slot)
		{
			ActionbarOperationResult result;
			return this.RemoveItemAt(slot, out result);
		}

		/// <summary>
		/// Removes an item at a specific slot
		/// </summary>
		/// <param name="slot"> The slot to remove from </param>
		/// <param name="result"> Description of the result </param>
		/// <returns> The result </returns>
		public bool RemoveItemAt(int slot, out ActionbarOperationResult result)
		{
			result = ActionbarOperationResult.Success;
			// rules check
			if(!CanRemove)
			{
				result = ActionbarOperationResult.NotAllowed;
				return false;
			}
			// actionbar is empty
			if (IsEmpty)
			{
				result = ActionbarOperationResult.ItemNotFound;
				return false;
			}
			// range check
			if (slot < 0 || slot >= this.Size)
			{
				result = ActionbarOperationResult.ItemNotFound;
				return false;
			}
			// slot is empty
			if (slots[slot].IsEmpty)
			{
				result = ActionbarOperationResult.ItemNotFound;
				return false;
			}
			// remove the item at the slot
			this.slots[slot].IsEmpty = true;
			this.FilledSlots--;
			return true;
		}

		/// <summary>
		/// Moves an item from an index to another within this <see cref="Actionbar"/>
		/// </summary>
		public bool MoveItem(int indexTo, int indexFrom)
		{
			return this.MoveItem(this, indexTo, indexFrom);
		}

		/// <summary>
		/// Moves an item from an <see cref="Actionbar"/> to this <see cref="Actionbar"/>
		/// </summary>
		public bool MoveItem(Actionbar actionbarFrom, int slotTo, int slotFrom)
		{
			// rules check
			if (!actionbarFrom.CanMoveFrom || !CanMoveTo)
				return false;
			// range check
			if (slotTo < 0 || slotTo >= this.Size || slotFrom < 0 || slotFrom >= actionbarFrom.Size)
				return false;
			// making sure that we are not moving an empty item
			if (actionbarFrom.slots[slotFrom].IsEmpty)
				return false;

			var itemTo = this.slots[slotTo];
			this.slots[slotTo] = actionbarFrom.slots[slotFrom];
			actionbarFrom.slots[slotFrom] = itemTo;
			return true;
		}

		/// <summary>
		/// Gets the <see cref="ActionItem"/> at a slot
		/// </summary>
		public bool TryGetItem(int slot, out ActionItem item)
		{
			item = ActionItem.Null;
			// range check
			if (slot < 0 || slot >= this.Size)
				return false;

			var slotslot = this.slots[slot];
			if (slotslot.IsEmpty)
				return false;

			item = slotslot.Item;
			return true;
		}

		/// <summary>
		/// Tells whether the slot is empty or not
		/// </summary>
		public bool SlotEmpty(int slot)
		{
			if (slot < 0 || slot >= this.Size)
				throw new IndexOutOfRangeException("IndexOutOfRange");

			return slots[slot].IsEmpty;
		}

		/// <summary>
		/// Get all the slots
		/// </summary>
		public IEnumerable<ISlotView<ActionItem>> GetSlots()
		{
			return this.slots.Cast<ISlotView<ActionItem>>();
		}

		#endregion
	}
}
