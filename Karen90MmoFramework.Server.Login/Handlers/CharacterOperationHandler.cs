using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Karen90MmoFramework.Quantum;
using Photon.SocketServer;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game;
using Karen90MmoFramework.Server.Game.Systems;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToServer;
using Karen90MmoFramework.Server.ServerToServer.Operations;
using Karen90MmoFramework.Server.ServerToClient.Operations;

namespace Karen90MmoFramework.Server.Login.Handlers
{
	public class CharacterOperationHandler : IGameOperationHandler
	{
		#region Constants and Fields

		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		private readonly LoginServerPeer peer;
		private readonly LoginApplication application;

		#endregion

		#region Constructors and Destructors

		public CharacterOperationHandler(LoginServerPeer peer, LoginApplication application)
		{
			this.application = application;
			this.peer = peer;
		}

		#endregion

		#region IClientOperationHandler Implementation

		public void OnOperationRequest(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			switch ((GameOperationCode) operationRequest.OperationCode)
			{
				case GameOperationCode.LogoutUser:
					{
						this.ExecUserOperation(() => this.HandleOperationLogoutUser(operationRequest, parameters), operationRequest.ClientId, parameters);
					}
					break;

				case GameOperationCode.LoginCharacter:
					{
						this.ExecUserOperation(() => this.HandleOperationLoginCharacter(operationRequest, parameters), operationRequest.ClientId, parameters);
					}
					break;

				case GameOperationCode.CreateCharacter:
					{
						this.ExecUserOperation(() => this.HandleOperationCreateCharacter(operationRequest, parameters), operationRequest.ClientId, parameters);
					}
					break;

				case GameOperationCode.RetrieveCharacters:
					{
						this.ExecUserOperation(() => this.HandleOperationRetrieveCharacters(operationRequest, parameters), operationRequest.ClientId, parameters);
					}
					break;

				case GameOperationCode.DeleteCharacter:
					{
						this.ExecUserOperation(() => this.HandleOperationDeleteCharacter(operationRequest, parameters), operationRequest.ClientId, parameters);
					}
					break;

				default:
					{
						this.peer.SendOperationResponse(operationRequest.ClientId,
						                                      new GameErrorResponse(operationRequest.OperationCode)
							                                      {
								                                      ReturnCode = (short) ResultCode.OperationNotAvailable,
							                                      },
						                                      new MessageParameters());
					}
					break;
			}
		}

		#endregion

		#region Operation Handler Methods

		protected virtual GameOperationResponse HandleOperationLogoutUser(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			this.peer.SendOperationRequest(new OperationRequest((byte) ServerOperationCode.KillClient,
			                                                    new KillClient
				                                                    {
					                                                    ClientId = operationRequest.ClientId
				                                                    }),
			                               new SendParameters());

			return null;
		}

