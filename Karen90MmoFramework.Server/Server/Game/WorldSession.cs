using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Karen90MmoFramework.Rpc;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Concurrency;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Items;
using Karen90MmoFramework.Server.Game.Objects;
using Karen90MmoFramework.Server.Game.Systems;
using Karen90MmoFramework.Server.ServerToClient;
using Karen90MmoFramework.Server.ServerToClient.Events;
using Karen90MmoFramework.Server.ServerToClient.Operations;
using LootItem = Karen90MmoFramework.Server.ServerToClient.Operations.LootItem;

namespace Karen90MmoFramework.Server.Game
{
	public sealed class WorldSession : IDisposable, IPeer, ISession
	{
		#region Constants and Fields

		/// <summary>
		/// logger
		/// </summary>
		private static readonly ExitGames.Logging.ILogger _logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		private readonly int sessionId;
		private readonly string name;
		private readonly bool transfer;
		private int guid;

		private readonly MmoWorld world;
		private readonly IWorldServer server;
		private readonly ISerialFiber fiber;

		private readonly AsyncEvent<ExitWorldReason> onExitWorld;
		private readonly AsyncEvent onDisconnect;

		private readonly List<MenuItemStructure> dialogues;

		private int sessionState;
		private int gmLevel;

		private MmoPlayerCamera camera;
		private Player player;

		private bool transferring;
		private GlobalPosition teleportLocation;
		private bool disposed;

		private MovementKeys lastKeys;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the session state
		/// </summary>
		public WorldSessionState SessionState
		{
			get
			{
				return (WorldSessionState) sessionState;
			}

			private set
			{
				Interlocked.Exchange(ref sessionState, (int) value);
			}
		}

		/// <summary>
		/// Gets the world
		/// </summary>
		public MmoWorld World
		{
			get
			{
				return this.world;
			}
		}

		/// <summary>
		/// Gets the guid
		/// </summary>
		public int Guid
		{
			get
			{
				return this.guid;
			}
		}

		/// <summary>
		/// The fiber used for player related tasks
		/// </summary>
		public ISerialFiber Fiber
		{
			get
			{
				return this.fiber;
			}
		}

		/// <summary>
		/// Gets the player
		/// </summary>
		public Player Player
		{
			get
			{
				return this.player;
			}

			private set
			{
				Interlocked.Exchange(ref player, value);
			}
		}

		/// <summary>
		/// Gets or sets the camera
		/// </summary>
		public MmoPlayerCamera Camera
		{
			get
			{
				return this.camera;
			}

			private set
			{
				Interlocked.Exchange(ref camera, value);
			}
		}

		/// <summary>
		/// Gets the disconnection channel
		/// </summary>
		public AsyncEvent<ExitWorldReason> OnExitWorld
		{
			get
			{
				return this.onExitWorld;
			}
		}

		/// <summary>
		/// Gets the async event to listen for disconnects
		/// </summary>
		public AsyncEvent OnDisconnect
		{
			get
			{
				return this.onDisconnect;
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="WorldSession"/> class,
		/// </summary>
		public WorldSession(int sessionId, string name, IWorldServer server, MmoWorld world, bool transfer)
		{
			this.server = server;
			this.world = world;
			this.sessionId = sessionId;
			this.name = name;
			this.transfer = transfer;

			this.fiber = new SerialPoolFiber();
			this.fiber.Start();

			this.dialogues = new List<MenuItemStructure>();

			this.SessionState = WorldSessionState.Login;
			this.onExitWorld = new AsyncEvent<ExitWorldReason>();
			this.onDisconnect = new AsyncEvent();
		}

		/// <summary>
		/// Player destructor. Disposes the player
		/// </summary>
		~WorldSession()
		{
			this.Dispose(false);
		}

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		/// Disposes the <see cref="WorldSession"/>.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (player != null && !player.Disposed)
					{
						// destroying the avatar
						this.player.Destroy();
						// disposing the avatar is important
						// because it will publish an object disposed message
						this.player.Dispose();
					}

					if (camera != null)
						this.camera.Dispose();
				}

				this.onExitWorld.ClearSubscribers();
				this.fiber.Dispose();
			}

			this.disposed = true;
		}

		#endregion

		#region Implementation of IPeer

		/// <summary>
		/// Sends an operation response to the client
		/// </summary>
		public void SendOperationResponse(GameOperationResponse response, MessageParameters parameters)
		{
			this.server.SendOperationResponse(sessionId, response, parameters);
		}

		/// <summary>
		/// Sends an operation response to the client
		/// </summary>
		public void SendEvent(GameEvent eventData, MessageParameters parameters)
		{
			this.server.SendEvent(sessionId, eventData, parameters);
		}

		/// <summary>
		/// Disconnects the client
		/// </summary>
		public void Disconnect()
		{
			this.server.KillClient(sessionId);
		}

		#endregion

		#region Implementation of ISession

