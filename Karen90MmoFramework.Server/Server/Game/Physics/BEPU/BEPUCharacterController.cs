using System;
using System.Collections.Generic;
using System.Threading;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.UpdateableSystems;
using BEPUphysics;
using BEPUutilities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Materials;
using BEPUphysics.PositionUpdating;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	/// <summary>
	/// Gives a physical object simple and cheap FPS-like control.
	/// This character has less features than the full CharacterController but is a little bit faster.
	/// </summary>
	public class BEPUCharacterController : Updateable, IBeforeSolverUpdateable, IBEPUCharacterController
	{
		#region IBEPUCharacterController Implementation

		public float Radius
		{
			get { return this.Body.Radius; }
		}

		public float Length
		{
			get { return this.Body.Length; }
		}

		public Karen90MmoFramework.Quantum.Vector3 Position
		{
			get
			{
				var p = this.Body.Position;
				return new Karen90MmoFramework.Quantum.Vector3(p.X, p.Y, p.Z);
			}

			set { this.TeleportToPosition(new Vector3(value.X, value.Y, value.Z), 0); }
		}

		public Karen90MmoFramework.Quantum.Quaternion Orientation
		{
			get
			{
				Quaternion quaternion;
				var forward = Vector3.Forward;
				var direction = this.viewDirection;
				Quaternion.GetQuaternionBetweenNormalizedVectors(ref forward, ref direction, out quaternion);
				return new Karen90MmoFramework.Quantum.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
			}

			set
			{
				var direction = Karen90MmoFramework.Quantum.Vector3.Transform(Karen90MmoFramework.Quantum.Vector3.Forward, value);
				this.ViewDirection = new Vector3(direction.X, direction.Y, direction.Z);
				//Utilities.Logger.DebugFormat("horizontaldirection={0} strafedirection={1}", HorizontalViewDirection, StrafeDirection);
			}
		}

		public Karen90MmoFramework.Quantum.Vector3 MovementDirection
		{
			get
			{
				var direction = this.HorizontalMotionConstraint.MovementDirection;
				return new Karen90MmoFramework.Quantum.Vector3(direction.X, 0, direction.Y);
			}

			set
			{
				this.HorizontalMotionConstraint.MovementDirection = new Vector2(value.X, value.Z);
			}
		}

		public float MovementeSpeed
		{
			get { return this.HorizontalMotionConstraint.Speed; }
			set { this.HorizontalMotionConstraint.Speed = value; }
		}

		public void SetMotion(Karen90MmoFramework.Quantum.Vector3 horizontalMoveDirection, float horizontalMoveSpeed)
		{
			this.MovementDirection = horizontalMoveDirection;
			this.MovementeSpeed = horizontalMoveSpeed;
		}

		public void SetMotion(Karen90MmoFramework.Quantum.Vector3 horizontalMoveDirection, float horizontalMoveSpeed, Karen90MmoFramework.Quantum.Quaternion rotation)
		{
			this.MovementDirection = horizontalMoveDirection;
			this.MovementeSpeed = horizontalMoveSpeed;
			this.Orientation = rotation;
		}

		public void Update(float delta)
		{
		}

		#endregion

		/// <summary>
		/// Gets the physical body of the character.
		/// </summary>
		public Capsule Body { get; private set; }

		/// <summary>
		/// Gets the support system which other systems use to perform local ray casts and contact queries.
		/// </summary>
		public QueryManager QueryManager { get; private set; }

		/// <summary>
		/// Gets the constraint used by the character to handle horizontal motion.  This includes acceleration due to player input and deceleration when the relative velocity
		/// between the support and the character exceeds specified maximums.
		/// </summary>
		public HorizontalMotionConstraint HorizontalMotionConstraint { get; private set; }

		/// <summary>
		/// Gets the constraint used by the character to stay glued to surfaces it stands on.
		/// </summary>
		public VerticalMotionConstraint VerticalMotionConstraint { get; private set; }

		private Vector3 down = new Vector3(0, -1, 0);

		/// <summary>
		/// Gets or sets the down direction of the character. Controls the interpretation of movement and support finding.
		/// </summary>
		public Vector3 Down
		{
			get { return this.down; }
		}

		private Vector3 viewDirection = new Vector3(0, 0, 1);
		private Vector3 horizontalViewDirection = new Vector3(0, 0, 1);

		/// <summary>
		/// Gets the horizontal view direction computed using the Down vector and the ViewDirection.
		/// </summary>
		public Vector3 HorizontalViewDirection
		{
			get { return this.horizontalViewDirection; }
		}

		/// <summary>
		/// Gets the axis along which the character can strafe.
		/// </summary>
		public Vector3 StrafeDirection
		{
			get { return Vector3.Cross(this.Down, this.horizontalViewDirection); }
		}

		/// <summary>
		/// Gets or sets the view direction associated with the character.
		/// Also sets the horizontal view direction internally based on the current down vector.
		/// This is used to interpret the movement directions.
		/// </summary>
		public Vector3 ViewDirection
		{
			get { return this.viewDirection; }
			set
			{
				this.viewDirection = value;
				this.UpdateHorizontalViewDirection();
			}
		}

		private void UpdateHorizontalViewDirection()
		{
			float dot = Vector3.Dot(this.viewDirection, this.Down);
			Vector3 toRemove = this.Down * dot;
			Vector3.Subtract(ref this.viewDirection, ref toRemove, out this.horizontalViewDirection);
			float length = this.horizontalViewDirection.LengthSquared();
			if (length > 0)
			{
				Vector3.Divide(ref this.horizontalViewDirection, (float) Math.Sqrt(length), out this.horizontalViewDirection);
			}
			else
				this.horizontalViewDirection = new Vector3();
		}

		private float jumpSpeed = 4.5f;

		/// <summary>
		/// Gets or sets the speed at which the character leaves the ground when it jumps.
		/// </summary>
		public float JumpSpeed
		{
			get { return this.jumpSpeed; }
			set
			{
				if (value < 0)
					throw new ArgumentException("Value must be nonnegative.");
				this.jumpSpeed = value;
			}
		}

		/// <summary>
		/// Gets the support finder used by the character.
		/// The support finder analyzes the character's contacts to see if any of them provide support and/or traction.
		/// </summary>
		public SupportFinder SupportFinder { get; private set; }

		/// <summary>
		/// Constructs a new character controller with the most common configuration options.
		/// </summary>
		/// <param name="position">Initial position of the character.</param>
		/// <param name="height"> </param>
		/// <param name="radius">Radius of the character body.</param>
		/// <param name="mass">Mass of the character body.</param>
		/// <param name="slopeInAngles"> </param>
		public BEPUCharacterController(Vector3 position, float height, float radius, float mass, float slopeInAngles)
		{
			this.Body = new Capsule(position, height, radius, mass);
			this.Body.IgnoreShapeChanges = true; //Wouldn't want inertia tensor recomputations to occur if the shape changes.
			//Making the character a continuous object prevents it from flying through walls which would be pretty jarring from a player's perspective.
			this.Body.PositionUpdateMode = PositionUpdateMode.Continuous;
			this.Body.LocalInertiaTensorInverse = new Matrix3x3();
			//TODO: In v0.16.2, compound bodies would override the material properties that get set in the CreatingPair event handler.
			//In a future version where this is changed, change this to conceptually minimally required CreatingPair.
			this.Body.CollisionInformation.Events.DetectingInitialCollision += this.RemoveFriction;
			this.Body.LinearDamping = 0;
			this.SupportFinder = new SupportFinder(this) { MaximumSlope = (float)(slopeInAngles * Karen90MmoFramework.Quantum.Mathf.DEG2_RAD) };
			this.HorizontalMotionConstraint = new HorizontalMotionConstraint(this);
			this.VerticalMotionConstraint = new VerticalMotionConstraint(this);
			this.QueryManager = new QueryManager(this);

			//Enable multithreading for the capsule characters.  
			//See the bottom of the Update method for more information about using multithreading with this character.
			this.IsUpdatedSequentially = false;

			//Link the character body to the character controller so that it can be identified by the locker.
			//Any object which replaces this must implement the ICharacterTag for locking to work properly.
			this.Body.CollisionInformation.Tag = new CharacterSynchronizer(this.Body);
		}

		private readonly List<ICharacterTag> involvedCharacters = new List<ICharacterTag>();

		public void LockCharacterPairs()
		{
			//If this character is colliding with another character, there's a significant danger of the characters
			//changing the same collision pair handlers.  Rather than infect every character system with micro-locks,
			//we lock the entirety of a character update.

			foreach (var pair in this.Body.CollisionInformation.Pairs)
			{
				//Is this a pair with another character?
				var other = pair.BroadPhaseOverlap.EntryA == this.Body.CollisionInformation
					            ? pair.BroadPhaseOverlap.EntryB
					            : pair.BroadPhaseOverlap.EntryA;
				var otherCharacter = other.Tag as ICharacterTag;
				if (otherCharacter != null)
				{
					this.involvedCharacters.Add(otherCharacter);
				}
			}
			if (this.involvedCharacters.Count > 0)
			{
				//If there were any other characters, we also need to lock ourselves!
				this.involvedCharacters.Add((ICharacterTag) this.Body.CollisionInformation.Tag);

				//However, the characters cannot be locked willy-nilly.  There needs to be some defined order in which pairs are locked to avoid deadlocking.
				this.involvedCharacters.Sort(_comparer);

				for (int i = 0; i < this.involvedCharacters.Count; ++i)
				{
					Monitor.Enter(this.involvedCharacters[i]);
				}
			}
		}

		private static readonly Comparer _comparer = new Comparer();

		private class Comparer : IComparer<ICharacterTag>
		{
			public int Compare(ICharacterTag x, ICharacterTag y)
			{
				if (x.InstanceId < y.InstanceId)
					return -1;
				if (x.InstanceId > y.InstanceId)
					return 1;
				return 0;
			}
		}

		public void UnlockCharacterPairs()
		{
			//Unlock the pairs, LIFO.
			for (int i = this.involvedCharacters.Count - 1; i >= 0; i--)
			{
				Monitor.Exit(this.involvedCharacters[i]);
			}
			this.involvedCharacters.Clear();
		}

		private void RemoveFriction(EntityCollidable sender, BroadPhaseEntry other, NarrowPhasePair pair)
		{
			var collidablePair = pair as CollidablePairHandler;
			if (collidablePair != null)
			{
				//The default values for InteractionProperties is all zeroes- zero friction, zero bounciness.
				//That's exactly how we want the character to behave when hitting objects.
				collidablePair.UpdateMaterialProperties(new InteractionProperties());
			}
		}

		private void ExpandBoundingBox()
		{
			if (this.Body.ActivityInformation.IsActive)
			{
				//This runs after the bounding box updater is run, but before the broad phase.
				//The expansion allows the downward pointing raycast to collect hit points.
				Vector3 expansion = this.SupportFinder.MaximumAssistedDownStepHeight * this.down;
				BoundingBox box = this.Body.CollisionInformation.BoundingBox;
				if (this.down.X < 0)
					box.Min.X += expansion.X;
				else
					box.Max.X += expansion.X;
				if (this.down.Y < 0)
					box.Min.Y += expansion.Y;
				else
					box.Max.Y += expansion.Y;
				if (this.down.Z < 0)
					box.Min.Z += expansion.Z;
				else
					box.Max.Z += expansion.Z;
				this.Body.CollisionInformation.BoundingBox = box;
			}
		}

		private void CollectSupportData()
		{
			//Identify supports.
			this.SupportFinder.UpdateSupports();

			//Collect the support data from the support, if any.
			if (this.SupportFinder.HasSupport)
				this.supportData = this.SupportFinder.HasTraction
					                   ? this.SupportFinder.TractionData.Value
					                   : this.SupportFinder.SupportData.Value;
			else
				this.supportData = new SupportData();
		}

		private SupportData supportData;

		void IBeforeSolverUpdateable.Update(float dt)
		{
			//We can't let multiple characters manage the same pairs simultaneously.  Lock it up!
			this.LockCharacterPairs();
			try
			{
				bool hadSupport = this.SupportFinder.HasSupport;

				this.CollectSupportData();

				//Compute the initial velocities relative to the support.
				var relativeVelocity = this.Body.LinearVelocity;
				var verticalVelocity = Vector3.Dot(this.supportData.Normal, relativeVelocity);

				//Don't attempt to use an object as support if we are flying away from it (and we were never standing on it to begin with).
				if (this.SupportFinder.HasSupport && !hadSupport && verticalVelocity < 0)
				{
					this.SupportFinder.ClearSupportData();
					this.supportData = new SupportData();
				}

				//Attempt to jump.
				if (this.tryToJump) //Jumping while crouching would be a bit silly.
				{
					//In the following, note that the jumping velocity changes are computed such that the separating velocity is specifically achieved,
					//rather than just adding some speed along an arbitrary direction.  This avoids some cases where the character could otherwise increase
					//the jump speed, which may not be desired.
					if (this.SupportFinder.HasTraction)
					{
						//The character has traction, so jump straight up.
						float currentDownVelocity;
						Vector3.Dot(ref this.down, ref relativeVelocity, out currentDownVelocity);
						//Target velocity is JumpSpeed.
						float velocityChange = Math.Max(this.jumpSpeed + currentDownVelocity, 0);
						this.ApplyJumpVelocity(this.down * -velocityChange, ref relativeVelocity);

						//Prevent any old contacts from hanging around and coming back with a negative depth.
						foreach (var pair in this.Body.CollisionInformation.Pairs)
							pair.ClearContacts();
						this.SupportFinder.ClearSupportData();
						this.supportData = new SupportData();
					}
					else
					{
						Utilities.Logger.Error("Support Does not have traction");
					}
				}
				this.tryToJump = false;
			}
			finally
			{
				this.UnlockCharacterPairs();
			}

			//Vertical support data is different because it has the capacity to stop the character from moving unless
			//contacts are pruned appropriately.
			Vector3 movement3D;
			this.HorizontalMotionConstraint.GetMovementDirectionIn3D(out movement3D);
			this.HorizontalMotionConstraint.SupportData = this.supportData;

			SupportData verticalSupportData;
			this.SupportFinder.GetTractionInDirection(ref movement3D, out verticalSupportData);
			this.VerticalMotionConstraint.SupportData = verticalSupportData;
		}

		private void TeleportToPosition(Vector3 newPosition, float dt)
		{
			this.Body.Position = newPosition;
			var orientation = this.Body.Orientation;
			//The re-do of contacts won't do anything unless we update the collidable's world transform.
			this.Body.CollisionInformation.UpdateWorldTransform(ref newPosition, ref orientation);
			//Refresh all the narrow phase collisions.
			foreach (var pair in this.Body.CollisionInformation.Pairs)
			{
				//Clear out the old contacts.  This prevents contacts in persistent manifolds from surviving the step
				//Such old contacts might still have old normals which blocked the character's forward motion.
				pair.ClearContacts();
				pair.UpdateCollision(dt);
			}
			//Also re-collect supports.
			//This will ensure the constraint and other velocity affectors have the most recent information available.
			this.CollectSupportData();
		}

		/// <summary>
		/// Changes the relative velocity between the character and its support.
		/// </summary>
		/// <param name="velocityChange">Change to apply to the character and support relative velocity.</param>
		/// <param name="relativeVelocity">Relative velocity to update.</param>
		private void ApplyJumpVelocity(Vector3 velocityChange, ref Vector3 relativeVelocity)
		{
			this.Body.LinearVelocity += velocityChange;
			//Update the relative velocity as well.  It's a ref parameter, so this update will be reflected in the calling scope.
			Vector3.Add(ref relativeVelocity, ref velocityChange, out relativeVelocity);
		}

		private bool tryToJump;

		/// <summary>
		/// Jumps the character off of whatever it's currently standing on.  If it has traction, it will go straight up.
		/// If it doesn't have traction, but is still supported by something, it will jump in the direction of the surface normal.
		/// </summary>
		public void Jump()
		{
			//The actual jump velocities are applied next frame.  This ensures that gravity doesn't pre-emptively slow the jump, and uses more
			//up-to-date support data.
			this.tryToJump = true;
		}

		public override void OnAdditionToSpace(Space newSpace)
		{
			//Add any supplements to the space too.
			newSpace.Add(this.Body);
			newSpace.Add(this.HorizontalMotionConstraint);
			newSpace.Add(this.VerticalMotionConstraint);
			//This character controller requires the standard implementation of Space.
			newSpace.BoundingBoxUpdater.Finishing += this.ExpandBoundingBox;

			this.Body.AngularVelocity = new Vector3();
			this.Body.LinearVelocity = new Vector3();
		}

		public override void OnRemovalFromSpace(Space oldSpace)
		{
			//Remove any supplements from the space too.
			oldSpace.Remove(this.Body);
			oldSpace.Remove(this.HorizontalMotionConstraint);
			oldSpace.Remove(this.VerticalMotionConstraint);
			//This character controller requires the standard implementation of Space.
			oldSpace.BoundingBoxUpdater.Finishing -= this.ExpandBoundingBox;
			this.SupportFinder.ClearSupportData();
			this.Body.AngularVelocity = new Vector3();
			this.Body.LinearVelocity = new Vector3();
		}
	}
}