		protected virtual GameOperationResponse HandleOperationLoginCharacter(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new LoginCharacter(this.peer.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			ThreadPool.QueueUserWorkItem(
				o => this.ExecUserOperation(() => this.HandleLoginCharacter(operationRequest.ClientId, operation), operationRequest.ClientId, parameters));

			return null;
		}

		protected virtual GameOperationResponse HandleOperationRetrieveCharacters(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new RetrieveCharacters(this.peer.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			ThreadPool.QueueUserWorkItem(
				o => this.ExecUserOperation(() => this.HandleRetrieveCharacters(operationRequest.ClientId, operation), operationRequest.ClientId, parameters));

			return null;
		}

		protected virtual GameOperationResponse HandleOperationCreateCharacter(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new CreateCharacter(this.peer.Protocol, operationRequest);
			if (!operation.IsValid)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter, operation.GetErrorMessage());

			ThreadPool.QueueUserWorkItem(
				o => this.ExecUserOperation(() => this.HandleCreateCharacter(operationRequest.ClientId, operation), operationRequest.ClientId, parameters));

			return null;
		}

		protected virtual GameOperationResponse HandleOperationDeleteCharacter(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Helper Methods

		private GameOperationResponse HandleRetrieveCharacters(int sessionId, RetrieveCharacters operation)
		{
			try
			{
				var characters = this.application.CharacterDatabase.Query<PlayerData>("PlayerData/ByUsername")
					//.Customize(x => x.WaitForNonStaleResultsAsOfNow())
					.Where(chr => chr.Username.Equals(operation.Username, StringComparison.CurrentCultureIgnoreCase))
					.Select(chr => new {chr.Name, chr.Race, chr.Origin, chr.Level})
					.ToArray();

				if (characters.Length > 0)
				{
					var characterCollection = new CharacterStructure[characters.Length];
					for (var i = 0; i < characters.Length; i++)
					{
						var chr = characters[i];
						characterCollection[i] = new CharacterStructure(chr.Race, chr.Origin, chr.Level, chr.Name);
					}

					return new RetrieveCharactersResponse(operation.OperationCode)
						{
							ReturnCode = (short) ResultCode.Ok,
							Characters = characterCollection
						};
				}

				return operation.GetErrorResponse((short) ResultCode.Ok);
			}
			catch (Exception e)
			{
				_logger.Error(e);
				return operation.GetErrorResponse((short) ResultCode.Fail);
			}
		}

		private GameOperationResponse HandleCreateCharacter(int sessionId, CreateCharacter operation)
		{
			try
			{
				var charInfo = operation.CharacterData;
				var characterName = charInfo.Name;

				var existingPlayer = this.application.CharacterDatabase.Query<PlayerData>("PlayerData/ByName")
					//.Customize(x => x.WaitForNonStaleResultsAsOfNow())
					.Select(player => new {player.Name})
					.FirstOrDefault(player => player.Name.Equals(characterName, StringComparison.CurrentCultureIgnoreCase));

				if (existingPlayer != null)
					return operation.GetErrorResponse((short) ResultCode.CharacterNameAlreadyExists);

				ResultCode resultCode;
				if (CharacterHelper.IsValidCharacterName(characterName, out resultCode) == false)
					return operation.GetErrorResponse((short) resultCode);

				if (CharacterHelper.IsValidCharacterInfo(charInfo, out resultCode) == false)
					return operation.GetErrorResponse((short) resultCode);

				var guid = Utils.NewGuidInt32(GuidCreationCulture.Utc);
				var charStats = CharacterSettings.NewCharacterDefaultStats;

				var actionItemsCount = Mathf.Min(CharacterSettings.NewCharacterDefaultSpells.Length, GlobalGameSettings.PLAYER_ACTION_BAR_SIZE);
				var actionItems = new int[actionItemsCount];
				for (var i = 0; i < actionItemsCount; i++)
					actionItems[i] = (int) new ActionItemStructure(CharacterSettings.NewCharacterDefaultSpells[i], (byte) ActionItemType.Spell, (byte) i);

				var inventoryItemsCount = Mathf.Min(CharacterSettings.NewCharacterDefaultItems.Length, GlobalGameSettings.PLAYER_DEFAULT_INVENTORY_SIZE);
				var inventoryItems = new int[inventoryItemsCount];
				for (byte i = 0; i < inventoryItemsCount; i++)
				{
					var inventoryItem = CharacterSettings.NewCharacterDefaultItems[i];
					inventoryItems[i] = (int) new ContainerItemStructure(inventoryItem.ItemId, i, inventoryItem.Count);
				}

				var spellsInfo = SpellManager.CreateNew().ToDataField();
				spellsInfo.ClientSpells = CharacterSettings.NewCharacterDefaultSpells;

				var newPlayerData = new PlayerData
					{
						Id = PlayerData.GenerateId(guid),
						Guid = guid,
						Username = operation.Username,
						GmLevel = 0,

						Name = characterName,
						Race = charInfo.Race,
						Origin = charInfo.Origin,
						Species = (byte) Species.Humanoid,
						Level = CharacterSettings.NEW_CHARACTER_DEFAULT_LEVEL,
						Orientation = CharacterSettings.NewCharacterDefaultOrientation,
						Position = CharacterSettings.NewCharacterDefaultPosition.ToFloatArray(3),
						ZoneId = CharacterSettings.NEW_CHARACTER_DEFAULT_ZONE_ID,

						Inventory = new InventoryData {Size = GlobalGameSettings.PLAYER_DEFAULT_INVENTORY_SIZE, Items = inventoryItems},
						Stats = charStats,
						Spells = spellsInfo,
						ActionBar = new ActionBarData {Items = actionItems},
						CurrentQuests = new Dictionary<short, QuestProgression>(0),
						FinishedQuests = new short[0],
						CurrHealth = charStats[Stats.Health - Stats.eFirstStat],
						CurrMana = charStats[Stats.Power - Stats.eFirstStat],

						InitLogin = true,
						CreatedOn = DateTime.Now,
						LastPlayed = null
					};
				this.application.CharacterDatabase.Store(newPlayerData);

				var socialProfileData = new SocialProfileData
					{
						Id = SocialProfileData.GenerateId(newPlayerData.Guid),
						Guid = newPlayerData.Guid,
						GmLevel = newPlayerData.GmLevel,
						Name = newPlayerData.Name,
						Level = newPlayerData.Level,

						Friends = new string[0],
						Ignores = new string[0]
					};
				this.application.CharacterDatabase.Store(socialProfileData);

				return new CreateCharacterResponse(operation.OperationCode)
					{
						ReturnCode = (short) ResultCode.Ok,
						CharacterData = charInfo
					};
			}
			catch (Exception e)
			{
				_logger.Error(e);
				return operation.GetErrorResponse((short) ResultCode.Fail);
			}
		}

		private GameOperationResponse HandleLoginCharacter(int sessionId, LoginCharacter operation)
		{
			try
			{
				var playerData = application.CharacterDatabase.Query<PlayerData>("PlayerData/ByUsernameAndByName")
					//.Customize(x => x.WaitForNonStaleResultsAsOfNow())
					.Select(chr => new {chr.Username, chr.Name, chr.Guid, chr.ZoneId})
					.FirstOrDefault(
						chr =>
						chr.Username.Equals(operation.Username, StringComparison.CurrentCultureIgnoreCase) &&
						chr.Name.Equals(operation.CharacterName, StringComparison.CurrentCultureIgnoreCase));

				if (playerData == null)
					return operation.GetErrorResponse((short) ResultCode.PlayerNotFound);

				// requesting master to zone in the client
				this.peer.SendOperationRequest(new OperationRequest((byte) ServerOperationCode.AckClientCharacterLogin,
				                                                    new AckClientCharacterLogin
					                                                    {
						                                                    SessionId = sessionId,
																			Guid = playerData.Guid,
																			CharacterName = playerData.Name,
						                                                    ZoneId = playerData.ZoneId,
					                                                    }),
				                               new SendParameters());
				return null;
			}
			catch (Exception e)
			{
				_logger.Error(e);
				return operation.GetErrorResponse((short) ResultCode.Fail);
			}
		}

		private void ExecUserOperation(Func<GameOperationResponse> operation, int sessionId, MessageParameters parameters)
		{
			var response = operation();
			if (response != null)
				this.peer.SendOperationResponse(sessionId, response, parameters);
		}

		#endregion
	}
}
