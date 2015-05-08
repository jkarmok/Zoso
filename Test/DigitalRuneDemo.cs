using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DigitalRune.Geometry.Collisions;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Physics;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Quantum.Geometry;
using Karen90MmoFramework.Quantum.Physics;
using Karen90MmoFramework.Server.Game.Physics;
using Karen90MmoFramework.Server.Game.Physics.Rune;
using MotionType = DigitalRune.Physics.MotionType;

namespace Karen90MmoTests
{
	public class DigitalRuneDemo : IDemo
	{
		public void Run()
		{
			var worldBounds = new Bounds() { Min =  new Vector3(0, -5000, 0), Max = new Vector3(2000, 5000, 1600)};
			var serialNumber = @"tgCcAQADXEY+784BLMKfsg8O0AEnACNQcmF2ZWVuIFB1dmFuYXNpbmdhbSMxIzEjTm9uQ29tbWVyY2lhbECDssOQzVFUBAMB/A9Wt80a70/+9vKHaeL+Sj8kTFNIChTkCbzgKeebsjGh6Gx5YhufqQqjlT4HHZLRuk8=";
			var physics = new RunePhysicsWorld(worldBounds, serialNumber);
			
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "map0.map");
			float[,] heights;
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var buffer = new byte[sizeof(int)];
				stream.Read(buffer, 0, buffer.Length);
				var c = BitConverter.ToInt32(buffer, 0);

				buffer = new byte[sizeof(int)];
				stream.Read(buffer, 0, buffer.Length);
				var r = BitConverter.ToInt32(buffer, 0);

				heights = new float[c, r];
				for (var z = 0; z < r; z++)
				{
					for (var x = 0; x < c; x++)
					{
						buffer = new byte[sizeof(float)];
						stream.Read(buffer, 0, sizeof(float));

						heights[x, z] = BitConverter.ToSingle(buffer, 0);
					}
				}
			}

			var heightFieldDescription = new HeightFieldDescription()
				{
					Heights = heights,
					Position = Vector3.Zero,
					WidthX = 2000,
					WidthZ = 1600,
				};
			physics.CreateHeightField(heightFieldDescription);
			var boxDescription = new BoxDescription()
				{
					Position = new Vector3(1, 100, 1),
					Rotation = Quaternion.Identity,
					Size = new Vector3(1, 1, 1)
				};
			physics.CreateWorldObject(boxDescription, CollisionHelper.AvatarObjectColliderDescription);
			
			var rnd = new Random();
			var characterControllerDescription = new CharacterControllerDescription()
				{
					Height = 2f,
					Radius = .5f,
					SkinWidth = .03f,
					SlopeLimit = 45,
					StepOffset = .3f,
					Position = new Vector3(200, 20, 200),
				};
			var controller = physics.CreateCharacterController(characterControllerDescription);
			controller.Position = new Vector3(rnd.Next(1, 1999), 100, rnd.Next(1, 1599));
			controller.LocalVelocity = new Vector3(0, 0, 5);

			var body = new RigidBody(new BoxShape(10, 10, 1))
				{
					MotionType = MotionType.Static,
				};
			body.CollisionObject.Type = CollisionObjectType.Trigger;
			
			Console.WriteLine("simulation stated...");
			try
			{
				var timer2 = Stopwatch.StartNew();
				timer2.Start();

				while (true)
				{
					Thread.Sleep(60);

					controller.LocalVelocity = new Vector3(0, 0, timer2.ElapsedMilliseconds / 60f);
					Console.WriteLine(controller.Position);
					timer2.Restart();
				}
			}
			finally
			{
				Console.WriteLine("simulation ended...");
			}
		}
	}
}