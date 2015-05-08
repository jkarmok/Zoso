using System;
using Karen90MmoFramework.Quantum.Geometry;
using Karen90MmoFramework.Quantum.Physics;

namespace Karen90MmoFramework.Quantum
{
	public interface IPhysicsWorld
	{
		/// <summary>
		/// Updates the physics world.
		/// </summary>
		/// <param name="delta"></param>
		void Update(TimeSpan delta);

		/// <summary>
		/// Creates a height field object in the world
		/// </summary>
		/// <param name="heightFieldDescription"> </param>
		/// <returns></returns>
		IWorldObject CreateHeightField(HeightFieldDescription heightFieldDescription);

		/// <summary>
		/// Creates a box shape in the world
		/// </summary>
		/// <param name="boxDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		IWorldObject CreateWorldObject(BoxDescription boxDescription, ColliderDescription colliderDescription);

		/// <summary>
		/// Creates a sphere shape in the world
		/// </summary>
		/// <param name="sphereDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		IWorldObject CreateWorldObject(SphereDescription sphereDescription, ColliderDescription colliderDescription);

		/// <summary>
		/// Creates a capsule shape in the world
		/// </summary>
		/// <param name="capsuleDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		IWorldObject CreateWorldObject(CapsuleDescription capsuleDescription, ColliderDescription colliderDescription);

		/// <summary>
		/// Creates a cylinder shape in the world
		/// </summary>
		/// <param name="cylinderDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		IWorldObject CreateWorldObject(CylinderDescription cylinderDescription, ColliderDescription colliderDescription);
		
		/// <summary>
		/// Removes a particular rigidbody from the world
		/// </summary>
		/// <param name="worldObject"></param>
		void RemoveWorldObject(IWorldObject worldObject);

		/// <summary>
		/// Creates a trigger volume in the world
		/// </summary>
		/// <param name="triggerVolumeDescription"></param>
		/// <param name="onTriggerEnter"> </param>
		/// <returns></returns>
		ITriggerVolume CreateTriggerVolume(TriggerVolumeDescription triggerVolumeDescription, Action<object> onTriggerEnter);

		/// <summary>
		/// Removes a trigger volume from the world
		/// </summary>
		/// <param name="triggerVolume"></param>
		void RemoveTriggerVolume(ITriggerVolume triggerVolume);

		/// <summary>
		/// Creates a character controller in the world
		/// </summary>
		/// <param name="characterControllerDescription"> </param>
		/// <returns></returns>
		ICharacterController CreateCharacterController(CharacterControllerDescription characterControllerDescription);
		
		/// <summary>
		/// Removes a character controller from the world
		/// </summary>
		/// <param name="characterController"></param>
		void RemoveCharacterController(ICharacterController characterController);
	}
}
