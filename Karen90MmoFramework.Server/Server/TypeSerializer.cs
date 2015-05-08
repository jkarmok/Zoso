using System;
using Photon.SocketServer;
using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server
{
	public static class TypeSerializer
	{
		/// <summary>
		/// Registers a <see cref="CustomTypeCode"/> on the protocol
		/// </summary>
		/// <param name="typeCode"></param>
		public static void RegisterType(CustomTypeCode typeCode)
		{
			switch (typeCode)
			{
				case CustomTypeCode.Guid:
					Protocol.TryRegisterCustomType(typeof (Guid), (byte) CustomTypeCode.Guid,
					                               GlobalSerializerMethods.SerializeGuid,
					                               GlobalSerializerMethods.DeserializeGuid);
					break;

				case CustomTypeCode.CharacterStructure:
					Protocol.TryRegisterCustomType(typeof(CharacterStructure), (byte)CustomTypeCode.CharacterStructure,
					                               GlobalSerializerMethods.SerializeCharacterStructure,
					                               GlobalSerializerMethods.DeserializeCharacterStructure);
					break;

				case CustomTypeCode.ItemStructure:
					Protocol.TryRegisterCustomType(typeof(ItemStructure), (byte)CustomTypeCode.ItemStructure,
												   GlobalSerializerMethods.SerializeItemStructure,
												   GlobalSerializerMethods.DeserializeItemStructure);
					break;

				case CustomTypeCode.ContainerItemStructure:
					Protocol.TryRegisterCustomType(typeof(ContainerItemStructure), (byte)CustomTypeCode.ContainerItemStructure,
												   GlobalSerializerMethods.SerializeContainerItemStructure,
												   GlobalSerializerMethods.DeserializeContainerItemStructure);
					break;

				case CustomTypeCode.ActionItemStructure:
					Protocol.TryRegisterCustomType(typeof(ActionItemStructure), (byte)CustomTypeCode.ActionItemStructure,
												   GlobalSerializerMethods.SerializeActionItemStructure,
												   GlobalSerializerMethods.DeserializeActionItemStructure);
					break;

				case CustomTypeCode.SlotItemStructure:
					Protocol.TryRegisterCustomType(typeof(SlotItemStructure), (byte)CustomTypeCode.SlotItemStructure,
												   GlobalSerializerMethods.SerializeSlotItemStructure,
												   GlobalSerializerMethods.DeserializeSlotItemStructure);
					break;

				case CustomTypeCode.MenuItemStructure:
					Protocol.TryRegisterCustomType(typeof(MenuItemStructure), (byte)CustomTypeCode.MenuItemStructure,
												   GlobalSerializerMethods.SerializeMenuItemStructure,
												   GlobalSerializerMethods.DeserializeMenuItemStructure);
					break;

				case CustomTypeCode.ActiveQuestStructure:
					Protocol.TryRegisterCustomType(typeof(ActiveQuestStructure), (byte)CustomTypeCode.ActiveQuestStructure,
												   GlobalSerializerMethods.SerializeActiveQuestStructure,
												   GlobalSerializerMethods.DeserializeActiveQuestStructure);
					break;

				case CustomTypeCode.QuestProgressStructure:
					Protocol.TryRegisterCustomType(typeof(QuestProgressStructure), (byte)CustomTypeCode.QuestProgressStructure,
												   GlobalSerializerMethods.SerializeQuestProgressStructure,
												   GlobalSerializerMethods.DeserializeQuestProgressStructure);
					break;

				case CustomTypeCode.GroupStructure:
					Protocol.TryRegisterCustomType(typeof(GroupStructure), (byte)CustomTypeCode.GroupStructure,
												   GlobalSerializerMethods.SerializeGroupStructure,
												   GlobalSerializerMethods.DeserializeGroupStructure);
					break;

				case CustomTypeCode.GroupMemberStructure:
					Protocol.TryRegisterCustomType(typeof(GroupMemberStructure), (byte)CustomTypeCode.GroupMemberStructure,
												   GlobalSerializerMethods.SerializeGroupMemberStructure,
												   GlobalSerializerMethods.DeserializeGroupMemberStructure);
					break;

				case CustomTypeCode.ProfileStructure:
					Protocol.TryRegisterCustomType(typeof(ProfileStructure), (byte)CustomTypeCode.ProfileStructure,
												   GlobalSerializerMethods.SerializeProfileStructure,
												   GlobalSerializerMethods.DeserializeProfileStructure);
					break;
			}
		}
	}
}
