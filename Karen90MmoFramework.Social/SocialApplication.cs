using Photon.SocketServer;

using Karen90MmoFramework.Database;
using Karen90MmoFramework.Server.Core;

namespace Karen90MmoFramework.Server.Social
{
	public class SocialApplication : SubServerApplication
	{
		#region Properties

		/// <summary>
		/// Gets the character database
		/// </summary>
		public DatabaseFactory CharacterDatabase { get; private set; }

		#endregion

		#region Constructors and Destructors

		public SocialApplication()
			: base(SubServerType.Social)
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
			
			// registering custom types with the serializer
			TypeSerializer.RegisterType(CustomTypeCode.ProfileStructure);
			TypeSerializer.RegisterType(CustomTypeCode.GroupStructure);
			TypeSerializer.RegisterType(CustomTypeCode.GroupMemberStructure);
		}

		protected override void TearDown()
		{
			this.CharacterDatabase.Dispose();
			base.TearDown();
		}

		protected override OutgoingMasterServerPeer CreateOutgoingMasterPeer(InitResponse initResponse)
		{
			return new SocialServerPeer(initResponse, this);
		}

		protected override PeerBase CreatePeer(InitRequest initRequest)
		{
			return this.IsGameServer(initRequest) ? new IncomingSocialListenerPeer(initRequest, this) : null;
		}

		#endregion
	}
}
