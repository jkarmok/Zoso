using System;
using System.Linq;
using System.Collections.Generic;

using DigitalRune.Physics;
using DigitalRune.Geometry;
using DigitalRune.Geometry.Collisions;
using DigitalRune.Geometry.Shapes;
using DigitalRune.Mathematics.Algebra;

using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneTriggerVolume : ITriggerVolume
	{
		private readonly CollisionDomain collisionDomain;
		private readonly string nameOfTarget;

		private readonly List<RigidBody> triggeredRigidbodies = new List<RigidBody>();

		public CollisionObject CollisionObject { get; private set; }
		public GeometricObject GeometricObject { get; private set; }

		public Vector3F Position
		{
			get
			{
				return GeometricObject.Pose.Position;
			}

			set
			{
				var oldPose = GeometricObject.Pose;
				GeometricObject.Pose = new Pose(value, oldPose.Orientation);
				collisionDomain.Update(CollisionObject);
			}
		}

		public QuaternionF Orientation
		{
			get
			{
				return QuaternionF.CreateRotation(GeometricObject.Pose.Orientation);
			}

			set
			{
				var oldPose = GeometricObject.Pose;
				GeometricObject.Pose = new Pose(oldPose.Position, value);
				collisionDomain.Update(CollisionObject);
			}
		}

		public int CollisionGroup
		{
			get
			{
				return this.CollisionObject.CollisionGroup;
			}
			set
			{
				this.CollisionObject.CollisionGroup = value;
			}
		}

		public event Action<object> OnTriggerEnter;

		public RuneTriggerVolume(Simulation simulation, Shape shape, string nameOfTarget)
		{
			this.collisionDomain = simulation.CollisionDomain;
			this.nameOfTarget = nameOfTarget;

			this.GeometricObject = new GeometricObject(shape, new Pose(simulation.World.Aabb.Center));
			this.CollisionObject = new CollisionObject(GeometricObject) {Type = CollisionObjectType.Trigger};
			
			simulation.CollisionDomain.CollisionObjects.Add(CollisionObject);
		}

		#region Implementation of ITriggerVolume

		Vector3 ITriggerVolume.Position
		{
			get
			{
				return this.Position.ToGameVector();
			}
			set
			{
				this.Position = value.ToRuneVector();
			}
		}

		Quaternion ITriggerVolume.Orientation
		{
			get
			{
				return this.Orientation.ToGameQuaternion();
			}
			set
			{
				this.Orientation = value.ToRuneQuaternion();
			}
		}

		public void Update()
		{
			if (collisionDomain.HasContact(CollisionObject))
			{
				if (triggeredRigidbodies.Count != 0)
					triggeredRigidbodies.RemoveAll(rigidBody1 => !collisionDomain.HaveContact(CollisionObject, rigidBody1.CollisionObject));

				var characters = collisionDomain.GetContactObjects(CollisionObject)
					.Select(collisionObject => collisionObject.GeometricObject)
					.OfType<RigidBody>()
					.Where(rigidBody1 => rigidBody1.Name == nameOfTarget)
					.Except(triggeredRigidbodies);
				foreach (var character in characters)
				{
					triggeredRigidbodies.Add(character);
					OnTriggerEnter(character.UserData);
				}
			}
			else
			{
				if (triggeredRigidbodies.Count != 0)
					triggeredRigidbodies.Clear();
			}
		}
		
		#endregion
	}
}
