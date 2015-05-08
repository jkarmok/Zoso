namespace Karen90MmoFramework.Game
{
	public struct GroupMemberStructure
	{
		public readonly MmoGuid Guid;
		public readonly string Name;

		public GroupMemberStructure(MmoGuid guid, string name)
		{
			this.Guid = guid;
			this.Name = name;
		}

		public bool Equals(GroupMemberStructure groupMemberInfo)
		{
			return groupMemberInfo.Guid == this.Guid;
		}
	}
}