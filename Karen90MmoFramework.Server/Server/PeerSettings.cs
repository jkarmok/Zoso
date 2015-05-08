namespace Karen90MmoFramework.Server
{
	public static class PeerSettings
	{
		static PeerSettings()
		{
			MaxLockWaitTime = 1000;
			RescheduleInterval = 4000;

			MmoObjectEventChannel = 0;
			GroupEventChannel = 1;
			InventoryEventChannel = 1;
			LootEventChannel = 1;
			QuestEventChannel = 1;
			LowPriorityChannel = 1;
			ChatEventChannel = 1;
		}

		/// <summary>
		/// Max Lock Wait Time for ReaderWriterLocks
		/// </summary>
		public static int MaxLockWaitTime { get; set; }

		/// <summary>
		/// Reschedule Request Interval
		/// </summary>
		public static int RescheduleInterval { get; set; }

		/// <summary>
		/// Item event channel in which item events are published through
		/// </summary>
		public static byte MmoObjectEventChannel { get; set; }

		/// <summary>
		/// Group event channel
		/// </summary>
		public static byte GroupEventChannel { get; set; }

		/// <summary>
		/// Inventory event channel
		/// </summary>
		public static byte InventoryEventChannel { get; set; }

		/// <summary>
		/// Loot event channel
		/// </summary>
		public static byte LootEventChannel { get; set; }

		/// <summary>
		/// Quest event channel
		/// </summary>
		public static byte QuestEventChannel { get; set; }

		/// <summary>
		/// Slow channel
		/// </summary>
		public static byte ChatEventChannel { get; set; }

		/// <summary>
		/// Slow channel
		/// </summary>
		public static byte SocialEventChannel { get; set; }

		/// <summary>
		/// Slow channel
		/// </summary>
		public static byte LowPriorityChannel { get; set; }
	}
}
