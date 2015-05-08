using System;
using System.IO;

using ExitGames.Logging.Log4Net;

using log4net;
using log4net.Config;

using Photon.SocketServer;

using Karen90MmoFramework.Server.Config;

using LogManager = ExitGames.Logging.LogManager;

namespace Karen90MmoFramework.Server.Master
{
	public class MasterApplication : ApplicationBase
	{
		#region Constants and Fields

		/// <summary>
		/// logger
		/// </summary>
		protected readonly ExitGames.Logging.ILogger Logger = LogManager.GetCurrentClassLogger();

		#endregion

		#region Properties

		/// <summary>
		/// Gets the server settings
		/// </summary>
		public ServerConfig Settings { get; private set; }

		/// <summary>
		/// Gets the master lobby
		/// </summary>
		public Lobby MasterLobby { get; private set; }

		#endregion
		
		#region ApplicationBase Overrides

		protected override PeerBase CreatePeer(InitRequest initRequest)
		{
			// if the request is from a game server create the server peer
			if (IsGameServer(initRequest))
				return new IncomingSubServerPeer(initRequest, this);

			// if the request is from a client then create a client peer otherwise null which will disconnect the connecting peer
			return this.IsGameClient(initRequest) ? new MasterClientPeer(initRequest, this) : null;
		}

		protected override void Setup()
		{
			// this will let us send raw bytes
			Protocol.AllowRawCustomValues = true;
			// init the settings
			this.Settings = ServerConfig.Initialize(this.BinaryPath);
			this.MasterLobby = new Lobby();
			// register custom types
			TypeSerializer.RegisterType(CustomTypeCode.Guid);
			// setup logging
			this.SetupLogging();
			// log any unhandled exception
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			if (Logger.IsDebugEnabled)
				Logger.DebugFormat("(:> ********** <:) MasterServer Started > {0} (:> ********** <:)", DateTime.Now);
		}

		protected override void TearDown()
		{
			if (Logger.IsDebugEnabled)
				Logger.DebugFormat("(:> ********** <:) MasterServer Stopped > {0} (:> ********** <:)", DateTime.Now);
		}

		protected override void OnStopRequested()
		{
			// dispose the lobby
			this.MasterLobby.Dispose();
		}

		#endregion

		#region Server Type Check Methods

		protected virtual bool IsGameServer(InitRequest initRequest)
		{
			return initRequest.LocalPort == Settings.MasterServer.ServerPort;
		}

		protected virtual bool IsGameClient(InitRequest initRequest)
		{
			return initRequest.LocalPort == Settings.MasterServer.ClientPort;
		}

		#endregion

		#region Local Methods

		private void SetupLogging()
		{
			var file = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));

			if (file.Exists)
			{
				LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
				GlobalContext.Properties["LogFileName"] = "MS_" + this.ApplicationName;
				XmlConfigurator.ConfigureAndWatch(file);
			}
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error(e.ExceptionObject);
		}

		#endregion
	}
}
