using System;
using System.Text;
using Karen90MmoFramework.Game;

namespace Karen90MmoFramework
{
	public static class GlobalSerializerMethods
	{
		/// <summary>
		/// <see cref="Guid"/> serialization
		/// </summary>
		public static byte[] SerializeGuid(object obj)
		{
			return ((Guid) obj).ToByteArray();
		}

		/// <summary>
		/// <see cref="Guid"/> deserialization
		/// </summary>
		public static object DeserializeGuid(byte[] bytes)
		{
			return new Guid(bytes);
		}

		/// <summary>
		/// <see cref="CharacterStructure"/> serialization
		/// </summary>
		public static byte[] SerializeCharacterStructure(object obj)
		{
			var charStruct = (CharacterStructure) obj;
			var strBytes = Encoding.UTF8.GetBytes(charStruct.Name);
			var size = 3 + strBytes.Length;
			var bytes = new byte[size];

			bytes[0] = charStruct.Race;
			bytes[1] = charStruct.Origin;
			bytes[2] = charStruct.Level;

			Buffer.BlockCopy(strBytes, 0, bytes, 3, strBytes.Length);

			return bytes;
		}

		/// <summary>
		/// <see cref="CharacterStructure"/> deserialization
		/// </summary>
		public static object DeserializeCharacterStructure(byte[] bytes)
		{
			var race = bytes[0];
			var origin = bytes[1];
			var level = bytes[2];
			var name = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

			return new CharacterStructure(race, origin, level, name);
		}

		/// <summary>
		/// <see cref="ItemStructure"/> serialization
		/// </summary>
		public static byte[] SerializeItemStructure(object obj)
		{
			var itemStruct = (ItemStructure) obj;
			var bytes = new byte[3]; // 2 + 1

			var bbs = BitConverter.GetBytes(itemStruct.ItemId);
			Buffer.BlockCopy(bbs, 0, bytes, 0, bbs.Length); // 2
			bytes[2] = itemStruct.Count;

			return bytes;
		}

		/// <summary>
		/// <see cref="ItemStructure"/> deserialization
		/// </summary>
		public static object DeserializeItemStructure(byte[] bytes)
		{
			var itemId = BitConverter.ToInt16(bytes, 0); // 2
			var count = bytes[2];

			return new ItemStructure(itemId, count);
		}

		/// <summary>
		/// <see cref="ContainerItemStructure"/> serialization
		/// </summary>
		public static byte[] SerializeContainerItemStructure(object obj)
		{
			var inventoryItemStruct = (ContainerItemStructure) obj;
			var intValue = ((inventoryItemStruct.Index << 24) | (inventoryItemStruct.Count << 16) | (ushort) inventoryItemStruct.ItemId);
			return BitConverter.GetBytes(intValue);
		}

		/// <summary>
		/// <see cref="ContainerItemStructure"/> deserialization
		/// </summary>
		public static object DeserializeContainerItemStructure(byte[] bytes)
		{
			var intValue = BitConverter.ToInt32(bytes, 0);
			return new ContainerItemStructure((short) (intValue & 0xFFFF), (byte) ((intValue >> 24) & 0xFF), (byte) ((intValue >> 16) & 0xFF));
		}

		/// <summary>
		/// <see cref="ActionItemStructure"/> serialization
		/// </summary>
		public static byte[] SerializeActionItemStructure(object obj)
		{
			var actionItemStruct = (ActionItemStructure) obj;
			var intValue = ((actionItemStruct.Type << 24) | (actionItemStruct.Index << 16) | (ushort) actionItemStruct.ItemId);
			return BitConverter.GetBytes(intValue);
		}

		/// <summary>
		/// <see cref="ActionItemStructure"/> deserialization
		/// </summary>
		public static object DeserializeActionItemStructure(byte[] bytes)
		{
			var intValue = BitConverter.ToInt32(bytes, 0);
			return new ActionItemStructure((short) (intValue & 0xFFFF), (byte) ((intValue >> 24) & 0xFF), (byte) ((intValue >> 16) & 0xFF));
		}

		/// <summary>
		/// <see cref="SlotItemStructure"/> serialization
		/// </summary>
		public static byte[] SerializeSlotItemStructure(object obj)
		{
			var slotItemStruct = (SlotItemStructure) obj;
			var shortValue = ((slotItemStruct.Slot << 8) | slotItemStruct.Count);
			return BitConverter.GetBytes(shortValue);
		}

		/// <summary>
		/// <see cref="SlotItemStructure"/> deserialization
		/// </summary>
		public static object DeserializeSlotItemStructure(byte[] bytes)
		{
			var shortValue = BitConverter.ToInt16(bytes, 0);
			return new SlotItemStructure((byte) ((shortValue >> 8) & 0xFF), (byte) (shortValue & 0xFF));
		}

		/// <summary>
		/// <see cref="MenuItemStructure"/> serialization
		/// </summary>
		public static byte[] SerializeMenuItemStructure(object obj)
		{
			var menuItemStruct = (MenuItemStructure) obj;
			var intValue = ((menuItemStruct.ItemId << 16) | (menuItemStruct.ItemType << 8) | menuItemStruct.IconType);
			return BitConverter.GetBytes(intValue);
		}

		/// <summary>
		/// <see cref="MenuItemStructure"/> deserialization
		/// </summary>
		public static object DeserializeMenuItemStructure(byte[] bytes)
		{
			var intValue = BitConverter.ToInt32(bytes, 0);
			return new MenuItemStructure((short) ((intValue >> 16) & 0xFFFF), (byte) ((intValue >> 8) & 0xFF), (byte) (intValue & 0xFF));
		}

