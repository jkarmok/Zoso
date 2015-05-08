//using System;
//using System.Diagnostics;
//using System.Threading;

//using Karen90MmoFramework.Server;

//namespace Karen90MmoTests
//{
//	public class NetworkSim : IDemo
//	{
//		public const int MMO_OBJECT_TYPES = 2;
//		public const int PLAYERS_COUNT = 100;
//		public const int NPC_COUNT = 300;
//		public const int PACKET_DELAY = 500;
//		public const int PACKET_CHANCE = 3;
//		public const int PACKET_PROCESSING_TIME = 20;
//		public const int PLAYER_UPDATE_TIME = 10;
//		public const int NPC_UPDATE_TIME = 5;
//		public const int SIM_UPDATE_INTERVAL = 50;

//		public static NetworkSim Instance
//		{
//			get
//			{
//				return _instance;
//			}
//		}

//		private static readonly NetworkSim _instance = new NetworkSim();

//		public readonly object SyncRoot = new object();
//		public StorageMap<MmoObject> Cache = new StorageMap<MmoObject>(1000);

//		public event Action OnProcessMessages;
//		private event Action<float> OnUpdate;

//		bool isRunning = false;
//		public void Run()
//		{
//			for (var i = 0; i < NPC_COUNT; i++)
//			{
//				var npc = new Npc();
//				Cache.AddItem(npc.Type, npc.Id, npc);
//				OnUpdate += npc.Update;
//			}

//			for (var i = 0; i < PLAYERS_COUNT; i++)
//			{
//				var player = new Player();
//				Cache.AddItem(player.Type, player.Id, player);
//				OnUpdate += player.Update;
//			}

//			var interationCount = 0;
//			var interationTotal = 0L;

//			var timer = Stopwatch.StartNew();
//			timer.Start();

//			Console.WriteLine(DateTime.Now.TimeOfDay + " => starting server...");

//			isRunning = true;
//			while (isRunning)
//			{
//				if(timer.ElapsedMilliseconds < SIM_UPDATE_INTERVAL)
//					continue;

//				var start = timer.ElapsedMilliseconds;
//				if (OnProcessMessages != null)
//					OnProcessMessages();

//				if (OnUpdate != null)
//					OnUpdate(timer.ElapsedMilliseconds / 1000f);

//				var end = timer.ElapsedMilliseconds;
//				interationCount++;
//				interationTotal += (end - start);

//				if(interationCount >= 1)
//				{
//					Console.WriteLine(DateTime.Now.TimeOfDay + " => average processing time: " + ((float)interationTotal / interationCount));
//					interationCount = 0;
//					interationTotal = 0L;
//				}

//				OnProcessMessages = null;
//				timer.Restart();
//			}
//		}

//		void Stop()
//		{
//			isRunning = false;
//		}
//	}

//	public abstract class MmoObject
//	{
//		private static int _counter;

//		public byte Type { get; private set; }
//		public int Id { get; private set; }

//		protected MmoObject(byte type)
//		{
//			Type = type;
//			Id = _counter++;
//		}

//		public virtual void Update(float delta)
//		{
//		}
//	}

//	class Player : MmoObject
//	{
//		public Player()
//			: base(0)
//		{
//			new Thread(o => new PacketHandler(this).Run()).Start();
//		}

//		public override void Update(float delta)
//		{
//			Thread.Sleep(NetworkSim.PLAYER_UPDATE_TIME);
//		}
//	}

//	class Npc : MmoObject
//	{
//		public Npc()
//			: base(1)
//		{
//		}

//		public override void Update(float delta)
//		{
//			Thread.Sleep(NetworkSim.NPC_UPDATE_TIME);
//		}
//	}

//	internal class PacketHandler
//	{
//		public static int Count = 0;

//		private readonly int id;
//		private readonly Player player;

//		public PacketHandler(Player player)
//		{
//			id = Count;
//			Interlocked.Increment(ref Count);

//			this.player = player;
//		}

//		private readonly Random rnd = new Random();
//		public void Run()
//		{
//			while (true)
//			{
//				if (rnd.Next() % NetworkSim.PACKET_CHANCE == 0)
//				{
//					lock (NetworkSim.Instance.SyncRoot)
//					{
//						NetworkSim.Instance.OnProcessMessages += InstanceOnOnProcessMessages;
//					}
//					continue;
//				}

//				Thread.Sleep(NetworkSim.PACKET_DELAY);
//			}
//		}

//		private void InstanceOnOnProcessMessages()
//		{
//			var npcId = rnd.Next() % NetworkSim.NPC_COUNT;
//			MmoObject mmoObject;
//			if (NetworkSim.Instance.Cache.TryGetItem(1, npcId, out mmoObject))
//			{
//				Thread.Sleep(NetworkSim.PACKET_PROCESSING_TIME);
//			}
//		}
//	}
//}
