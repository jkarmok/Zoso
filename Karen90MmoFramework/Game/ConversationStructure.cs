namespace Karen90MmoFramework.Game
{
	public struct ConversationStructure
	{
		public int ObjectId;
		public short ConversationId;

		public ConversationStructure(int objectId, short conversationId)
		{
			this.ObjectId = objectId;
			this.ConversationId = conversationId;
		}
	}
}