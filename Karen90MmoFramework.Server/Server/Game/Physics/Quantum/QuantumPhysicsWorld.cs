using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Quantum.Geometry;
using Karen90MmoFramework.Quantum.Physics;

namespace Karen90MmoFramework.Server.Game.Physics.Quantum
{
	public class QuantumPhysicsWorld : IPhysicsWorld
	{
		private readonly MmoZone world;
		private readonly Bounds bounds;
		private readonly Vector3 gravity;

		private readonly List<QuantumCharacterController> characterControllers = new List<QuantumCharacterController>();

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

		/// <summary>
		/// Creates a new instance of the <see cref="QuantumPhysicsWorld"/> class.
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="world"></param>
		public QuantumPhysicsWorld(Bounds bounds, MmoZone world)
		{
			this.world = world;
			this.bounds = bounds;
			this.gravity = new Vector3(0, -9.81f, 0);
		}

		/// <summary>
		/// Updates the physics world.
		/// </summary>
		/// <param name="delta"></param>
		public void Update(TimeSpan delta)
		{
			lock (this)
			{
				var elapsed = (float)(delta.TotalMilliseconds * 0.001);
				Parallel.ForEach(characterControllers, c => c.Update(elapsed));
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
				var heightField = new QuantumHeightField(heightFieldDescription.WidthX, heightFieldDescription.WidthZ, heightFieldDescription.Heights, heightFieldDescription.Position);
				return new QuantumRigidbody(heightField)
					{
						Position = heightFieldDescription.Position,
						Rotation = Quaternion.Identity
					};
			}
		}

		/// <summary>
		/// Removes a height field object from the world
		/// </summary>
		/// <param name="terrain"></param>
		public void RemoveHeightField(IHeightField terrain)
		{
		}

		/// <summary>
		/// Creates a box shape in the world
		/// </summary>
		/// <param name="boxDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(BoxDescription boxDescription, ColliderDescription colliderDescription)
		{
			return null;
		}

		/// <summary>
		/// Creates a sphere shape in the world
		/// </summary>
		/// <param name="sphereDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(SphereDescription sphereDescription, ColliderDescription colliderDescription)
		{
			return null;
		}

		/// <summary>
		/// Creates a capsule shape in the world
		/// </summary>
		/// <param name="capsuleDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(CapsuleDescription capsuleDescription, ColliderDescription colliderDescription)
		{
			return null;
		}

		/// <summary>
		/// Creates a cylinder shape in the world
		/// </summary>
		/// <param name="cylinderDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IWorldObject CreateWorldObject(CylinderDescription cylinderDescription, ColliderDescription colliderDescription)
		{
			return null;
		}

		/// <summary>
		/// Removes a particular rigidbody from the world
		/// </summary>
		/// <param name="rigidbody"></param>
		public void RemoveWorldObject(IWorldObject rigidbody)
		{
		}

		/// <summary>
		/// Creates a trigger volume in the world
		/// </summary>
		/// <param name="triggerVolumeDescription"></param>
		/// <param name="onTriggerEnter"> </param>
		/// <returns></returns>
		public ITriggerVolume CreateTriggerVolume(TriggerVolumeDescription triggerVolumeDescription, Action<object> onTriggerEnter)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes a trigger volume from the world
		/// </summary>
		/// <param name="triggerVolume"></param>
		public void RemoveTriggerVolume(ITriggerVolume triggerVolume)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a character controller in the world
		/// </summary>
		/// <param name="characterControllerDescription"> </param>
		/// <returns></returns>
		public ICharacterController CreateCharacterController(CharacterControllerDescription characterControllerDescription)
		{
			lock (this)
			{
				var characterController = new QuantumCharacterController(world, characterControllerDescription.Position);
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
				var quantumCharacterController = characterController as QuantumCharacterController;
				if (quantumCharacterController != null)
					this.characterControllers.Remove(quantumCharacterController);
			}
		}
	}
}
