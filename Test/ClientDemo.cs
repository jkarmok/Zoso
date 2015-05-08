using System.Collections.Generic;
using System.Threading;
using ExitGames.Client.Photon;
using System;
using System.Diagnostics;
using Karen90MmoFramework;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Server.Game;

namespace Karen90MmoTests
{
	public class ClientDemo : IPhotonPeerListener, IDemo
	{
		public void Run()
		{
			PhotonPeer.RegisterType(typeof(CharacterStructure), (byte)CustomTypeCode.CharacterStructure,
									GlobalSerializerMethods.SerializeCharacterStructure,
									GlobalSerializerMethods.DeserializeCharacterStructure);

			for (var i = 0; i < 20; i++)
			{
				var program = new ClientDemo();

				var id = i;
				new Thread(o => program.Run("debug" + id, "password")).Start();

				Thread.Sleep(1000);
			}
		}

		private PhotonPeer peer;
		private string username;
		private string password;

		private void Run(string username, string password)
		{
			this.username = username;
			this.password = password;

			peer = new PhotonPeer(this, ConnectionProtocol.Udp) { ChannelCount = 2 };
			peer.Connect("127.0.0.1:5055", "Master");

			var timer = Stopwatch.StartNew();
			timer.Start();

			while (true)
			{
				if (timer.ElapsedMilliseconds < 50)
					continue;

				peer.Service();
				timer.Restart();
			}
		}

		public void DebugReturn(DebugLevel level, string message)
		{
			Console.WriteLine(message);
		}

		public void OnEvent(EventData eventData)
		{
			throw new NotImplementedException();
		}

		public void OnOperationResponse(OperationResponse operationResponse)
		{
			switch ((GameOperationCode)operationResponse.OperationCode)
			{
				case GameOperationCode.LoginUser:
					{
						if (operationResponse.ReturnCode == (byte)ResultCode.Ok)
						{
							Console.WriteLine(username + " logged in");
							CreateCharacter(peer, new CharacterStructure(0, 0, 0, username));
						}
					}
					break;
			}
		}

		public void OnStatusChanged(StatusCode statusCode)
		{
			switch (statusCode)
			{
				case StatusCode.Connect:
					peer.EstablishEncryption();
					break;

				case StatusCode.Disconnect:
				case StatusCode.DisconnectByServer:
				case StatusCode.DisconnectByServerLogic:
				case StatusCode.DisconnectByServerUserLimit:
				case StatusCode.TimeoutDisconnect:
					Console.WriteLine(username + " disconnected");
					break;

				case StatusCode.EncryptionEstablished:
					Login(peer, username, password);
					break;
			}
		}

		/// <summary>
		/// Creates a new user
		/// </summary>
		public static void CreateNewUser(PhotonPeer peer, string username, string password)
		{
			var parameters = new Dictionary<byte, object>()
			{
				{ (byte)1, (byte)GameOperationCode.CreateUser },
				{ (byte)ParameterCode.Username, username },
				{ (byte)ParameterCode.Password, password },
			};

			peer.OpCustom((byte)ClientOperationCode.Login, parameters, true, 0, true);
		}

		/// <summary>
		/// Logs in a user
		/// </summary>
		public static void Login(PhotonPeer peer, string username, string password)
		{
			var parameters = new Dictionary<byte, object>()
			{
				{ (byte)1, (byte)GameOperationCode.LoginUser },
				{ (byte)ParameterCode.Username, username },
				{ (byte)ParameterCode.Password, password },
			};

			peer.OpCustom((byte)ClientOperationCode.Login, parameters, true, 0, true);
		}

		/// <summary>
		/// Creates a new character
		/// </summary>
		public static void CreateCharacter(PhotonPeer peer, CharacterStructure characterInfo)
		{
			var parameters = new Dictionary<byte, object>()
			{
				{ (byte)1, (byte)GameOperationCode.CreateCharacter },
				{ (byte)ParameterCode.Data, characterInfo },
			};

			peer.OpCustom((byte)ClientOperationCode.Character, parameters, true);
		}
	}
}
