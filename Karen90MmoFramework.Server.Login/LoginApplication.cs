using Photon.SocketServer;

using Karen90MmoFramework.Database;
using Karen90MmoFramework.Server.Core;

namespace Karen90MmoFramework.Server.Login
{
	public class LoginApplication : SubServerApplication
	{
		#region Constants and Fields

		private DatabaseFactory userDatabase;
		private DatabaseFactory characterDatabase;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the user database
		/// </summary>
		public IDatabase UserDatabase
		{
			get
			{
				return this.userDatabase;
			}
		}

		/// <summary>
		/// Gets the character database
		/// </summary>
		public IDatabase CharacterDatabase
		{
			get
			{
				return this.characterDatabase;
			}
		}

		#endregion

		#region Constructors and Destrucotors

		/// <summary>
		/// Creates a new <see cref="LoginApplication"/>.
		/// </summary>
		public LoginApplication()
			: base(SubServerType.Login)
		{
		}

		#endregion

		#region SubServerApplication Overrides

		protected override void Setup()
		{
			base.Setup();

			this.userDatabase = new DatabaseFactory("UserDB");
			this.userDatabase.Initialize();

			this.characterDatabase = new DatabaseFactory("CharacterDB");
			this.characterDatabase.Initialize();

			TypeSerializer.RegisterType(CustomTypeCode.CharacterStructure);
		}

		protected override void TearDown()
		{
			this.userDatabase.Dispose();
			this.characterDatabase.Dispose();

			base.TearDown();
		}

		protected override OutgoingMasterServerPeer CreateOutgoingMasterPeer(InitResponse initResponse)
		{
			return new LoginServerPeer(initResponse, this);
		}

		#endregion

	}
}
