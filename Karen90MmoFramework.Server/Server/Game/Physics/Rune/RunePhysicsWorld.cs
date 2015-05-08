using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DigitalRune.Physics;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Collisions;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Physics.ForceEffects;

using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Quantum.Geometry;
using Karen90MmoFramework.Quantum.Physics;

using MotionType = DigitalRune.Physics.MotionType;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RunePhysicsWorld : IPhysicsWorld
	{
		private const int CollisionGroupObject = CollisionHelper.COLLISION_GROUP_WORLD_OBJECT;
		private const int CollisionGroupAvatar = CollisionHelper.COLLISION_GROUP_AVATAR_OBJECT;

		private readonly Simulation simulation;
		private readonly Bounds bounds;

		private readonly List<RuneCharacterController> characterControllers = new List<RuneCharacterController>();
		private readonly List<RuneTriggerVolume> triggerVolumes = new List<RuneTriggerVolume>();

		/// <summary>
		/// Gets the world bounds
		/// </summary>
		public Bounds Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public Simulation Simulation
		{
			get
			{
				return this.simulation;
			}
		}

		/// <summary>
		/// Creates a new instance of the <see cref="RunePhysicsWorld"/> class.
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="serialNumber"></param>
		public RunePhysicsWorld(Bounds bounds, string serialNumber)
		{
			// adding license is important for digital rune
			DigitalRune.Licensing.AddSerialNumber(serialNumber);

			this.simulation = new Simulation();
			// creating the world space based on the bounds
			//var worldSize = bounds.Size;
			//this.simulation.World.Shape = new BoxShape(worldSize.X, worldSize.Y, worldSize.Z);
			// setting the world position based on the bounds
			//var worldMin = bounds.Min;
			//this.simulation.World.Pose = new Pose(new Vector3F(worldMin.X + worldSize.X * .5f, worldMin.Y + worldSize.Y * .5f, worldMin.Z + worldSize.Z * .5f));
			// setting up world forces
			// this will prevent any world forces from acting on the character controllers
			//var avatarExcludedAreaOfEffect = new GlobalAreaOfEffect { Exclude = body => body.CollisionObject.CollisionGroup == CollisionGroupAvatar };
			//this.simulation.ForceEffects.Add(new Gravity(avatarExcludedAreaOfEffect));
			//this.simulation.ForceEffects.Add(new Damping(avatarExcludedAreaOfEffect));
			
			// enabling multi-threading for collision
			this.simulation.Settings.EnableMultithreading = true;
			this.simulation.CollisionDomain.EnableMultithreading = true;
			
			// defining collision filters
			var broadPhaseCollisionFilter = new CollisionFilter(maxNumberOfGroups: 4);
			broadPhaseCollisionFilter.Set(CollisionGroupObject, CollisionGroupObject, false);
			broadPhaseCollisionFilter.Set(CollisionGroupObject, CollisionGroupAvatar, true);
			broadPhaseCollisionFilter.Set(CollisionGroupAvatar, CollisionGroupAvatar, false);
			this.simulation.CollisionDomain.BroadPhase.Filter = broadPhaseCollisionFilter;

			this.bounds = bounds;
		}

		/// <summary>
		/// Updates the physics world.
		/// </summary>
		/// <param name="delta"></param>
		public void Update(TimeSpan delta)
		{
			lock (this)
			{
				this.simulation.Update(delta);
				var elapsed = (float)(delta.TotalMilliseconds * 0.001);
				Parallel.ForEach(characterControllers, c => c.Update(elapsed));
				Parallel.ForEach(triggerVolumes, t => t.Update());
			}
		}

		/// <summary>
		/// Creates a height field object in the world
		/// </summary>
		/// <param name="heightFieldDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateHeightField(HeightFieldDescription heightFieldDescription)
		{
			lock (this)
			{
				var heightField = new RuneHeightField(heightFieldDescription.WidthX, heightFieldDescription.WidthZ, heightFieldDescription.Heights);
				var rigidBody = new RuneRigidbody(heightField)
					{
						Pose = new Pose(heightFieldDescription.Position.ToRuneVector(), QuaternionF.Identity),
					};
				rigidBody.CollisionObject.CollisionGroup = CollisionGroupObject;
				rigidBody.MotionType = MotionType.Static;
				this.simulation.RigidBodies.Add(rigidBody);

				return rigidBody;
			}
		}

		/// <summary>
		/// Creates a box shape in the world
		/// </summary>
		/// <param name="boxDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(BoxDescription boxDescription, ColliderDescription colliderDescription)
		{
			lock (this)
			{
				var size = boxDescription.Size;
				var rigidBody = new RuneRigidbody(new RuneBoxShape(size.X, size.Y, size.Z))
					{
						Pose = new Pose(boxDescription.Position.ToRuneVector(), boxDescription.Rotation.ToRuneQuaternion()),
						Scale = boxDescription.Scale.ToRuneVector(),
					};
				UpdateRigidbodyCollisionSettings(rigidBody, ref colliderDescription);
				this.simulation.RigidBodies.Add(rigidBody);

				return rigidBody;
			}
		}

		/// <summary>
		/// Creates a sphere shape in the world
		/// </summary>
		/// <param name="sphereDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(SphereDescription sphereDescription, ColliderDescription colliderDescription)
		{
			lock (this)
			{
				var rigidBody = new RuneRigidbody(new RuneSphereShape(sphereDescription.Radius))
					{
						Pose = new Pose(sphereDescription.Position.ToRuneVector(), sphereDescription.Rotation.ToRuneQuaternion()),
						Scale = sphereDescription.Scale.ToRuneVector(),
					};
				UpdateRigidbodyCollisionSettings(rigidBody, ref colliderDescription);
				this.simulation.RigidBodies.Add(rigidBody);

				return rigidBody;
			}
		}

		/// <summary>
		/// Creates a capsule shape in the world
		/// </summary>
		/// <param name="capsuleDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(CapsuleDescription capsuleDescription, ColliderDescription colliderDescription)
		{
			lock (this)
			{
				var rigidBody = new RuneRigidbody(new RuneCapsuleShape(capsuleDescription.Radius, capsuleDescription.Height))
					{
						Pose = new Pose(capsuleDescription.Position.ToRuneVector(), capsuleDescription.Rotation.ToRuneQuaternion()),
						Scale = capsuleDescription.Scale.ToRuneVector(),
					};
				UpdateRigidbodyCollisionSettings(rigidBody, ref colliderDescription);
				this.simulation.RigidBodies.Add(rigidBody);

				return rigidBody;
			}
		}

		/// <summary>
		/// Creates a cylinder shape in the world
		/// </summary>
		/// <param name="cylinderDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(CylinderDescription cylinderDescription, ColliderDescription colliderDescription)
		{
			lock (this)
			{
				var rigidBody = new RuneRigidbody(new RuneCapsuleShape(cylinderDescription.Radius, cylinderDescription.Length))
					{
						Pose = new Pose(cylinderDescription.Position.ToRuneVector(), cylinderDescription.Rotation.ToRuneQuaternion()),
						Scale = cylinderDescription.Scale.ToRuneVector(),
					};
				UpdateRigidbodyCollisionSettings(rigidBody, ref colliderDescription);
				this.simulation.RigidBodies.Add(rigidBody);

				return rigidBody;
			}
		}

		/// <summary>
		/// Removes a particular rigidbody from the world
		/// </summary>
		/// <param name="worldObject"></param>
		public void RemoveWorldObject(IWorldObject worldObject)
		{
			lock (this)
			{
				var runeRigidbody = worldObject as RuneRigidbody;
				if (runeRigidbody != null)
					this.simulation.RigidBodies.Remove(runeRigidbody);
			}
		}

		/// <summary>
		/// Creates a trigger volume in the world
		/// </summary>
		/// <param name="triggerVolumeDescription"></param>
		/// <param name="onTriggerEnter"> </param>
		/// <returns></returns>
		public ITriggerVolume CreateTriggerVolume(TriggerVolumeDescription triggerVolumeDescription, Action<object> onTriggerEnter)
		{
			lock (this)
			{
				var size = triggerVolumeDescription.Size;
				var runeTriggerVolume = new RuneTriggerVolume(simulation, new BoxShape(size.X, size.Y, size.Z), triggerVolumeDescription.NameOfTarget)
					{
						CollisionGroup = CollisionGroupObject,
					};
				runeTriggerVolume.OnTriggerEnter += onTriggerEnter;
				this.triggerVolumes.Add(runeTriggerVolume);
				return runeTriggerVolume;
			}
		}

		/// <summary>
		/// Removes a trigger volume from the world
		/// </summary>
		/// <param name="triggerVolume"></param>
		public void RemoveTriggerVolume(ITriggerVolume triggerVolume)
		{
			lock (this)
			{
				var runeTriggerVolume = triggerVolume as RuneTriggerVolume;
				if (runeTriggerVolume != null)
				{
					this.simulation.CollisionDomain.CollisionObjects.Remove(runeTriggerVolume.CollisionObject);
					this.triggerVolumes.Remove(runeTriggerVolume);
				}
			}
		}

		/// <summary>
		/// Creates a character controller in the world
		/// </summary>
		/// <param name="id"> </param>
		/// <param name="characterControllerDescription"> </param>
		/// <returns></returns>
		public ICharacterController CreateCharacterController(CharacterControllerDescription characterControllerDescription)
		{
			lock (this)
			{
				var characterController = new RuneCharacterController(simulation, characterControllerDescription.Name, characterControllerDescription.UserData)
					{
						Width = characterControllerDescription.Radius * 0.5f,
						Height = characterControllerDescription.Height,
						SlopeLimit = characterControllerDescription.SlopeLimit * Mathf.DEG2_RAD,
						StepHeight = characterControllerDescription.StepOffset,
						Position = characterControllerDescription.Position.ToRuneVector(),
						CollisionGroup = CollisionGroupAvatar,
						Enabled = true,
						// yes, this is positive
						Gravity = 9.81f,
					};
				this.characterControllers.Add(characterController);
				return characterController;
			}
		}

		/// <summary>
		/// Removes a character controller from the world
		/// </summary>
		/// <param name="characterController"></param>
		public void RemoveCharacterController(ICharacterController characterController)
		{
			lock (this)
			{
				var runeCharacterController = characterController as RuneCharacterController;
				if(runeCharacterController != null)
				{
					runeCharacterController.Enabled = false;
					simulation.CollisionDomain.CollisionObjects.Remove(runeCharacterController.Body.CollisionObject);
					this.characterControllers.Remove(runeCharacterController);
				}
			}
		}

		private static void UpdateRigidbodyCollisionSettings(RigidBody rigidBody, ref ColliderDescription colliderDescription)
		{
			rigidBody.CollisionObject.CollisionGroup = colliderDescription.CollisionGroup;
			switch (colliderDescription.MotionType)
			{
				case Karen90MmoFramework.Quantum.Physics.MotionType.Static:
					{
						rigidBody.MotionType = MotionType.Static;
						rigidBody.MassFrame = new MassFrame { Mass = 0 };
					}
					break;

				case Karen90MmoFramework.Quantum.Physics.MotionType.Kinematic:
					{
						rigidBody.MotionType = MotionType.Kinematic;
						rigidBody.MassFrame = new MassFrame { Mass = 0 };
					}
					break;

				case Karen90MmoFramework.Quantum.Physics.MotionType.Dynamic:
					{
						rigidBody.MotionType = MotionType.Dynamic;
						rigidBody.MassFrame = new MassFrame { Mass = 1 };
					}
					break;
			}
		}
	}
}
