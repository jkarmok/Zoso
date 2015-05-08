using System.Collections.Generic;
using System.Linq;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Items;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Server
{
	public static class MmoSerializer
	{
		public static ActionBarData SerializeActionbar(Actionbar actionbar)
		{
			var items = new int[actionbar.FilledSlots];
			var slots = actionbar.GetSlots().ToArray();

			for (byte i = 0, slotCounter = 0; i < slots.Length; i++)
			{
				var slot = slots[i];
				if (slot.IsEmpty)
					continue;

				var item = slot.Item;
				items[slotCounter++] = (int) new ActionItemStructure(item.ItemId, (byte) item.Type, i);
			}

			return new ActionBarData {Items = items};
		}

		public static Actionbar DeserializeActionbar(ActionBarData actionBarData)
		{
			var actionbar = new Actionbar(GlobalGameSettings.PLAYER_ACTION_BAR_SIZE);
			foreach (ActionItemStructure actionItem in actionBarData.Items)
				actionbar.AddItemAt((ActionItemType) actionItem.Type, actionItem.ItemId, actionItem.Index);
			return actionbar;
		}
		
		public static InventoryData SerializePlayerInventory(Inventory inventory)
		{
			var items = new int[inventory.FilledSlots];
			var slots = inventory.GetSlots().ToArray();

			for (byte i = 0, slotCounter = 0; i < slots.Length; i++)
			{
				var slot = slots[i];
				if (slot.IsEmpty)
					continue;

				var item = slot.Item;
				items[slotCounter++] = (int) new ContainerItemStructure(item.Id, i, (byte) item.StackCount);
			}

			return new InventoryData {Items = items, Size = inventory.Size};
		}
		
		public static Inventory DeserializePlayerInventory(InventoryData inventoryData)
		{
			var inventory = new Inventory(inventoryData.Size, itemData => new MmoItem(itemData));
			foreach (ContainerItemStructure item in inventoryData.Items)
				inventory.AddItemAt(item.ItemId, item.Index, item.Count);
			return inventory;
		}

		public static Dictionary<short, QuestProgression> DeserializePlayerQuests(Dictionary<short, QuestProgression> quests)
		{
			return new Dictionary<short, QuestProgression>(quests);
		}

		public static Dictionary<short, QuestProgression> SerializePlayerQuests(Dictionary<short, QuestProgression> quests)
		{
			return new Dictionary<short, QuestProgression>(quests);
		}
	}
}
