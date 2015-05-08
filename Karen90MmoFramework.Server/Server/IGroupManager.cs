using Karen90MmoFramework.Game;

namespace Karen90MmoFramework.Server
{
	public interface IGroupManager
	{
		void FormGroup(MmoGuid groupGuid, GroupMemberStructure leader);
		void DisbandGroup(MmoGuid groupGuid);

		void AddMember(MmoGuid groupGuid, GroupMemberStructure member);
		void RemoveMember(MmoGuid groupGuid, MmoGuid memberGuid);

		void UpdateMemberStatus(MmoGuid groupGuid, MmoGuid memberGuid, OnlineStatus status);
	}
}
