using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.Threading;
using BEPUutilities;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Quantum.Geometry;
using Karen90MmoFramework.Quantum.Physics;
using Box = Karen90MmoFramework.Quantum.Geometry.BoxDescription;
using Capsule = Karen90MmoFramework.Quantum.Geometry.CapsuleDescription;
using Cylinder = Karen90MmoFramework.Quantum.Geometry.CylinderDescription;
using Space = BEPUphysics.Space;
using Sphere = Karen90MmoFramework.Quantum.Geometry.SphereDescription;
using Vector3 = BEPUutilities.Vector3;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	public class BEPUPhysicsWorld : IPhysicsWorld
	{
		private readonly Space space;
		private readonly Karen90MmoFramework.Quantum.Bounds bounds;

		private readonly CollisionGroup staticGroup;
		private readonly CollisionGroup kinematicGroup;
		private readonly CollisionGroup dynamicGroup;
		private readonly CollisionGroup avatarGroup;

		private readonly Material frictionlessMaterial;

		private List<Triangle> boundaryShapes;
		private bool hasBoundary;

		public BEPUPhysicsWorld(Karen90MmoFramework.Quantum.Bounds bounds, Karen90MmoFramework.Quantum.Vector3 gravity, bool createBoundary = false)
		{
			var parallelLooper = new ParallelLooper();
			if(Environment.ProcessorCount > 1)
				for (var i = 0; i < Environment.ProcessorCount; i++)
					parallelLooper.AddThread();
			
			this.space = new Space(parallelLooper) {ForceUpdater = {Gravity = new Vector3(gravity.X, gravity.Y, gravity.Z)}};
			this.bounds = bounds;

			this.frictionlessMaterial = new Material(0f, 0f, 0f);

			this.staticGroup = new CollisionGroup();
			this.kinematicGroup = new CollisionGroup();
			this.dynamicGroup = new CollisionGroup();
			this.avatarGroup = new CollisionGroup();

			CollisionGroup.DefineCollisionRule(this.staticGroup, this.staticGroup, CollisionRule.NoBroadPhase);
			CollisionGroup.DefineCollisionRule(this.staticGroup, this.kinematicGroup, CollisionRule.NoBroadPhase);
			CollisionGroup.DefineCollisionRule(this.staticGroup, this.dynamicGroup, CollisionRule.NoBroadPhase);
			CollisionGroup.DefineCollisionRule(this.staticGroup, this.avatarGroup, CollisionRule.Defer);
			CollisionGroup.DefineCollisionRule(this.kinematicGroup, this.kinematicGroup, CollisionRule.NoBroadPhase);
			CollisionGroup.DefineCollisionRule(this.kinematicGroup, this.dynamicGroup, CollisionRule.NoBroadPhase);
			CollisionGroup.DefineCollisionRule(this.kinematicGroup, this.avatarGroup, CollisionRule.Defer);
			CollisionGroup.DefineCollisionRule(this.dynamicGroup, this.dynamicGroup, CollisionRule.NoBroadPhase);
			CollisionGroup.DefineCollisionRule(this.dynamicGroup, this.avatarGroup, CollisionRule.Defer);
			CollisionGroup.DefineCollisionRule(this.avatarGroup, this.avatarGroup, CollisionRule.NoBroadPhase);
			
			if(createBoundary)
				this.CreateWorldBoundary();
		}

		public void Update(TimeSpan delta)
		{
			this.space.Update(delta.Milliseconds * 0.001f);
		}

		/// <summary>
		/// Creates a height field object in the world
		/// </summary>
		/// <param name="heightFieldDescription"> </param>
		/// <returns></returns>
		public IHeightField CreateHeightField(HeightFieldDescription heightFieldDescription)
		{
			var position = heightFieldDescription.Position;
			var terrain = new Terrain(heightFieldDescription.Heights,
			                          new AffineTransform(new Vector3(position.X, position.Y, position.Z)))
				              {
					              CollisionRules = {Group = this.staticGroup},
					              Material = this.frictionlessMaterial,
					              Thickness = 10,
				              };
			//var terrain = new BEPUTerrain(heights, position) {CollisionRules = {Group = this.terrainGroup}};
			this.space.Add(terrain);

			return null;
		}

		/// <summary>
		/// Removes a height field object from the world
		/// </summary>
		/// <param name="terrain"></param>
		public void RemoveHeightField(IHeightField terrain)
		{
			var bepuTerrain = terrain as IBEPUHeightField;
			if (bepuTerrain != null)
				this.space.Remove(bepuTerrain);
		}

		/// <summary>
		/// Creates a box shape in the world
		/// </summary>
		/// <param name="boxDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IRigidbody CreateBox(Box boxDescription, ColliderDescription colliderDescription)
		{
			var position = Karen90MmoFramework.Quantum.Vector3.Clamp(boxDescription.Position, bounds.Min, bounds.Max);
			var bepuBox = new BEPUBoxShape(boxDescription.Size, position);
			bepuBox.CollisionInformation.CollisionRules.Group = GetCollisionGroup(colliderDescription.CollisionGroup);
			bepuBox.Material = this.frictionlessMaterial;
			bepuBox.Mass = (colliderDescription.MotionType == MotionType.Static || colliderDescription.MotionType == MotionType.Kinematic) ? 0 : 1;
			this.space.Add(bepuBox);

			return bepuBox;
		}

		/// <summary>
		/// Creates a sphere shape in the world
		/// </summary>
		/// <param name="sphereDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IRigidbody CreateSphere(Sphere sphereDescription, ColliderDescription colliderDescription)
		{
			var position = Karen90MmoFramework.Quantum.Vector3.Clamp(sphereDescription.Position, bounds.Min, bounds.Max);
			var bepuSphere = new BEPUSphereShape(sphereDescription.Radius, position);
			bepuSphere.CollisionInformation.CollisionRules.Group = GetCollisionGroup(colliderDescription.CollisionGroup);
			bepuSphere.Material = this.frictionlessMaterial;
			bepuSphere.Mass = (colliderDescription.MotionType == MotionType.Static || colliderDescription.MotionType == MotionType.Kinematic) ? 0 : 1;
			this.space.Add(bepuSphere);

			return bepuSphere;
		}

		/// <summary>
		/// Creates a capsule shape in the world
		/// </summary>
		/// <param name="capsuleDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IRigidbody CreateCapsule(Capsule capsuleDescription, ColliderDescription colliderDescription)
		{
			var position = Karen90MmoFramework.Quantum.Vector3.Clamp(capsuleDescription.Position, bounds.Min, bounds.Max);
			var bepuCapsule = new BEPUCapsuleShape(capsuleDescription.Radius, capsuleDescription.Height, position);
			bepuCapsule.CollisionInformation.CollisionRules.Group = GetCollisionGroup(colliderDescription.CollisionGroup);
			bepuCapsule.Material = this.frictionlessMaterial;
			bepuCapsule.Mass = (colliderDescription.MotionType == MotionType.Static || colliderDescription.MotionType == MotionType.Kinematic) ? 0 : 1;
			this.space.Add(bepuCapsule);

			return bepuCapsule;
		}

		/// <summary>
		/// Creates a cylinder shape in the world
		/// </summary>
		/// <param name="cylinderDescription"></param>
		/// <param name="colliderDescription"> </param>
		/// <returns></returns>
		public IRigidbody CreateCylinder(Cylinder cylinderDescription, ColliderDescription colliderDescription)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Removes a particular rigidbody from the world
		/// </summary>
		/// <param name="rigidbody"></param>
		public void RemoveRigidbody(IRigidbody rigidbody)
		{
			var bepuShape = rigidbody.Shape as IBEPUShape;
			if (bepuShape != null)
				this.space.Remove(bepuShape);
		}

		/// <summary>
		/// Creates a character controller in the world
		/// </summary>
		/// <param name="characterControllerDescription"> </param>
		/// <returns></returns>
		public ICharacterController CreateCharacterController(CharacterControllerDescription characterControllerDescription)
		{
			var position = characterControllerDescription.Position;
			var correctedPosition = new Vector3(position.X, position.Y, position.Z);
			var bepuCharacterController = new BEPUCharacterController(correctedPosition,
			                                                          characterControllerDescription.Height -
			                                                          characterControllerDescription.Radius * 2f,
			                                                          characterControllerDescription.Radius, 1,
			                                                          characterControllerDescription.SlopeLimit);
			bepuCharacterController.Body.CollisionInformation.CollisionRules.Group = GetCollisionGroup(CollisionHelper.AvatarObjectColliderDescription.CollisionGroup);
			this.space.Add(bepuCharacterController);

			return bepuCharacterController;
		}

		/// <summary>
		/// Removes a character controller from the world
		/// </summary>
		/// <param name="characterController"></param>
		public void RemoveCharacterController(ICharacterController characterController)
		{
			var bepuCharacterController = characterController as IBEPUCharacterController;
			if(bepuCharacterController != null)
				this.space.Remove(bepuCharacterController);
		}

		private void CreateWorldBoundary()
		{
			if(hasBoundary && boundaryShapes != null && boundaryShapes.Count > 0)
			{
				foreach (var boundaryShape in boundaryShapes)
					space.Remove(boundaryShape);
				this.hasBoundary = false;
			}

			if (!hasBoundary)
			{
				if(boundaryShapes == null)
					boundaryShapes = new List<Triangle>();

				// TODO: Create bounds

				this.hasBoundary = true;
			}
		}

		private CollisionGroup GetCollisionGroup(int group)
		{
			switch (group)
			{
				case CollisionHelper.COLLISION_GROUP_STATIC_OBJECT:
					return this.staticGroup;

				case CollisionHelper.COLLISION_GROUP_KINEMATIC_OBJECT:
					return this.kinematicGroup;

				case CollisionHelper.COLLISION_GROUP_DYNAMIC_OBJECT:
					return this.dynamicGroup;

				case CollisionHelper.COLLISION_GROUP_AVATAR_OBJECT:
					return this.avatarGroup;

				default:
					return this.staticGroup;
			}
		}
	}
}