		/// <summary>
		/// Gets the session Id
		/// </summary>
		public int SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		/// <summary>
		/// Gets the character name
		/// </summary>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Queues an <see cref="GameOperationRequest"/> to be processed
		/// </summary>
		void ISession.ReceiveOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters)
		{
			this.fiber.Enqueue(() => this.OnOperationRequest(operationRequest, messageParameters));
		}

		/// <summary>
		/// Destroys the session
		/// </summary>
		/// <param name="destroyReason"></param>
		public void Destroy(DestroySessionReason destroyReason)
		{
			this.fiber.Enqueue(() =>
				{
					// making a copy of our current state since this can change
					var currentState = this.SessionState;
					// setting the session state right before doing cleanup to update the value immediately
					this.SessionState = WorldSessionState.Destroyed;
					// do not move it outside the thread
					// because this will be ignored if there are two destroy calls queued at the same time
					if (currentState == WorldSessionState.Destroyed)
						return;
					// if we are at the login stage we dont need to dispose ourselves
					// since there is no player created or entered the zone yet
					if (currentState == WorldSessionState.Login)
					{
						this.Dispose();
						return;
					}
					
					this.onExitWorld.Invoke(destroyReason == DestroySessionReason.Transfer ? ExitWorldReason.Transfer : ExitWorldReason.Logout);
					if(destroyReason != DestroySessionReason.Transfer)
						this.onDisconnect.Invoke();

					world.SocialManager.LeaveWorld(SessionId, player.CurrentZone.Id);
					world.ChatManager.LeaveChannel(SessionId, player.CurrentZone.LocalChatChannel);
					world.ChatManager.LeaveChannel(SessionId, player.CurrentZone.TradeChatChannel);

					var zone = player.CurrentZone;
					if (!zone.Disposed)
					{
						// remove the avatar from the world
						while (!zone.Remove(player.Guid))
						{
							MmoObject existingActor;
							if (!zone.ObjectCache.TryGetItem(player.Guid, out existingActor))
								break;
						}
					}
					player.Destroy();
					player.Dispose();

					if (!world.Disposed)
					{
						// remove us from the chat
						while (!world.RemoveSession(this))
						{
							WorldSession existingSession;
							if (!world.SessionCache.TryGetSessionBySessionId(SessionId, out existingSession))
								return;
						}
					}
					this.Dispose();
					this.SaveCharacter();
				});
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Transfer the session to another world
		/// </summary>
		public void Transfer(short zoneId, Vector3 position)
		{
			lock (this)
			{
				if (transferring || disposed || SessionState == WorldSessionState.Destroyed || SessionState == WorldSessionState.Login)
					return;

				this.transferring = true;
				this.teleportLocation = new GlobalPosition(zoneId, position);
				this.server.TransferSession(SessionId, zoneId);
			}
		}

		#endregion

		#region Local Methods

		/// <summary>
		/// Saves the character on the database
		/// </summary>
		private void SaveCharacter()
		{
			using (var session = this.server.CharacterDatabase.OpenSession())
			{
				var id = PlayerData.GenerateId(Player.Guid.Id);
				var playerData = session.Load<PlayerData>(id);

				if (playerData == null)
					throw new NullReferenceException(string.Format("Cannot locate Id={0} in Character Database", id));

				this.Player.UpdateSaveData(playerData);
				if (transferring)
				{
					playerData.ZoneId = teleportLocation.ZoneId;
					playerData.Position = teleportLocation.Position.ToFloatArray(3);
				}
				playerData.LastPlayed = DateTime.Now;

				if (playerData.InitLogin)
					playerData.InitLogin = false;

				session.Store(playerData);
				session.SaveChanges();
			}
		}

		/// <summary>
		/// Gets all items from the loot or <value>NULL</value>.
		/// </summary>
		private static ContainerItemStructure[] GetAvailableItems(ILoot loot)
		{
			ContainerItemStructure[] availableItems = null;

			var lootItems = loot.LootItems;
			if (lootItems.Length > 1)
			{
				// looping thru the loot collection to see if there are any unlooted items left
				// the reason for doing it in two loops is to avoid creating a list and converting it to array because we do not know how many unlooted items are there in advance
				// since loots dont contain too many items, an interation over a few collection is faster than resizing and converting a list to an array
				var unlootedCount = 0;
				for (var i = 1; i < lootItems.Length; i++)
				{
					if (!lootItems[i].IsLooted)
						unlootedCount++;
				}

				// if we have any unlooted items
				if (unlootedCount > 0)
				{
					// create the collection
					availableItems = new ContainerItemStructure[unlootedCount];
					unlootedCount = 0;

					for (var i = 1; i < lootItems.Length; i++)
					{
						// if its already looted continue
						var lootItem = lootItems[i];
						if (lootItem.IsLooted)
							continue;

						// explicit conversion ItemInfo to int
						availableItems[unlootedCount++] = new ContainerItemStructure(lootItem.ItemId, (byte) i, (byte) lootItem.CountOrGold);
					}
				}
			}

			return availableItems;
		}

		/// <summary>
		/// Gets the gold from the loot or <value>0</value>.
		/// </summary>
		private static int GetAvailableGold(ILoot loot)
		{
			return loot.LootItems[0].TryGetGold();
		}

		#endregion

		#region Operation Handlers

		// immediate operation
		private GameOperationResponse HandleOperationEnterWorld()
		{
			var playerData = this.server.CharacterDatabase.Query<PlayerData>("PlayerData/ByName")
				//.Customize(x => x.WaitForNonStaleResultsAsOfNow())
				.FirstOrDefault(chr => chr.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase));
			if (playerData == null)
			{
				// this can only happen if the character was deleted before we login
				// still the master should disconnect us before letting us enter the world
				// we are handling it to satisfy Murphy's Law
				_logger.ErrorFormat("[HandleOperationEnterWorld]: PlayerData (Name={0}) cannot be found", this.Name);
				this.Disconnect();
				this.Destroy(DestroySessionReason.KickedByServer);
				return null;
			}

			this.guid = playerData.Guid;
			// adding player to the world
			while (!world.AddSession(this))
			{
				WorldSession exisitingSession;
				if (world.SessionCache.TryGetSessionBySessionId(sessionId, out exisitingSession))
				{
					// this shouldn't happen because the master won't let the same player login twice it will disconnect the exisitng player
					// we are handling it to satisfy Murphy's Law
#if MMO_DEBUG
					_logger.DebugFormat("[HandleOperationEnterWorld]: PlayerSession (Name={0}) cannot be added because an existing PlayerSession (Name={1}) is found",
					                    this.Name, exisitingSession.Name);
#endif
					exisitingSession.Disconnect();
					this.Disconnect();
					this.Destroy(DestroySessionReason.KickedByExistingSession);
					return null;
				}
			}

			// finding the player's zone
			MmoZone playerZone;
			if (!world.Zones.TryGetValue(playerData.ZoneId, out playerZone))
			{
				_logger.ErrorFormat("[HandleOperationEnterWorld]: Zone (Id={0}) cannot be found", playerData.ZoneId);

				this.Disconnect();
				this.Destroy(DestroySessionReason.KickedByServer);
				return null;
			}

			var newPlayer = new Player(this, playerZone, playerData);
			// using a while loop for the lock-timeout
			while (!playerZone.Add(newPlayer))
			{
				MmoObject existingAvatarObject;
				if (playerZone.ObjectCache.TryGetItem(newPlayer.Guid, out existingAvatarObject))
				{
					// this shouldn't happen because the master won't let the same player login twice it will disconnect the exisitng player
					// we are handling it to satisfy Murphy's Law
					var exisitingAvatar = (Player) existingAvatarObject;
#if MMO_DEBUG
					_logger.DebugFormat("[HandleOperationEnterWorld]: Player (Name={0}) cannot be added because an exisiting Player (Nmae={1}) is found", newPlayer.Name,
					                    exisitingAvatar.Name);
#endif
					exisitingAvatar.Session.Disconnect();

					newPlayer.Dispose();
					world.RemoveSession(this);
					this.Disconnect();
					this.Destroy(DestroySessionReason.KickedByExistingSession);
					return null;
				}
			}

			// level 0 = normal player
			// level 1 = moderator
			// level 2 = game master
			// level 3 = admin
			this.gmLevel = playerData.GmLevel;

			this.Player = newPlayer;
			/************************************/
			/*         Updating Session         */
			this.SessionState = WorldSessionState.EnterWorld;
			/*         Updating Session         */
			/************************************/
			/************************************/
			/*        EnterWorldResponse        */
			// if the player is a transfer then the teleport position should be set
			var spawnPosition = playerData.Position.ToVector();
			// if the player is out of bounds spawn at the default position
			if (!newPlayer.CurrentZone.Bounds.Contains(spawnPosition))
				spawnPosition = newPlayer.CurrentZone.GetDefaultPlayerSpawnPosition();
			// adjusting the player's y position
			var terrainHeight = newPlayer.CurrentZone.GetHeight(spawnPosition.X, spawnPosition.Z);
			var minAltitude = terrainHeight + newPlayer.Bounds.Size.Y * 0.5f;
			// correcting player's y position
			if (spawnPosition.Y < minAltitude)
				spawnPosition.Y = minAltitude;
			// the rotation
			var spawnRotation = Quaternion.CreateEular(0, playerData.Orientation, 0);

			var enterWorldResponse = new EnterWorldResponse((byte) GameOperationCode.EnterWorld)
				{
					WorldId = newPlayer.CurrentZone.Id,
					PlayerId = newPlayer.Guid,
					Position = spawnPosition.ToFloatArray(3),
					Orientation = spawnRotation.EularAngles().Y,
				};
			if (!transfer)
				enterWorldResponse.Motd = World.GetMotd();
			this.SendOperationResponse(enterWorldResponse, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			/*        EnterWorldResponse        */
			/************************************/

			if (transfer)
				goto SendPropertiesEnd;

			#region SendProperties

			/*************************************/
			/*               Properties          */
			var properties = new Hashtable(newPlayer.GetProperties())
				{
					{(byte) PropertyCode.CurrPow, newPlayer.CurrentPower},
					{(byte) PropertyCode.MaxPow, newPlayer.MaximumPower},
					{(byte) PropertyCode.Xp, newPlayer.Xp},
					{(byte) PropertyCode.Money, newPlayer.Money},
					{(byte) PropertyCode.Stats, newPlayer.GetBaseStats().ToArray()}
				};
			var objectPropertiesEvent = new ObjectPropertyMultiple
				{
					ObjectId = newPlayer.Guid,
					Properties = properties,
					Revision = newPlayer.Revision
				};
			this.SendEvent(objectPropertiesEvent, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});

			/*            Properties            */
			/************************************/
			/************************************/
			/*             Inventory            */
			var initInventory = new InventoryInit{Size = (byte) playerData.Inventory.Size};

			var inventoryItems = playerData.Inventory.Items;
			if (inventoryItems.Length > 0)
			{
				var itemsToSend = new ContainerItemStructure[inventoryItems.Length];
				for (var i = 0; i < inventoryItems.Length; i++)
					itemsToSend[i] = (ContainerItemStructure) inventoryItems[i];
				initInventory.Items = itemsToSend;
			}

			this.SendEvent(initInventory, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			/*             Inventory            */
			/************************************/
			/************************************/
			/*              Spells              */
			var spells = playerData.Spells.ClientSpells;
			if (spells.Length > 0)
				this.SendEvent(new SpellManagerInit {Spells = spells}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			/*              Spells              */
			/************************************/
			/************************************/
			/*             ActionBar            */
			var actionItems = playerData.ActionBar.Items;
			if (actionItems.Length > 0)
			{
				var itemsToSend = new ActionItemStructure[actionItems.Length];
				for (var i = 0; i < actionItems.Length; i++)
					itemsToSend[i] = (ActionItemStructure) actionItems[i];
				this.SendEvent(new ActionbarInit {ActionItems = itemsToSend}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			}
			/*             ActionBar            */
			/************************************/
			/************************************/
			/*              Quests              */
			var questList = playerData.CurrentQuests;
			if (questList.Count > 0)
			{
				var activeQuests = new ActiveQuestStructure[questList.Count];
				var index = 0;

				foreach (var entry in questList)
					activeQuests[index++] = entry.Value.ToActiveQuestInfo(entry.Key);

				this.SendEvent(new QuestManagerInit {Quests = activeQuests}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			}
			/*              Quests              */
			/************************************/

			#endregion

			SendPropertiesEnd:
			var newCamera = new MmoPlayerCamera(this, playerZone)
				{
					ViewDistanceEnter = new Vector3(50, 0, 50),
					ViewDistanceExit = new Vector3(70, 0, 70)
				};

			this.Camera = newCamera;
			lock (newCamera.SyncRoot)
			{
				newCamera.SetOwner(newPlayer);
				newCamera.UpdateInterestManagement();
			}

			// can send out a spawned event but for our game, enter world response is enough
			playerZone.PrimaryFiber.Enqueue(() =>
				{
					if(playerData.GroupGuid.HasValue)
					{
						Group group;
						if (GroupManager.Instance.TryGetGroup(playerData.GroupGuid.Value, out group))
						{
							player.Group = group;
							group.AddReference(player);
						}
					}

					newPlayer.Spawn(spawnPosition, spawnRotation);
				});

			world.ChatManager.JoinChannel(SessionId, playerZone.LocalChatChannel);
			world.ChatManager.JoinChannel(SessionId, playerZone.TradeChatChannel);
			world.SocialManager.JoinWorld(SessionId, playerZone.Id);

			return null;
		}

		// immediate operation
		private GameOperationResponse HandleOperationLogoutUser()
		{
			// logout can be processed immediately
			this.server.KillClient(SessionId);
			return null;
		}

		// immediate operation
		private GameOperationResponse HandleOperationLogoutCharacter()
		{
			// logout can be processed immediately
			this.server.KillSession(SessionId);
			return null;
		}

		// immediate operation
		private GameOperationResponse HandleOperationGetProperties(GameOperationRequest operationRequest)
		{
			var operation = new GetProperties(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			if (operation.Flags == 0)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			MmoObject mmoObject = this.player;
			if (operation.ObjectId != this.player.Guid)
				if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId, out mmoObject))
					return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			// immediate processing of the message
			// propertiesCollection are internally synched so they can be accessed directly
			var properties = new Hashtable();
			var revision = mmoObject.Revision;

			mmoObject.BuildProperties(properties, (PropertyFlags) operation.Flags);
			if (properties.Count == 0)
				return null;

			var propertiesEvent = new ObjectPropertyMultiple
				{
					ObjectId = mmoObject.Guid,
					Properties = properties,
					Revision = revision
				};
			this.SendEvent(propertiesEvent, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationMove(GameOperationRequest operationRequest)
		{
			var operation = new Move(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => this.DoMove(operation));
			return null;
		}

		private void DoMove(Move operation)
		{
			// cant move while dead (corpse)
			if (player.State == UnitState.Dead)
				return;

			// cant move while falling or jumping
			if (player.CurrentMovementState == MovementState.Falling || player.CurrentMovementState == MovementState.Jumping)
				return;

			var keys = (MovementKeys) operation.Keys;
			var sentTime = operation.SentTime;

			var isStopping = (keys & MovementKeys.MoveKeys) == MovementKeys.None;
			var isJumping = (keys & MovementKeys.Jump) == MovementKeys.Jump;

			// removing jump key cuz it will be handled seperately
			keys &= ~MovementKeys.Jump;

			// this makes sure that the client cannot instant teleport by sending an unusual sent time
			if (world.GlobalTime - sentTime > ServerGameSettings.MAX_MOVEMENT_LATENCEY)
			{
				sentTime = world.GlobalTime - ServerGameSettings.MAX_MOVEMENT_LATENCEY;
#if MMO_DEBUG
				_logger.WarnFormat("Correcting SentTime={0} to WorldTime={1}", sentTime, world.GlobalTime);
#endif
			}

			// no movement
			if (isStopping)
			{
				if (player.IsMoving)
					player.StopMovement(sentTime);
				this.lastKeys = MovementKeys.None;
			}
			else
			{
				if (lastKeys != keys)
					player.HandleMovement(keys, sentTime);
				this.lastKeys = keys;
			}

			if (isJumping)
			{
				// TODO: Handle jumping
			}
		}

		// synchronous operation
		private GameOperationResponse HandleOperationRotate(GameOperationRequest operationRequest)
		{
			var operation = new Rotate(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => this.DoRotate(operation));
			return null;
		}

		private void DoRotate(Rotate operation)
		{
			this.player.SetOrientation(operation.Orientation);
		}

		// synchronous operation
		private GameOperationResponse HandleOperationInteract(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new Interact(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoInteract(operation), parameters));
			return null;
		}

		private GameOperationResponse DoInteract(Interact operation)
		{
			// we cannot interact with ourselves
			if (operation.ObjectId == this.player.Guid)
				return null;
			
			MmoObject mmoObject;
			// is the object in the same world ?
			if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId, out mmoObject))
				return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);
			// is the object disposed ?
			if (mmoObject.Disposed)
				return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);
			// is the object hidden for our player ?
			if (mmoObject.IsHiddenFor(player.Guid))
				return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);
			// are we within the interaction range ?
			const float minInteractionRange = GlobalGameSettings.MIN_INTERACTION_RANGE;
			if (Vector3.SqrDistance(player.Position, mmoObject.Position) > minInteractionRange * minInteractionRange)
				return operation.GetErrorResponse((short) ResultCode.ObjectIsOutOfRange);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Player:
					// cannot interact with players at the moment
					return null;

				case ObjectType.Npc:
					{
						var npc = (Npc) mmoObject;
						// if the npc is dead check to see whether we have loot or not
						if (npc.IsDead())
						{
							// if there is loot for us send the loot
							if (npc.HaveLootFor(player))
							{
								// finally, we can interact
								player.SetInteractor(mmoObject);
								// if the send loot flag is set, the client needs all loot from this object
								if (((InteractionFlags) operation.Flags & InteractionFlags.SendLoot) == InteractionFlags.SendLoot)
								{
									// extract the loot items and gold
									var playerLoot = npc.GetLootFor(player);
									var items = GetAvailableItems(playerLoot);
									var gold = GetAvailableGold(playerLoot);
									// send the interaction event with the loot list
									var interactionLootList = new InteractionLootList {ObjectId = mmoObject.Guid};
									if (items != null)
										interactionLootList.Items = items;
									// if there is gold send it also
									if (gold > 0)
										interactionLootList.Gold = gold;
									// informing the client to show the loot
									this.SendEvent(interactionLootList, new MessageParameters {ChannelId = PeerSettings.LootEventChannel});
								}
								else
								{
									// informing the client to show the loot
									this.SendEvent(new InteractionLoot {ObjectId = npc.Guid}, new MessageParameters {ChannelId = PeerSettings.LootEventChannel});
								}
							}
							return null;
						}

						// cannot interact with hostile npcs
						if (npc.IsHostileTo(player))
							return operation.GetErrorResponse((short) ResultCode.InteractionNotAvailable);
						// can we talk to the npc?
						if (player.CanTalkTo(npc))
						{
							// finally, we can interact
							player.SetInteractor(mmoObject);
							// clearing any old dialogues if there is any
							this.dialogues.Clear();
							// prepare the dialogues
							player.PrepareQuestDialogueFor(npc, dialogues);
							player.PrepareGossipDialogueFor(npc, dialogues);

							var interactionDialogueList = new InteractionDialogueList
								{
									ObjectId = npc.Guid,
									MenuItems = dialogues.ToArray()
								};
							this.SendEvent(interactionDialogueList, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
							return null;
						}

						// is the npc a merchant?
						if (npc.IsMerchant())
						{
							// finally, we can interact
							player.SetInteractor(mmoObject);
							// if the send inventory flag is set, the client needs all items from this object
							if (((InteractionFlags) operation.Flags & InteractionFlags.SendInventory) == InteractionFlags.SendInventory)
							{
								var interactionShopList = new InteractionShopList
									{
										ObjectId = npc.Guid,
										ItemList = ((IMerchant) npc).Inventory.ToArray()
									};
								this.SendEvent(interactionShopList, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
							}
							else
							{
								this.SendEvent(new InteractionShop {ObjectId = npc.Guid}, new MessageParameters {ChannelId = PeerSettings.MmoObjectEventChannel});
							}
							return null;
						}
					}
					break;

				case ObjectType.Gameobject:
					{
						var gameObject = (Gameobject) mmoObject;
						switch (gameObject.GoType)
						{
							case GameObjectType.Plant:
							case GameObjectType.Vein:
								{
									// if there isn't already loot available for us we need to gather gameobject
									if (!gameObject.HaveLootFor(player))
									{
										// TODO: Check for required tools necessary to harvest the gathering node
										// can we perform gather on the gameobject ?
										if (!gameObject.CanGather(player))
											return operation.GetErrorResponse((short)ResultCode.InteractionNotAvailable);
										// finally, we can interact
										player.SetInteractor(mmoObject);
										// gather the gameobject
										gameObject.Gather(player);
										// TODO: Query gather time
										// for now gathering spots will generate loot immediately so we can query for loot right after using
										if (gameObject.HaveLootFor(player))
										{
											// extract the loot items and gold
											var playerLoot = gameObject.GetLootFor(player);
											var items = GetAvailableItems(playerLoot);
											var gold = GetAvailableGold(playerLoot);
											// send the interaction event with the loot list
											var interactionLootList = new InteractionLootList { ObjectId = mmoObject.Guid };
											if (items != null)
												interactionLootList.Items = items;
											// if there is gold send it also
											if (gold > 0)
												interactionLootList.Gold = gold;
											// informing the client to show the loot
											this.SendEvent(interactionLootList, new MessageParameters { ChannelId = PeerSettings.LootEventChannel });
										}
										else
										{
											// no loot so cannot interact anymore
											player.SetInteractor(null);
										}
									}
									else
									{
										// finally, we can interact
										player.SetInteractor(mmoObject);
										// if the send loot flag is set, the client needs all loot from this object
										if (((InteractionFlags)operation.Flags & InteractionFlags.SendLoot) == InteractionFlags.SendLoot)
										{
											// extract the loot items and gold
											var playerLoot = gameObject.GetLootFor(player);
											var items = GetAvailableItems(playerLoot);
											var gold = GetAvailableGold(playerLoot);
											// send the interaction event with the loot list
											var interactionLootList = new InteractionLootList { ObjectId = mmoObject.Guid };
											if (items != null)
												interactionLootList.Items = items;
											// if there is gold send it also
											if (gold > 0)
												interactionLootList.Gold = gold;
											// informing the client to show the loot
											this.SendEvent(interactionLootList, new MessageParameters { ChannelId = PeerSettings.LootEventChannel });
										}
										else
										{
											// informing the client to show the loot
											this.SendEvent(new InteractionLoot { ObjectId = gameObject.Guid }, new MessageParameters { ChannelId = PeerSettings.LootEventChannel });
										}
									}
								}
								return null;

							case GameObjectType.Chest:
								{
									// if there isn't already loot available for us we need to open (use) the chest
									if (!gameObject.HaveLootFor(player))
									{
										// can we perform use on the chest ?
										if (!gameObject.CanUse(player))
											return operation.GetErrorResponse((short) ResultCode.InteractionNotAvailable);
										// finally, we can interact
										player.SetInteractor(mmoObject);
										// use the game object
										gameObject.Use(player);
										// TODO: Query use time
										// for now chests will generate loot immediately so we can query for loot right after using
										if (gameObject.HaveLootFor(player))
										{
											// extract the loot items and gold
											var playerLoot = gameObject.GetLootFor(player);
											var items = GetAvailableItems(playerLoot);
											var gold = GetAvailableGold(playerLoot);
											// send the interaction event with the loot list
											var interactionLootList = new InteractionLootList {ObjectId = mmoObject.Guid};
											if (items != null)
												interactionLootList.Items = items;
											// if there is gold send it also
											if (gold > 0)
												interactionLootList.Gold = gold;
											// informing the client to show the loot
											this.SendEvent(interactionLootList, new MessageParameters {ChannelId = PeerSettings.LootEventChannel});
										}
										else
										{
											// no loot so cannot interact anymore
											player.SetInteractor(null);
										}
									}
									else
									{
										// finally, we can interact
										player.SetInteractor(mmoObject);
										// if the send loot flag is set, the client needs all loot from this object
										if (((InteractionFlags) operation.Flags & InteractionFlags.SendLoot) == InteractionFlags.SendLoot)
										{
											// extract the loot items and gold
											var playerLoot = gameObject.GetLootFor(player);
											var items = GetAvailableItems(playerLoot);
											var gold = GetAvailableGold(playerLoot);
											// send the interaction event with the loot list
											var interactionLootList = new InteractionLootList {ObjectId = mmoObject.Guid};
											if (items != null)
												interactionLootList.Items = items;
											// if there is gold send it also
											if (gold > 0)
												interactionLootList.Gold = gold;
											// informing the client to show the loot
											this.SendEvent(interactionLootList, new MessageParameters {ChannelId = PeerSettings.LootEventChannel});
										}
										else
										{
											// informing the client to show the loot
											this.SendEvent(new InteractionLoot {ObjectId = gameObject.Guid}, new MessageParameters {ChannelId = PeerSettings.LootEventChannel});
										}
									}
								}
								return null;

							default:
								// TODO: NotImplemented
								Utils.Logger.ErrorFormat("NotImplemented");
								break;
						}
					}
					break;

				case ObjectType.Dynamic:
					{
						var dynamic = (Dynamic) mmoObject;
						switch (dynamic.DynamicType)
						{
							case DynamicType.Portal:
								// TODO: Teleport player
								break;
						}
					}
					break;

				default:
					return operation.GetErrorResponse((short) ResultCode.InteractionNotAvailable);
			}

			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationCloseInteraction(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new CloseInteraction(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoCloseInteraction(operation), parameters));
			return null;
		}

		private GameOperationResponse DoCloseInteraction(CloseInteraction operation)
		{
			// no interactor? then report error
			if (player.CurrentInteractor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);

			// resetting our interactor
			player.SetInteractor(null);
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationPurchaseItem(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new PurchaseItem(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoPurchaseItem(operation), parameters));
			return null;
		}

		private GameOperationResponse DoPurchaseItem(PurchaseItem operation)
		{
			// do we have an interactor ?
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);
			// is the interactor an npc ?
			if (interactor.IsNpc())
			{
				// if the npc is dead we cannot interact with it
				var npc = (Npc) interactor;
				if (npc.IsDead())
					return operation.GetErrorResponse((short) ResultCode.InteractorIsDead);
			}
			// is the interactor an npc ?
			if (!interactor.IsMerchant())
				return operation.GetErrorResponse((short) ResultCode.InteractorNotAMerchant);
			// is the interactor a merchant ?
			var merchant = (IMerchant) interactor;
			// serializing the index count info
			var slotItem = operation.SlotItemData;
			// validating the purchase quanity
			var quantity = slotItem.Count;
			if (quantity < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidQuantity);
			// TODO: Access the Npc Inventory directly when the Npc Inventory system is finished
			var items = merchant.Inventory.ToArray();
			// validating the index
			var slot = slotItem.Slot;
			if (slot >= items.Length)
				return operation.GetErrorResponse((short) ResultCode.ItemNotFound);
			// loading the template
			GameItemData item;
			if (!World.ItemCache.TryGetGameItemData(items[slot], out item))
				return operation.GetErrorResponse((short) ResultCode.ItemNotFound);
			// validating fund availability
			var totalPrice = item.BuyoutPrice * quantity;
			if (totalPrice > player.Money)
				return operation.GetErrorResponse((short) ResultCode.InsufficentFund);
			// try adding the item
			var added = player.Inventory.AddItem(item.ItemId, quantity);
			// verifying addition of atleast one item
			if (added == 0)
				return operation.GetErrorResponse((short) ResultCode.InventoryFull);
			// deducting money
			player.DeductGold(totalPrice, true);
			// not all the items have been added
			return added < quantity ? operation.GetErrorResponse((short) ResultCode.InventoryFull) : null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSellItem(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SellItem(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSellItem(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSellItem(SellItem operation)
		{
			// do we have an interactor ?
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);
			// is the interactor an npc ?
			if (interactor.IsNpc())
			{
				// if the npc is dead we cannot interact with it
				var npc = (Npc) interactor;
				if (npc.IsDead())
					return operation.GetErrorResponse((short) ResultCode.InteractorIsDead);
			}
			// is the interactor a merchant ?
			if (!interactor.IsMerchant())
				return operation.GetErrorResponse((short) ResultCode.InteractorNotAMerchant);
			// serializing the sale info
			var slotItem = operation.SlotItemData;
			// validating the purchase quanity
			var quantity = slotItem.Count;
			if (quantity < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidQuantity);
			// getting the index
			var slot = slotItem.Slot;
			// locating the item
			IMmoItem mmoItem;
			if (!player.Inventory.TryGetItem(slot, out mmoItem))
				return operation.GetErrorResponse((short) ResultCode.ItemNotFound);
			// are we trying to sell more than we have at the current item index?
			if (mmoItem.StackCount < quantity)
				return operation.GetErrorResponse((short) ResultCode.InvalidQuantity);
			// removing the item from the inventory
			var removed = player.Inventory.RemoveItemAt(slot, quantity);
			// we can only sell the number of items we actually removed
			if (removed > 0)
			{
				// and gain gold for sold items
				this.player.GainGold(removed * mmoItem.SellPrice, true);
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationLootItem(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new LootItem(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoLootItem(operation), parameters));
			return null;
		}

		private GameOperationResponse DoLootItem(LootItem operation)
		{
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);

			// have no loot
			if (!interactor.HaveLootFor(player))
				return operation.GetErrorResponse((short) ResultCode.NoLootAvailable);

			// remember to add one because index[0] = gold and the items at the client start at index 0 so we need to offset
			interactor.GetLootFor(player).CollectLootItem(operation.Index + 1, player);
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationLootGold(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new LootGold(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoLootGold(operation), parameters));
			return null;
		}

		private GameOperationResponse DoLootGold(LootGold operation)
		{
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);

			// have no loot
			if (!interactor.HaveLootFor(player))
				return operation.GetErrorResponse((short) ResultCode.NoLootAvailable);

			// remember gold is always at index 0
			interactor.GetLootFor(player).CollectLootItem(0, player);
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationLootAll(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new LootAll(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoLootAll(operation), parameters));
			return null;
		}

		private GameOperationResponse DoLootAll(LootAll operation)
		{
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);

			// have no loot
			if (!interactor.HaveLootFor(player))
				return operation.GetErrorResponse((short) ResultCode.NoLootAvailable);

			// collecting all loot
			interactor.GetLootFor(player).CollectAll(player);
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSelectConversationMenu(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SelectDialogue(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSelectConversationMenu(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSelectConversationMenu(SelectDialogue operation)
		{
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);

			// for now only npcs can have conversation
			if (!interactor.IsNpc())
				return operation.GetErrorResponse((short) ResultCode.ActionNotAvailable);

			// for now we cannot interact with dead npcs
			var npc = (Npc) interactor;
			if (npc.IsDead())
				return operation.GetErrorResponse((short) ResultCode.InteractorIsDead);

			player.PerformConversation(interactor, operation.DialogueId);
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationAcceptQuest(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new AcceptQuest(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoAcceptQuest(operation), parameters));
			return null;
		}

		private GameOperationResponse DoAcceptQuest(AcceptQuest operation)
		{
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);

			// for now only npcs can have quests
			if (!interactor.IsNpc())
				return operation.GetErrorResponse((short) ResultCode.ActionNotAvailable);

			// for now we cannot interact with dead npcs
			var npc = (Npc) interactor;
			if (npc.IsDead())
				return operation.GetErrorResponse((short) ResultCode.InteractorIsDead);

			// does the npc have the quest that we are trying to accept
			if (!interactor.HaveStartableQuest(operation.QuestId))
				return operation.GetErrorResponse((short) ResultCode.NoQuestAvailable);

			// add the quest
			player.AddQuest(operation.QuestId, interactor);

			// if we can complete the quest complete it
			if (player.CanCompleteQuest(operation.QuestId))
				player.CompleteQuest(operation.QuestId);
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationTurnInQuest(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new TurnInQuest(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoTurnInQuest(operation), parameters));
			return null;
		}

		private GameOperationResponse DoTurnInQuest(TurnInQuest operation)
		{
			var interactor = player.CurrentInteractor;
			if (interactor == null)
				return operation.GetErrorResponse((short) ResultCode.InteractorNotFound);

			// for now only npcs can have quests
			if (!interactor.IsNpc())
				return operation.GetErrorResponse((short) ResultCode.ActionNotAvailable);

			// for now we cannot interact with dead npcs
			var npc = (Npc) interactor;
			if (npc.IsDead())
				return operation.GetErrorResponse((short) ResultCode.InteractorIsDead);

			if (!interactor.HaveCompletableQuest(operation.QuestId))
				return operation.GetErrorResponse((short) ResultCode.NoQuestAvailable);

			player.FinishQuest(operation.QuestId, interactor);
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationUseInventorySlot(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new UseInventorySlot(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoUseInventorySlot(operation), parameters));
			return null;
		}

		private GameOperationResponse DoUseInventorySlot(UseInventorySlot operation)
		{
			IMmoItem item;
			// is the item in our quest inventory
			// remember all general items are stored inside the player's general inventory
			if (!player.Inventory.TryGetItem(operation.Index, out item))
				return operation.GetErrorResponse((short) ResultCode.ItemNotFound);

			// if the item cannot be used report the error
			if (!item.IsUsable)
				return operation.GetErrorResponse((short) ResultCode.CannotUseItem);

			MmoObject target = this.player;
			if (operation.ObjectId.HasValue)
				if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out target))
					return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			// cast the item use spell
			var castResult = player.SpellManager.CastSpell(item.SpellId, target);
			switch (castResult)
			{
				case CastResults.Ok:
					// if the use was successfull and we can only use the item once remove one from the inventory
					if(item.UseLimit == UseLimit.Once)
						player.Inventory.RemoveItemAt(operation.Index, 1);
					break;

				case CastResults.SpellNotReady:
				case CastResults.SpellNotFound:
					return operation.GetErrorResponse((short) ResultCode.ItemUseFailed);

				default:
					return operation.GetErrorResponse((short) castResult.ToResultCode());
			}

			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationMoveInventorySlot(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new MoveInventorySlot(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoMoveInventorySlot(operation), parameters));
			return null;
		}

		private GameOperationResponse DoMoveInventorySlot(MoveInventorySlot operation)
		{
			// if the move is valid dont do anything
			if (player.Inventory.MoveItem(operation.IndexTo, operation.IndexFrom))
				return null;

			// if the move was not valid, correct the move in the client side
			var generalItemMoved = new InventoryItemMoved
				{
					IndexFrom = operation.IndexFrom,
					IndexTo = operation.IndexTo
				};
			this.SendEvent(generalItemMoved, new MessageParameters {ChannelId = PeerSettings.InventoryEventChannel});
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationAddActionbarItem(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new AddActionbarItem(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoAddActionbarItem(operation), parameters));
			return null;
		}

		private GameOperationResponse DoAddActionbarItem(AddActionbarItem operation)
		{
			// does the player even have the item to begin with
			if (!player.Inventory.HasItem(operation.ItemId))
				return operation.GetErrorResponse((byte) ResultCode.ItemNotFound);
			// if we successfully added the item we are good and exit
			// NOTE:: We do not have to notify the client of the addition since slot addition are predicted at the client stage and the addition
			// should have been already made at the client
			if (player.Actionbar.AddItemAt(ActionItemType.Item, operation.ItemId, operation.Index))
				return null;
			// if the addition was unsuccessful, then we have to reverse the addition client side
            // TODO: Finish the bottom
			//var actionbarSlotRemoved = new ActionbarSlotRemoved{Index = }
			return null;
		}
		
		// synchronous operation
		private GameOperationResponse HandleOperationAddActionbarSpell(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new AddActionbarSpell(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoAddActionbarSpell(operation), parameters));
			return null;
		}

		private GameOperationResponse DoAddActionbarSpell(AddActionbarSpell operation)
		{
			// TODO: To be implemented
			// if the add wasnt valid remove the item from the client
			// the client basically predicts that a valid add was valid and has already made the addition
			// if the add was valid we dont need to let the client know otherwise we need the client to reverse (remove) the add
			//if (!actionbar.AddItemAt(operation.ItemId.Value, operation.Index))
			//{
			//	var containerEvent = new ContainerEvent
			//	{
			//		ContainerEventType = (byte)ContainerEventType.RemoveItem,
			//		ContainerType = (byte)ContainerType.ActionBar,
			//		Index = operation.Index
			//	};

			//	this.SendEventContainerEvent(containerEvent);
			//}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationUseActionbarSlot(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new UseActionbarSlot(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode) { ReturnCode = (short)ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoUseActionbarSlot(operation), parameters));
			return null;
		}

		private GameOperationResponse DoUseActionbarSlot(UseActionbarSlot operation)
		{
			Actionbar.ActionItem actionItem;
			if (!player.Actionbar.TryGetItem(operation.Index, out actionItem))
				return operation.GetErrorResponse((short) ResultCode.ItemNotFound);

			MmoObject target = this.player;
			if (operation.ObjectId.HasValue)
				if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out target))
					return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			switch (actionItem.Type)
			{
				case ActionItemType.Spell:
					// cast the spell
					var castResult = player.SpellManager.CastSpell(actionItem.ItemId, target);
					// send the result if there is an error
					if (castResult != CastResults.Ok)
						return operation.GetErrorResponse((short) castResult.ToResultCode());
					break;

				case ActionItemType.Item:
					// TODO: Handle item usage
					break;
			}

			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationCastSpell(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new CastSpell(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoCastSpell(operation), parameters));
			return null;
		}

		private GameOperationResponse DoCastSpell(CastSpell operation)
		{
			MmoObject target = this.player;
			if (operation.ObjectId.HasValue)
				if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out target))
					return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			// cast the spell
			var castResult = player.SpellManager.CastSpell(operation.SpellId, target);
			// send the result if there is an error
			return castResult != CastResults.Ok ? operation.GetErrorResponse((short) castResult.ToResultCode()) : null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpDamage(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpDamage(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpDamage(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpDamage(SpDamage operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			if (operation.Value < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					var gc = mmoObject as Character;
					if (gc != null) gc.Damage(player, operation.Value, null);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpHeal(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpHeal(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpHeal(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpHeal(SpHeal operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			if (operation.Value < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					var gc = mmoObject as Character;
					if (gc != null) gc.Heal(player, operation.Value, null);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpAddPower(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpAddPower(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpAddPower(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpAddPower(SpAddPower operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			if (operation.Value < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					var gc = mmoObject as Character;
					if (gc != null) gc.GainPower(player, operation.Value, null);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpRemovePower(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpRemovePower(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpRemovePower(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpRemovePower(SpRemovePower operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			if (operation.Value < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					var gc = mmoObject as Character;
					if (gc != null) gc.DrainPower(player, operation.Value, null);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpKill(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpKill(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpKill(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpKill(SpKill operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Npc:
				case ObjectType.Player:
					var gc = mmoObject as Character;
					if (gc != null) gc.Kill(player);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpAddXp(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpAddXp(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpAddXp(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpAddXp(SpAddXp operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			if (operation.Value < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Player:
					var actor = mmoObject as Player;
					if (actor != null) actor.GainXp(operation.Value);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpAddSpell(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpAddSpell(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpAddSpell(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpAddSpell(SpAddSpell operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Player:
					var actor = mmoObject as Player;
					if (actor != null) actor.SpellManager.AddSpell(operation.SpellId);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpRemoveSpell(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpRemoveSpell(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpRemoveSpell(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpRemoveSpell(SpRemoveSpell operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Player:
					var actor = mmoObject as Player;
					if (actor != null) actor.SpellManager.RemoveSpell(operation.SpellId);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpAddGold(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpAddGold(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpAddGold(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpAddGold(SpAddGold operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			if (operation.Value < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Player:
					var actor = mmoObject as Player;
					if (actor != null) actor.GainGold(operation.Value, true);
					break;
			}
			return null;
		}

		// synchronous operation
		private GameOperationResponse HandleOperationSpRemoveGold(GameOperationRequest operationRequest, MessageParameters parameters)
		{
			var operation = new SpRemoveGold(server.Protocol, operationRequest);
			if (!operation.IsValid)
				return new GameErrorResponse(operationRequest.OperationCode)
					{ReturnCode = (short) ResultCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage()};

			player.CurrentZone.PrimaryFiber.Enqueue(() => ExecItemOperation(() => this.DoSpRemoveGold(operation), parameters));
			return null;
		}

		private GameOperationResponse DoSpRemoveGold(SpRemoveGold operation)
		{
			MmoObject mmoObject = this.player;
			if (operation.ObjectId.HasValue)
				if (operation.ObjectId.Value != this.player.Guid)
					if (!player.CurrentZone.ObjectCache.TryGetItem(operation.ObjectId.Value, out mmoObject))
						return operation.GetErrorResponse((short) ResultCode.ObjectNotFound);

			if (operation.Value < 1)
				return operation.GetErrorResponse((short) ResultCode.InvalidOperationParameter);

			switch ((ObjectType) mmoObject.Guid.Type)
			{
				case ObjectType.Player:
					var actor = mmoObject as Player;
					if (actor != null) actor.DeductGold(operation.Value, true);
					break;
			}
			return null;
		}

		/// <summary>
		/// Call this to handle client operation requests
		/// </summary>
		private void OnOperationRequest(GameOperationRequest operationRequest, MessageParameters messageParameters)
		{
			// filters the operation operationRequest and updates the operation
			// this call is VERY IMPORTANT! since this ensures that operations are handled according
			// to the session state and permissions
			this.FilterOperationRequest(operationRequest);

			GameOperationResponse response;
			switch ((GameOperationCode) operationRequest.OperationCode)
			{
				case GameOperationCode.EnterWorld:
					response = this.HandleOperationEnterWorld();
					break;

				case GameOperationCode.LogoutUser:
					response = this.HandleOperationLogoutUser();
					break;

				case GameOperationCode.LogoutCharacter:
					response = this.HandleOperationLogoutCharacter();
					break;

				case GameOperationCode.Move:
					response = this.HandleOperationMove(operationRequest);
					break;

				case GameOperationCode.Rotate:
					response = this.HandleOperationRotate(operationRequest);
					break;

				case GameOperationCode.GetProperties:
					response = this.HandleOperationGetProperties(operationRequest);
					break;

				case GameOperationCode.Interact:
					response = this.HandleOperationInteract(operationRequest, messageParameters);
					break;

				case GameOperationCode.CloseInteraction:
					response = this.HandleOperationCloseInteraction(operationRequest, messageParameters);
					break;

				case GameOperationCode.PurchaseItem:
					response = this.HandleOperationPurchaseItem(operationRequest, messageParameters);
					break;

				case GameOperationCode.SellItem:
					response = this.HandleOperationSellItem(operationRequest, messageParameters);
					break;

				case GameOperationCode.LootItem:
					response = this.HandleOperationLootItem(operationRequest, messageParameters);
					break;

				case GameOperationCode.LootGold:
					response = this.HandleOperationLootGold(operationRequest, messageParameters);
					break;

				case GameOperationCode.LootAll:
					response = this.HandleOperationLootAll(operationRequest, messageParameters);
					break;

				case GameOperationCode.SelectDialogue:
					response = this.HandleOperationSelectConversationMenu(operationRequest, messageParameters);
					break;

				case GameOperationCode.AcceptQuest:
					response = this.HandleOperationAcceptQuest(operationRequest, messageParameters);
					break;

				case GameOperationCode.TurnInQuest:
					response = this.HandleOperationTurnInQuest(operationRequest, messageParameters);
					break;

				case GameOperationCode.UseInventorySlot:
					response = this.HandleOperationUseInventorySlot(operationRequest, messageParameters);
					break;

				case GameOperationCode.MoveInventorySlot:
					response = this.HandleOperationMoveInventorySlot(operationRequest, messageParameters);
					break;

				case GameOperationCode.AddActionbarItem:
					response = this.HandleOperationAddActionbarItem(operationRequest, messageParameters);
					break;

				case GameOperationCode.AddActionbarSpell:
					response = this.HandleOperationAddActionbarSpell(operationRequest, messageParameters);
					break;

				case GameOperationCode.UseActionbarSlot:
					response = this.HandleOperationUseActionbarSlot(operationRequest, messageParameters);
					break;

				case GameOperationCode.CastSpell:
					response = this.HandleOperationCastSpell(operationRequest, messageParameters);
					break;
#if GM_COMMANDS
				case GameOperationCode.SpDamage:
					response = this.HandleOperationSpDamage(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpHeal:
					response = this.HandleOperationSpHeal(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpAddPower:
					response = this.HandleOperationSpAddPower(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpRemovePower:
					response = this.HandleOperationSpRemovePower(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpKill:
					response = this.HandleOperationSpKill(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpAddXp:
					response = this.HandleOperationSpAddXp(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpAddSpell:
					response = this.HandleOperationSpAddSpell(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpRemoveSpell:
					response = this.HandleOperationSpRemoveSpell(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpAddGold:
					response = this.HandleOperationSpAddGold(operationRequest, messageParameters);
					break;

				case GameOperationCode.SpRemoveGold:
					response = this.HandleOperationSpRemoveGold(operationRequest, messageParameters);
					break;
#endif
				case GameOperationCode.InvalidOperation:
					response = new GameErrorResponse(operationRequest.OperationCode) {ReturnCode = (short) ResultCode.OperationNotAvailable};
					break;

				case GameOperationCode.IgnoreOperation:
					response = null;
					break;

				default:
					response = new GameErrorResponse(operationRequest.OperationCode) {ReturnCode = (short) ResultCode.OperationNotAvailable};
					break;
			}

			if (response != null)
				this.SendOperationResponse(response, messageParameters);
		}

		/// <summary>
		/// Filters a(n) <see cref="GameOperationRequest"/> and updates the operationRequest
		/// </summary>
		/// <param name="operationRequest"></param>
		/// <returns></returns>
		private void FilterOperationRequest(GameOperationRequest operationRequest)
		{
			switch (SessionState)
			{
				case WorldSessionState.Login:
					switch ((GameOperationCode) operationRequest.OperationCode)
					{
						case GameOperationCode.LogoutUser:
						case GameOperationCode.LogoutCharacter:
						case GameOperationCode.EnterWorld:
							break;

						default:
							operationRequest.OperationCode = (byte) GameOperationCode.InvalidOperation;
							break;
					}
					break;

				case WorldSessionState.EnterWorld:
					switch ((GameOperationCode) operationRequest.OperationCode)
					{
						case GameOperationCode.EnterWorld:
							operationRequest.OperationCode = (byte) GameOperationCode.InvalidOperation;
							break;

						case GameOperationCode.SpDamage:
						case GameOperationCode.SpHeal:
						case GameOperationCode.SpAddPower:
						case GameOperationCode.SpRemovePower:
						case GameOperationCode.SpKill:
						case GameOperationCode.SpAddXp:
						case GameOperationCode.SpAddSpell:
						case GameOperationCode.SpRemoveSpell:
						case GameOperationCode.SpAddGold:
						case GameOperationCode.SpRemoveGold:
							if (gmLevel < 1)
								operationRequest.OperationCode = (byte) GameOperationCode.IgnoreOperation;
							break;
					}
					break;

				default:
					operationRequest.OperationCode = (byte) GameOperationCode.InvalidOperation;
					break;
			}
		}

		#endregion

		#region Helper Methods

		private void ExecItemOperation(Func<GameOperationResponse> operation, MessageParameters parameters)
		{
			var response = operation();
			if (response != null)
				this.SendOperationResponse(response, parameters);
		}

		#endregion
	}
}
