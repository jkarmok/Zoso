using System;
using System.Linq;
using System.Collections.Generic;

using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Items;

namespace Karen90MmoFramework.Server.Game.Systems
{
	// TODO: Account for infinite item count
	public class Inventory
	{
		#region Constants and Fields

		private struct Slot : ISlotView<IMmoItem>
		{
			#region Implementation of ISlotView<out MmoItem>

			/// <summary>
			/// Gets or sets the item
			/// </summary>
			public MmoItem Item;

			/// <summary>
			/// Gets the item
			/// </summary>
			IMmoItem ISlotView<IMmoItem>.Item
			{
				get
				{
					return this.Item;
				}
			}

			/// <summary>
			/// Determines whether the slot is empty or not
			/// </summary>
			public bool IsEmpty
			{
				get
				{
					return this.Item == null;
				}
			}

			#endregion
		}

		/// <summary>
		/// the item creator
		/// </summary>
		private readonly Func<GameItemData, MmoItem> itemCreator;

		/// <summary>
		/// the slots
		/// </summary>
		private Slot[] slots;

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
		/// Gets or sets the value indicating whether adding items is allowed to this <see cref="Inventory"/>.
		/// </summary>
		public bool CanAdd { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether removing items is allowed from this <see cref="Inventory"/>.
		/// </summary>
		public bool CanRemove { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether moving items to this <see cref="Inventory "/> from another is allowed.
		/// </summary>
		public bool CanMoveTo { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether moving items from this <see cref="Inventory"/> to another is allowed.
		/// </summary>
		public bool CanMoveFrom { get; set; }

		public delegate void InventoryItemAdded(IMmoItem mmoItem, int count, int slot);
		/// <summary>
		/// Called when an item is added
		/// </summary>
		public event InventoryItemAdded OnItemAdded;
		
		public delegate void InventoryItemRemoved(IMmoItem mmoItem, int count, int slot);
		/// <summary>
		/// Called when an item is removed
		/// </summary>
		public event InventoryItemRemoved OnItemRemoved;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="Inventory"/> class.
		/// </summary>
		public Inventory(int size, Func<GameItemData, MmoItem> itemCreator, bool canAdd = true, bool canRemove = true, bool canMoveTo = true, bool canMoveFrom = true)
		{
			if (size < 1)
				throw new ArgumentException("InvalidParameter", "size");

			if (itemCreator == null)
				throw new NullReferenceException("itemCreator");

			this.slots = new Slot[size];
			this.itemCreator = itemCreator;
			this.FilledSlots = 0;

			this.CanAdd = canAdd;
			this.CanRemove = canRemove;
			this.CanMoveTo = canMoveTo;
			this.CanMoveFrom = canMoveFrom;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the item count of a specific item
		/// </summary>
		/// <param name="itemId"> The id of the item </param>
		/// <returns> The count of the item </returns>
		public int GetItemCount(short itemId)
		{
			var count = 0;
			for (var i = 0; i < this.slots.Length; i++)
			{
				var slot = slots[i];
				// there wont be any item at an empty
				if (slot.IsEmpty)
					continue;
				// count the item with the same item id
				var item = slot.Item;
				if (item.Id == itemId)
					count += item.StackCount;
			}
			return count;
		}

		/// <summary>
		/// Determines whether an item with a specific amount can be added or not
		/// </summary>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="count"> The count of the item to add </param>
		/// <returns> The result </returns>
		public bool CanAddItem(short itemId, int count)
		{
			// rules check
			if (!CanAdd)
				return false;
			// we are not adding any?
			if (count < 1)
				return false;
			// loading the item template
			GameItemData itemData;
			if (!MmoWorld.Instance.ItemCache.TryGetGameItemData(itemId, out itemData))
			{
				// cannot find item. write to logger
				Utils.Logger.ErrorFormat("[CanAddItem]: Item (Id={0}) not found", itemId);
				return false;
			}
			// if the number of items to add exceeds max stack and we are full we cannot add that many at once
			if (count > itemData.MaxStack && this.IsFull)
				return false;
			// setting items left to the actual count in case we didnt add any
			var left = count;
			// looping thru all slots to see where we can add the item
			for (var i = 0; i < this.Size; i++)
			{
				var slot = this.slots[i];
				if (slot.IsEmpty)
				{
					// if the slot is empty we can add the whole stack
					left -= itemData.MaxStack;
				}
				else
				{
					// if the slot isnt empty check to see if both items are the same
					var itemAtSlot = slot.Item;
					if (itemData.ItemId == itemAtSlot.Id)
						// if they are the same top the stack
						// left can be negative at this point
						// since we are not actually adding the item having a negative count doesnt matter
						// it can be checked by the following line
						left -= itemData.MaxStack - itemAtSlot.StackCount;
				}
				// if there are no or negative items left
				// we can add all the items
				if (left <= 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Add a certain number of items
		/// </summary>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="count"> The count of the item </param>
		/// <returns> The number of items added </returns>
		public int AddItem(short itemId, int count)
		{
			InventoryOperationResult result;
			return this.AddItem(itemId, count, out result);
		}

		/// <summary>
		/// Add a certain number of items
		/// </summary>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="count"> The count of the item </param>
		/// <param name="result"> The result </param>
		/// <returns> The number of items added </returns>
		public int AddItem(short itemId, int count, out InventoryOperationResult result)
		{
			result = InventoryOperationResult.Success;
			// rules check
			if (!CanAdd)
			{
				result = InventoryOperationResult.NotAllowed;
				return 0;
			}
			// if we are not adding any fail the operation
			if (count < 1)
			{
				result = InventoryOperationResult.Fail;
				return 0;
			}
			// loading the item template
			GameItemData itemData;
			if (!MmoWorld.Instance.ItemCache.TryGetGameItemData(itemId, out itemData))
			{
				// cannot find item. write to logger
				Utils.Logger.ErrorFormat("[AddItem]: Item (Id={0}) not found", itemId);
				result = InventoryOperationResult.Fail;
				return 0;
			}
			// if the item is stackable look for similar items in the inventory which we can stack
			var added = 0;
			if (itemData.MaxStack > 1)
			{
				// searching for partial stacks and stacking as much as we can
				for (var i = 0; i < this.Size; i++)
				{
					// since we are looking for slots to stack
					// we can skip an empty slot
					var slot = slots[i];
					if (slot.IsEmpty)
						continue;
					// if the item at the slot isnt the same as the item we are adding
					// or if the item (same item) is max stacked we cannot add any more at the slot
					var itemAtSlot = slot.Item;
					if (itemData.ItemId != itemAtSlot.Id || itemAtSlot.IsStackMaxed)
						continue;
					// stack the item of however many more we have left
					var stacked = itemAtSlot.Stack(count - added);
					if (stacked > 0)
					{
						// annouce the number of items we added
						if (OnItemAdded != null)
							OnItemAdded(itemAtSlot, stacked, i);
						// update how many we have left
						added += stacked;
						// if we dont have anymore left then we added all items
						if (added == count)
							return added;
					}
				}
			}
			// no more partial stacks left so add to empty slots
			// if the container is full we can return since we are not stacking but adding to an empty slot
			if (IsFull)
			{
				result = InventoryOperationResult.NoAdditionDueToSpace;
				return 0;
			}
			// searching for an empty slot to add the item to
			for (var i = 0; i < this.Size; i++)
			{
				// if the slot is empty we can skip this slot
				// since we are not stacking but adding to an empty slot
				var slotslot = this.slots[i];
				if (!slotslot.IsEmpty)
					continue;
				// create the item to be added
				var itemToAdd = this.itemCreator(itemData);
				// remember to add the item count to the added count because a new item may contain a count of 1
				// using "itemToAdd.Count" just to be safe
				added += itemToAdd.StackCount;
				// if the item can be stacked and the count is more than 1
				// then stack rest of the count
				if (itemToAdd.IsStackable && count > 1)
				{
					// stack the item of however many we have left
					var stacked = itemToAdd.Stack(count - added);
					if (stacked > 0)
					{
						// set the item at the slot
						this.slots[i] = new Slot { Item = itemToAdd };
						// increase the filled slots count
						this.FilledSlots++;
						// announce the amount of items we added
						if (OnItemAdded != null)
							OnItemAdded(itemToAdd, itemToAdd.StackCount, i);
						// increment the added count
						added += stacked;
					}
				}
				else
				{
					// if the count is 1 we can just set the item at the slot
					this.slots[i] = new Slot { Item = itemToAdd };
					this.FilledSlots++;
					// announce that we added 1 item
					if (OnItemAdded != null)
						OnItemAdded(itemToAdd, 1, i);
				}
				// if we dont have anymore left then we added all items
				if (added == count)
					return added;
			}
			// this should not happen since we already checked whether the inventory is full or not
			if(added == 0)
				result = InventoryOperationResult.NoAdditionDueToSpace;
			// if we did not add all the items set the error
			if (added < count)
				result = InventoryOperationResult.PartialAdditionDueToSpace;
			// return the added items count
			return added;
		}

		/// <summary>
		/// Add a certain number of items at a specific slot
		/// </summary>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="slot"> The slot to add to </param>
		/// <param name="count"> The count of the item </param>
		/// <returns> The number of items added </returns>
		public int AddItemAt(short itemId, int slot, int count)
		{
			InventoryOperationResult result;
			return this.AddItemAt(itemId, slot, count, out result);
		}

		/// <summary>
		/// Add a certain number of items at a specific slot
		/// </summary>
		/// <param name="itemId"> The id of the item </param>
		/// <param name="slot"> The slot to add to </param>
		/// <param name="count"> The count of the item </param>
		/// <param name="result"> The result </param>
		/// <returns> The number of items added </returns>
		public int AddItemAt(short itemId, int slot, int count, out InventoryOperationResult result)
		{
			result = InventoryOperationResult.Success;
			// rules check
			if (!CanAdd)
			{
				result = InventoryOperationResult.NotAllowed;
				return 0;
			}
			// if we are not adding any fail the operation
			if (count < 1)
			{
				result = InventoryOperationResult.Fail;
				return 0;
			}
			// range check
			if (slot < 0 || slot >= this.Size)
			{
				result = InventoryOperationResult.NoRemovalDueToItemNotFound;
				return 0;
			}
			// setting the added count to 0
			var added = 0;
			// if the slot is empty create a new stack
			// otherwise stack the item
			var slotslot = this.slots[slot];
			if (slotslot.IsEmpty)
			{
				// loading the item template
				GameItemData itemData;
				if (!MmoWorld.Instance.ItemCache.TryGetGameItemData(itemId, out itemData))
				{
					// cannot find item. write to logger
					Utils.Logger.ErrorFormat("[AddItemAt]: Item (Id={0}) not found", itemId);
					result = InventoryOperationResult.Fail;
					return 0;
				}
				// create the item to be added
				var itemToAdd = this.itemCreator(itemData);
				// remember to add the item count to the added count because a new item may contain a count of 1
				// using "itemToAdd.Count" just to be safe
				added += itemToAdd.StackCount;
				// if the item can be stacked and the count is more than 1
				// then stack rest of the count
				if (itemToAdd.IsStackable && count > 1)
				{
					// stack the item of however many we have left
					var stacked = itemToAdd.Stack(count - added);
					if (stacked > 0)
					{
						// set the item at the slot
						this.slots[slot] = new Slot { Item = itemToAdd };
						// increase the filled slots count
						this.FilledSlots++;
						// announce the amount of items we added
						if (OnItemAdded != null)
							OnItemAdded(itemToAdd, itemToAdd.StackCount, slot);
						// increment the added count
						added += stacked;
					}
				}
				else
				{
					// if the count is 1 we can just set the item at the slot
					this.slots[slot] = new Slot { Item = itemToAdd };
					this.FilledSlots++;
					// announce that we added 1 item
					if (OnItemAdded != null)
						OnItemAdded(itemToAdd, 1, slot);
				}
			}
			else
			{
				// if an item exisits at the slot and its no the same item report error
				// if the item at the slot isnt the same as the item we are adding
				// or if the item (same item) is max stacked we cannot add any more at the slot
				var itemAtSlot = slotslot.Item;
				if (itemAtSlot.Id != itemId || itemAtSlot.IsStackMaxed)
				{
					result = InventoryOperationResult.Fail;
					return 0;
				}
				// stack the item of however many more we have left
				var stacked = itemAtSlot.Stack(count - added);
				if (stacked > 0)
				{
					// annouce the number of items we added
					if (OnItemAdded != null)
						OnItemAdded(itemAtSlot, stacked, slot);
					// increment the added count
					added += stacked;
				}
			}
			// this should not happen since we already checked whether the inventory is full or not
			if (added == 0)
				result = InventoryOperationResult.NoAdditionDueToSpace;
			// if we did not add all the items set the error
			if (added < count)
				result = InventoryOperationResult.PartialAdditionDueToSpace;
			// return the added items count
			return added;
		}

		/// <summary>
		/// Remove a certain number of an item or all.
		/// </summary>
		/// <param name="itemId"> Id of the item to removed </param>
		/// <param name="count"> The number of items to be removed. Specify <value>0</value> to remove all items. </param>
		/// <returns> The number of items removed </returns>
		public int RemoveItem(short itemId, int count)
		{
			InventoryOperationResult result;
			return this.RemoveItem(itemId, count, out result);
		}

		/// <summary>
		/// Remove a certain number of an item or all.
		/// </summary>
		/// <param name="itemId"> Id of the item to removed </param>
		/// <param name="count"> The number of items to be removed. Specify <value>0</value> to remove all items. </param>
		/// <param name="result"> The result </param>
		/// <returns> The number of items removed </returns>
		public int RemoveItem(short itemId, int count, out InventoryOperationResult result)
		{
			result = InventoryOperationResult.Success;
			// rules check
			if (!CanRemove)
			{
				result = InventoryOperationResult.NotAllowed;
				return 0;
			}
			// inventory is empty
			if (IsEmpty)
			{
				result = InventoryOperationResult.NoRemovalDueToItemNotFound;
				return 0;
			}
			// we are not removing any?
			if (count != MmoItem.INFINITE && count < 1)
			{
				result = InventoryOperationResult.Fail;
				return 0;
			}
			var removed = 0;
			// are we removing all the items
			if (count == MmoItem.INFINITE)
			{
				for (var i = 0; i < this.Size; i++)
				{
					// if the slot is empty simply skip this slot
					var slot = this.slots[i];
					if (slot.IsEmpty)
						continue;
					// if its the same item then perform the removal
					var itemAtSlot = slot.Item;
					if (itemAtSlot.Id == itemId)
					{
						// add the removed item count
						removed += itemAtSlot.StackCount;
						// remove the item stack at the slot completely
						var removedAtSlot = this.DoRemoveItemAt(i, count);
						// announce the number of items we removed
						if (OnItemRemoved != null)
							this.OnItemRemoved(itemAtSlot, removed, removedAtSlot);
					}
				}
				// we did not remove any items because the item was not found
				if (removed == 0)
					result = InventoryOperationResult.NoRemovalDueToItemNotFound;
				// we only removed some so set the error
				if(removed < count)
					result = InventoryOperationResult.PartialRemoval;
				// return the removed item count
				return removed;
			}
			// we are removing only a certain number of items
			for (var i = 0; i < this.Size; i++)
			{
				// if the slot is empty simply skip this slot
				var slot = this.slots[i];
				if (slot.IsEmpty)
					continue;
				// if the items arent the same we can skip this slot
				var itemAtSlot = slot.Item;
				if (itemAtSlot.Id != itemId)
					continue;
				// if the items current stack is less than the left items
				// we can completely remove the stack from the slot
				if (itemAtSlot.StackCount <= count - removed)
				{
					// add the items removed
					removed += itemAtSlot.StackCount;
					// remove the item stack at the slot completely
					var removedAtSlot = this.DoRemoveItemAt(i, count);
					// announce the number of items we removed
					if (OnItemRemoved != null)
						OnItemRemoved(itemAtSlot, removed, removedAtSlot);
					// if we have no items left to remove we can return
					if (removed == count)
						return removed;
				}
				else
				{
					// if the left items are less than the item count
					// we can destack the item count from the stack
					itemAtSlot.Destack(count - removed);
					// announce the number of items we removed
					if (OnItemRemoved != null)
						OnItemRemoved(itemAtSlot, count - removed, i);
					// we have removed all the items so we can return
					return removed;
				}
			}
			// we did not remove any items because the item was not found
			if (removed == 0)
				result = InventoryOperationResult.NoRemovalDueToItemNotFound;
			// we only removed some so set the error
			if (removed < count)
				result = InventoryOperationResult.PartialRemoval;
			// return the removed item count
			return removed;
		}

		/// <summary>
		/// Removes an item at a specific slot
		/// </summary>
		/// <param name="slot"> The slot to remove from </param>
		/// <param name="count"> The number of items to remove </param>
		/// <returns> The number of items removed </returns>
		public int RemoveItemAt(int slot, int count)
		{
			InventoryOperationResult result;
			return this.RemoveItemAt(slot, count, out result);
		}

		/// <summary>
		/// Removes an item at a specific slot
		/// </summary>
		/// <param name="slot"> The slot to remove from </param>
		/// <param name="count"> The number of items to remove </param>
		/// <param name="result"> The result </param>
		/// <returns> The number of items removed </returns>
		public int RemoveItemAt(int slot, int count, out InventoryOperationResult result)
		{
			result = InventoryOperationResult.Success;
			// rules check
			if (!CanRemove)
			{
				result = InventoryOperationResult.NotAllowed;
				return 0;
			}
			// range check
			if (slot < 0 || slot >= this.Size)
			{
				result = InventoryOperationResult.NoRemovalDueToItemNotFound;
				return 0;
			}
			// if the slot is empty there will not be any item there
			var slotslot = this.slots[slot];
			if (slotslot.IsEmpty)
			{
				result = InventoryOperationResult.NoRemovalDueToItemNotFound;
				return 0;
			}
			// we are not removing any?
			if (count != MmoItem.INFINITE && count < 1)
			{
				result = InventoryOperationResult.Fail;
				return 0;
			}
			// store the about-to-be-removed item before removing it so we can pass it to OnItemRemoved
			var itemAtSlot = slotslot.Item;
			// remove the item from the slot
			var removed = this.DoRemoveItemAt(slot, count);
			// if none removed return the result
			if (removed == 0)
			{
				result = InventoryOperationResult.NoRemovalDueToItemNotFound;
				return 0;
			}
			// announce the number of items we removed
			if (OnItemRemoved != null)
				OnItemRemoved(itemAtSlot, removed, slot);
			// we only removed some of the items so set the error
			if(removed < count)
				result = InventoryOperationResult.PartialRemoval;
			// return the removed count
			return removed;
		}

		/// <summary>
		/// Moves an item from a slot to another
		/// </summary>
		/// <param name="slotTo"> The slot to move to </param>
		/// <param name="slotFrom"> The slot to move from </param>
		/// <returns> The result of the move </returns>
		public bool MoveItem(int slotTo, int slotFrom)
		{
			return this.MoveItem(this, slotTo, slotFrom);
		}

		/// <summary>
		/// Moves an item from another <see cref="Inventory"/> to this.
		/// </summary>
		/// <param name="inventoryFrom"> The <see cref="Inventory"/> to move from </param>
		/// <param name="slotTo"> The slot to move to </param>
		/// <param name="slotFrom"> The slot to move from </param>
		/// <returns> The result of the move </returns>
		public bool MoveItem(Inventory inventoryFrom, int slotTo, int slotFrom)
		{
			// rules check
			if (!inventoryFrom.CanMoveFrom || !CanMoveTo)
				return false;
			// range check
			if (slotTo >= Size || slotTo < 0 || slotFrom >= inventoryFrom.Size || slotFrom < 0 || (inventoryFrom == this && slotTo == slotFrom))
				return false;
			// if the slot at the from slot is empty we dont have anything to move
			var slotslotFrom = inventoryFrom.slots[slotFrom];
			if (slotslotFrom.IsEmpty)
				return false;
			// if we are moving to an empty slot a two-way-swap isnt necessary
			var itemFrom = slotslotFrom.Item;
			var slotslotTo = this.slots[slotTo];
			if (slotslotTo.IsEmpty)
			{
				// remove the item from the other inventory slot
				inventoryFrom.DoRemoveItemAt(slotFrom, itemFrom.StackCount);
				// swap item
				this.slots[slotTo] = new Slot {Item = itemFrom};
				// since we are moving to an empty slot we need to increment the filled slots
				this.FilledSlots++;
				return true;
			}
			// if the items cannot be stacked on top of each other we need to do a two-way-swap
			var itemTo = slotslotTo.Item;
			if (itemTo != itemFrom || itemTo.IsStackable == false || itemTo.IsStackMaxed || itemFrom.IsStackMaxed)
			{
				// swaping both items
				this.slots[slotTo] = new Slot {Item = itemFrom};
				inventoryFrom.slots[slotFrom] = new Slot {Item = itemTo};
				return true;
			}
			// if the two items are the same can we stack?
			if (itemTo == itemFrom)
			{
				// stack our item and see how many cannot be stacked
				var stacked = itemTo.Stack(itemFrom.StackCount);
				if (stacked > 0)
				{
					// if we stacked all the items from the from container
					if (stacked == itemFrom.StackCount)
					{
						// we can completely remove the item from the from container
						inventoryFrom.DoRemoveItemAt(slotFrom, itemFrom.StackCount);
						return true;
					}
					// if we cannot stack completely, destack however many we stacked from the from item
					itemFrom.Destack(stacked);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Finds out whether the <see cref="Inventory"/> has a certain item or not.
		/// </summary>
		/// <param name="itemId"> The id of the item to look for </param>
		/// <returns> The result </returns>
		public bool HasItem(short itemId)
		{
			// searching all the slots for an item with the item id
			for (var i = 0; i < this.Size; i++)
			{
				// if the slot is empty simply skip this slot
				var slot = this.slots[i];
				if (slot.IsEmpty)
					continue;
				// if the item ids match, then we have located the item and the item exits
				if (slot.Item.Id == itemId)
					return true;
			}
			// we have finished searching all the slots
			// the item does not exist
			return false;
		}

		/// <summary>
		/// Tries to retrieve a(n) <see cref="IMmoItem"/> at the <see cref="slot"/>.
		/// </summary>
		/// <param name="slot"> The slot where the item is located </param>
		/// <param name="mmoItem"> The item at the slot </param>
		/// <returns> The result </returns>
		public bool TryGetItem(int slot, out IMmoItem mmoItem)
		{
			mmoItem = null;
			// range check
			if (slot < 0 || slot >= this.Size)
				return false;

			return (mmoItem = this.slots[slot].Item) != null;
		}

		/// <summary>
		/// Tells whether the slot is empty or not
		/// </summary>
		/// <param name="slot"> The slot to look for </param>
		/// <returns> The result </returns>
		public bool IsSlotEmpty(int slot)
		{
			// since we are concerned about a particular slot we need to throw an exception
			if (slot < 0 || slot >= this.Size)
				throw new IndexOutOfRangeException("IndexOutOfRange");

			return slots[slot].IsEmpty;
		}

		/// <summary>
		/// Expands the <see cref="Inventory"/> by the <see cref="amount"/>.
		/// </summary>
		/// <param name="amount"> The expansion amount </param>
		/// <remarks> The resize will only happen if the filled slots are within the new size </remarks>
		public bool Expand(int amount)
		{
			return this.Resize(Size + amount);
		}

		/// <summary>
		/// Shrinks the <see cref="Inventory"/> by the <see cref="amount"/>.
		/// </summary>
		/// <param name="amount"> The shrink amount </param>
		/// <remarks> The resize will only happen if the filled slots are within the new size </remarks>
		public bool Shrink(int amount)
		{
			return this.Resize(Size - amount);
		}
		
		/// <summary>
		/// Resizes the <see cref="Inventory"/> to the <see cref="newSize"/>.
		/// </summary>
		/// <param name="newSize"> The new size </param>
		/// <remarks> The resize will only happen if the filled slots are within the <see cref="newSize"/> </remarks>
		public bool Resize(int newSize)
		{
			// size validity
			if (newSize < 0)
				throw new ArgumentException("InvalidSize", "newSize");
			// size validity
			if (newSize == 0)
				return false;
			// is the current size same as the new size ?
			if (newSize == Size)
				return true;
			// are we shrinking ?
			if(newSize < Size)
			{
				// searching for any filled slots beyond the new size
				for (var i = newSize; i < Size; i++)
				{
					// if there is an item exists beyond the new size simply fail the resize
					if (!slots[i].IsEmpty)
						return false;
				}
			}
			// resize the array
			Array.Resize(ref slots, newSize);
			return true;
		}

		/// <summary>
		/// Get all the slots
		/// </summary>
		public IEnumerable<ISlotView<IMmoItem>> GetSlots()
		{
			return this.slots.Cast<ISlotView<IMmoItem>>();
		}

		/// <summary>
		/// Gets the slot at a specific slot location
		/// </summary>
		/// <param name="slot"> The slot location </param>
		/// <returns> The <see cref="ISlotView{IMmoItem}"/> </returns>
		public ISlotView<IMmoItem> this[int slot]
		{
			get
			{
				// range check
				if (slot < 0 || slot >= this.Size)
					throw new IndexOutOfRangeException("SlotOutOfRange");
				
				return this.slots[slot];
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Removes an item at a specific slot
		/// </summary>
		/// <returns> Returns the number of items removed </returns>
		private int DoRemoveItemAt(int slot, int count)
		{
			// range check
			if (slot > this.Size || slot < 0)
				return count;
			// if the slot is empty then we have nothing to remove
			var slotslot = this.slots[slot];
			if (slotslot.IsEmpty)
				return count;
			// if the current item count exceeds the number of items we are about to remove
			// them remove the whole slot
			var itemToRemove = slotslot.Item;
			if (count >= itemToRemove.StackCount || itemToRemove.IsStackable == false)
			{
				// empty the slot
				this.slots[slot] = new Slot {Item = null};
				// we lost a slot
				this.FilledSlots--;
				// return however many we removed from the slot
				return itemToRemove.StackCount;
			}
			// if the current item count is less than the number of items we are about to remove
			// then destack the item
			return itemToRemove.Destack(count);
		}

		#endregion
	}
}
