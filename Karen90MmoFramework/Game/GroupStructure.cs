namespace Karen90MmoFramework.Game
{
	public struct GroupStructure
	{
		public long GroupId;
		public long LeaderId;

		public GroupStructure(long groupId, long leaderId)
		{
			this.GroupId = groupId;
			this.LeaderId = leaderId;
		}
	}
}