		/// <summary>
		/// <see cref="ActiveQuestStructure"/> serialization
		/// </summary>
		public static byte[] SerializeActiveQuestStructure(object obj)
		{
			var activeQuestStructure = (ActiveQuestStructure)obj;
			var bytes = new byte[3 + activeQuestStructure.Counts.Length]; // 1 + 2 + len(count)

			bytes[0] = activeQuestStructure.Status; // 1
			var bbs = BitConverter.GetBytes(activeQuestStructure.QuestId);
			Buffer.BlockCopy(bbs, 0, bytes, 1, bbs.Length);
			Buffer.BlockCopy(activeQuestStructure.Counts, 0, bytes, 3, activeQuestStructure.Counts.Length);

			return bytes;
		}

		/// <summary>
		/// <see cref="ActiveQuestStructure"/> deserialization
		/// </summary>
		public static object DeserializeActiveQuestStructure(byte[] bytes)
		{
			var status = bytes[0]; // 1
			var questId = BitConverter.ToInt16(bytes, 1); // 3
			var counts = new byte[bytes.Length - 3];
			Buffer.BlockCopy(bytes, 3, counts, 0, counts.Length);

			return new ActiveQuestStructure(questId, status, counts);
		}

		/// <summary>
		/// <see cref="QuestProgressStructure"/> serialization
		/// </summary>
		public static byte[] SerializeQuestProgressStructure(object obj)
		{
			var questProgressStruct = (QuestProgressStructure) obj;
			var intValue = ((questProgressStruct.Index << 24) | (questProgressStruct.Count << 16) | (ushort) questProgressStruct.QuestId);
			return BitConverter.GetBytes(intValue);
		}

		/// <summary>
		/// <see cref="QuestProgressStructure"/> deserialization
		/// </summary>
		public static object DeserializeQuestProgressStructure(byte[] bytes)
		{
			var intValue = BitConverter.ToInt32(bytes, 0);
			return new QuestProgressStructure((short)(intValue & 0xFFFF), (byte)((intValue >> 24) & 0xFF), (byte)((intValue >> 16) & 0xFF));
		}

		/// <summary>
		/// <see cref="GroupStructure"/> serialization
		/// </summary>
		public static byte[] SerializeGroupStructure(object obj)
		{
			var groupStructure = (GroupStructure) obj;
			var bytes = new byte[16]; // 8 + 8

			var bbs = BitConverter.GetBytes(groupStructure.GroupId);
			Buffer.BlockCopy(bbs, 0, bytes, 0, bbs.Length); // 8
			var bbs2 = BitConverter.GetBytes(groupStructure.LeaderId);
			Buffer.BlockCopy(bbs2, 0, bytes, 8, bbs.Length); // 16

			return bytes;
		}

		/// <summary>
		/// <see cref="GroupStructure"/> deserialization
		/// </summary>
		public static object DeserializeGroupStructure(byte[] bytes)
		{
			var groupId = BitConverter.ToInt64(bytes, 0); // 8
			var leaderId = BitConverter.ToInt64(bytes, 8); // 16

			return new GroupStructure(groupId, leaderId);
		}

		/// <summary>
		/// <see cref="GroupMemberStructure"/> serialization
		/// </summary>
		public static byte[] SerializeGroupMemberStructure(object obj)
		{
			var groupMemberStructure = (GroupMemberStructure) obj;
			var strBytes = Encoding.UTF8.GetBytes(groupMemberStructure.Name);
			var bytes = new byte[8 + strBytes.Length]; // 8(guid) + strlen(name)

			var bbs = BitConverter.GetBytes(groupMemberStructure.Guid);
			Buffer.BlockCopy(bbs, 0, bytes, 0, bbs.Length); // 8
			Buffer.BlockCopy(strBytes, 0, bytes, 8, strBytes.Length);

			return bytes;
		}

		/// <summary>
		/// <see cref="GroupMemberStructure"/> deserialization
		/// </summary>
		public static object DeserializeGroupMemberStructure(byte[] bytes)
		{
			var guid = BitConverter.ToInt64(bytes, 0); // 8
			var name = Encoding.UTF8.GetString(bytes, 8, bytes.Length - 8); // 9 + count - 9

			return new GroupMemberStructure(guid, name);
		}

		/// <summary>
		/// <see cref="ProfileStructure"/> serialization
		/// </summary>
		public static byte[] SerializeProfileStructure(object obj)
		{
			var profileStructure = (ProfileStructure) obj;
			var strBytes = Encoding.UTF8.GetBytes(profileStructure.Name);
			var bytes = new byte[1 + 1 + 2 + strBytes.Length]; // 1 + 1 + 2 + len(name)

			bytes[0] = profileStructure.Level; // 1
			bytes[1] = profileStructure.OnlineStatus; // 2
			var bbs = BitConverter.GetBytes(profileStructure.WorldId);
			Buffer.BlockCopy(bbs, 0, bytes, 2, bbs.Length); // 4
			Buffer.BlockCopy(strBytes, 0, bytes, 4, strBytes.Length);

			return bytes;
		}

		/// <summary>
		/// <see cref="ProfileStructure"/> deserialization
		/// </summary>
		public static object DeserializeProfileStructure(byte[] bytes)
		{
			var level = bytes[0]; // 1
			var status = bytes[1]; // 2
			var worldId = BitConverter.ToInt16(bytes, 2); // 4
			var name = Encoding.UTF8.GetString(bytes, 4, bytes.Length - 4); // 4 + count - 4

			return new ProfileStructure(name, level, worldId, status);
		}
	}
}
