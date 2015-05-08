using Photon.SocketServer;

using Karen90MmoFramework.Database;
using Karen90MmoFramework.Server.Core;

namespace Karen90MmoFramework.Server.World
{
	public class WorldApplication : SubServerApplication
	{
		#region Constants and Fields

		//private readonly CounterSamplePublisher counterPublisher = new CounterSamplePublisher(1);

		#endregion

		#region Properties

		/// <summary>
		/// Gets the character database
		/// </summary>
		public DatabaseFactory CharacterDatabase { get; private set; }

		/// <summary>
		/// Gets the item database
		/// </summary>
		public DatabaseFactory ItemDatabase { get; private set; }

		/// <summary>
		/// Gets the world database
		/// </summary>
		public DatabaseFactory WorldDatabase { get; private set; }

		#endregion

		#region Constructors and Destructors

		public WorldApplication()
			: base(SubServerType.World)
		{
		}

		#endregion

		#region SubServerApplication Overrides

		protected override void Setup()
		{
			base.Setup();

			// setting up the character database
			this.CharacterDatabase = new DatabaseFactory("CharacterDB");
			this.CharacterDatabase.Initialize();

			// setting up the world database
			this.WorldDatabase = new DatabaseFactory("WorldDB");
			this.WorldDatabase.Initialize();

			// setting up the item database
			this.ItemDatabase = new DatabaseFactory("ItemDB");
			this.ItemDatabase.Initialize();

			// registering custom types with the serializer
			TypeSerializer.RegisterType(CustomTypeCode.ItemStructure);
			TypeSerializer.RegisterType(CustomTypeCode.ContainerItemStructure);
			TypeSerializer.RegisterType(CustomTypeCode.ActionItemStructure);
			TypeSerializer.RegisterType(CustomTypeCode.SlotItemStructure);
			TypeSerializer.RegisterType(CustomTypeCode.MenuItemStructure);
			TypeSerializer.RegisterType(CustomTypeCode.ActiveQuestStructure);
			TypeSerializer.RegisterType(CustomTypeCode.QuestProgressStructure);

			//this.counterPublisher.AddCounter(PhotonCounter.EventSentCount, "EventSentCounter");
			//this.counterPublisher.AddCounter(PhotonCounter.EventSentPerSec, "EventSentPerSecCounter");
			//this.counterPublisher.AddCounter(PhotonCounter.OperationReceiveCount, "OperationReceiveCounter");
			//this.counterPublisher.AddCounter(PhotonCounter.OperationReceivePerSec, "OperationReceivePerSecCounter");
			//this.counterPublisher.AddCounter(CpuUsageCounterReader.Instance, "CpuUsage");
			
			//this.counterPublisher.Start();
		}

		protected override void TearDown()
		{
			this.CharacterDatabase.Dispose();
			this.WorldDatabase.Dispose();
			this.ItemDatabase.Dispose();

			base.TearDown();
		}

		protected override OutgoingMasterServerPeer CreateOutgoingMasterPeer(InitResponse initResponse)
		{
			return new WorldServerPeer(initResponse, this);
		}

		#endregion
	}
}
