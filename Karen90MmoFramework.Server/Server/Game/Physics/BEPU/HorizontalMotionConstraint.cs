using System;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using BEPUutilities;
using BEPUutilities.DataStructures;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	/// <summary>
	/// Manages the horizontal movement of a character.
	/// </summary>
	public class HorizontalMotionConstraint : SolverUpdateable
	{
		private readonly BEPUCharacterController character;

		private SupportData supportData;

		/// <summary>
		/// Gets or sets the support data used by the constraint.
		/// </summary>
		public SupportData SupportData
		{
			get { return this.supportData; }
			set
			{
				//If the support changes, perform the necessary bookkeeping to keep the connections up to date.
				var oldSupport = this.supportData.SupportObject;
				this.supportData = value;
				if (oldSupport != this.supportData.SupportObject)
				{
					this.OnInvolvedEntitiesChanged();
					var supportEntityCollidable = this.supportData.SupportObject as EntityCollidable;
					// if we aren't on an entity clear out the support entity.
					this.supportEntity = supportEntityCollidable != null ? supportEntityCollidable.Entity : null;
				}
			}
		}

		private Vector2 movementDirection;

		/// <summary>
		/// Gets or sets the goal movement direction.
		/// The movement direction is based on the view direction.
		/// Values of X are applied to the axis perpendicular to the HorizontalViewDirection and Down direction.
		/// Values of Y are applied to the HorizontalViewDirection.
		/// </summary>
		public Vector2 MovementDirection
		{
			get { return this.movementDirection; }
			set
			{
				float lengthSquared = value.LengthSquared();
				if (lengthSquared > Toolbox.Epsilon)
				{
					this.character.Body.ActivityInformation.Activate();
					Vector2.Divide(ref value, (float) Math.Sqrt(lengthSquared), out this.movementDirection);
				}
				else
				{
					this.character.Body.ActivityInformation.Activate();
					this.movementDirection = new Vector2();
				}
			}
		}

		private float speed = 8f;

		/// <summary>
		/// Gets or sets the maximum speed at which the character can move while standing with a support that provides traction.
		/// Relative velocities with a greater magnitude will be decelerated.
		/// </summary>
		public float Speed
		{
			get { return this.speed; }
			set
			{
				if (value < 0)
					throw new ArgumentException("Value must be nonnegative.");
				this.speed = value;
			}
		}

		private float airSpeed = 1;

		/// <summary>
		/// Gets or sets the maximum speed at which the character can move with no support.
		/// The character will not be decelerated while airborne.
		/// </summary>
		public float AirSpeed
		{
			get { return this.airSpeed; }
			set
			{
				if (value < 0)
					throw new ArgumentException("Value must be nonnegative.");
				this.airSpeed = value;
			}
		}

		private float maximumForce = 1000;

		/// <summary>
		/// Gets or sets the maximum force that the character can apply while on a support which provides traction.
		/// </summary>
		public float MaximumForce
		{
			get { return this.maximumForce; }
			set
			{
				if (value < 0)
					throw new ArgumentException("Value must be nonnegative.");
				this.maximumForce = value;
			}
		}

		private float maximumAirForce = 250;

		/// <summary>
		/// Gets or sets the maximum force that the character can apply with no support.
		/// </summary>
		public float MaximumAirForce
		{
			get { return this.maximumAirForce; }
			set
			{
				if (value < 0)
					throw new ArgumentException("Value must be nonnegative.");
				this.maximumAirForce = value;
			}
		}

		/// <summary>
		/// Gets the movement mode that the character is currently in.
		/// </summary>
		public MovementMode MovementMode { get; private set; }

		private float maxSpeed;
		private float maxForce;

		private Matrix2x2 massMatrix;
		private Entity supportEntity;
		private Vector3 linearJacobianA1;
		private Vector3 linearJacobianA2;
		private Vector3 linearJacobianB1;
		private Vector3 linearJacobianB2;
		private Vector3 angularJacobianB1;
		private Vector3 angularJacobianB2;

		private Vector2 accumulatedImpulse;
		private Vector2 targetVelocity;

		private Vector2 positionCorrectionBias;

		private Vector3 positionLocalOffset;
		private bool wasTryingToMove;
		private bool hadTraction;
		private Entity previousSupportEntity;
		private float timeSinceTransition;

		/// <summary>
		/// Constructs a new horizontal motion constraint.
		/// </summary>
		/// <param name="characterController">Character to be governed by this constraint.</param>
		public HorizontalMotionConstraint(BEPUCharacterController characterController)
		{
			this.character = characterController;
			this.CollectInvolvedEntities();
		}

		internal void GetMovementDirectionIn3D(out Vector3 movement3D)
		{
			movement3D = this.character.HorizontalViewDirection * this.movementDirection.Y +
			             this.character.StrafeDirection * this.movementDirection.X;
		}

		protected override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
		{
			var entityCollidable = this.supportData.SupportObject as EntityCollidable;
			if (entityCollidable != null)
				outputInvolvedEntities.Add(entityCollidable.Entity);
			outputInvolvedEntities.Add(this.character.Body);
		}

		/// <summary>
		/// Computes per-frame information necessary for the constraint.
		/// </summary>
		/// <param name="dt">Time step duration.</param>
		public override void Update(float dt)
		{
			//Collect references, pick the mode, and configure the coefficients to be used by the solver.
			Vector3 movementDirectionIn3D;
			this.GetMovementDirectionIn3D(out movementDirectionIn3D);
			bool isTryingToMove = movementDirectionIn3D.LengthSquared() > 0;
			if (this.supportData.SupportObject != null)
			{
				this.MovementMode = MovementMode.Traction;
				this.maxSpeed = this.speed;
				this.maxForce = this.maximumForce;
			}
			else
			{
				this.MovementMode = MovementMode.Floating;
				this.maxSpeed = this.airSpeed;
				this.maxForce = this.maximumAirForce;
				this.supportEntity = null;
			}
			if (!isTryingToMove)
				this.maxSpeed = 0;

			this.maxForce *= dt;

			//Compute the jacobians.  This is basically a PointOnLineJoint with motorized degrees of freedom.
			Vector3 downDirection = this.character.Down;

			if (this.MovementMode != MovementMode.Floating)
			{
				//Compute the linear jacobians first.
				if (isTryingToMove)
				{
					Vector3 velocityDirection;
					Vector3 offVelocityDirection;
					//Project the movement direction onto the support plane defined by the support normal.
					//This projection is NOT along the support normal to the plane; that would cause the character to veer off course when moving on slopes.
					//Instead, project along the sweep direction to the plane.
					//For a 6DOF character controller, the lineStart would be different; it must be perpendicular to the local up.
					Vector3 lineStart = movementDirectionIn3D;
					Vector3 lineEnd;
					Vector3.Add(ref lineStart, ref downDirection, out lineEnd);
					Plane plane = new Plane(this.supportData.Normal, 0);
					float t;
					//This method can return false when the line is parallel to the plane, but previous tests and the slope limit guarantee that it won't happen.
					Toolbox.GetLinePlaneIntersection(ref lineStart, ref lineEnd, ref plane, out t, out velocityDirection);

					//The origin->intersection line direction defines the horizontal velocity direction in 3d space.
					velocityDirection.Normalize();

					//The normal and velocity direction are perpendicular and normal, so the off velocity direction doesn't need to be normalized.
					Vector3.Cross(ref velocityDirection, ref this.supportData.Normal, out offVelocityDirection);

					this.linearJacobianA1 = velocityDirection;
					this.linearJacobianA2 = offVelocityDirection;
					this.linearJacobianB1 = -velocityDirection;
					this.linearJacobianB2 = -offVelocityDirection;
				}
				else
				{
					//If the character isn't trying to move, then the velocity directions are not well defined.
					//Instead, pick two arbitrary vectors on the support plane.
					//First guess will be based on the previous jacobian.
					//Project the old linear jacobian onto the support normal plane.
					float dot;
					Vector3.Dot(ref this.linearJacobianA1, ref this.supportData.Normal, out dot);
					Vector3 toRemove;
					Vector3.Multiply(ref this.supportData.Normal, dot, out toRemove);
					Vector3.Subtract(ref this.linearJacobianA1, ref toRemove, out this.linearJacobianA1);

					//Vector3.Cross(ref linearJacobianA2, ref supportData.Normal, out linearJacobianA1);
					float length = this.linearJacobianA1.LengthSquared();
					if (length < Toolbox.Epsilon)
					{
						//First guess failed.  Try the right vector.
						Vector3.Cross(ref Toolbox.RightVector, ref this.supportData.Normal, out this.linearJacobianA1);
						length = this.linearJacobianA1.LengthSquared();
						if (length < Toolbox.Epsilon)
						{
							//Okay that failed too! try the forward vector.
							Vector3.Cross(ref Toolbox.ForwardVector, ref this.supportData.Normal, out this.linearJacobianA1);
							length = this.linearJacobianA1.LengthSquared();
							//Unless something really weird is happening, we do not need to test any more axes.
						}

					}
					Vector3.Divide(ref this.linearJacobianA1, (float)Math.Sqrt(length), out this.linearJacobianA1);
					//Pick another perpendicular vector.  Don't need to normalize it since the normal and A1 are already normalized and perpendicular.
					Vector3.Cross(ref this.linearJacobianA1, ref this.supportData.Normal, out this.linearJacobianA2);

					//B's linear jacobians are just -A's.
					this.linearJacobianB1 = -this.linearJacobianA1;
					this.linearJacobianB2 = -this.linearJacobianA2;
				}

				if (this.supportEntity != null)
				{
					//Compute the angular jacobians.
					Vector3 supportToContact = this.supportData.Position - this.supportEntity.Position;
					//Since we treat the character to have infinite inertia, we're only concerned with the support's angular jacobians.
					//Note the order of the cross product- it is reversed to negate the result.
					Vector3.Cross(ref this.linearJacobianA1, ref supportToContact, out this.angularJacobianB1);
					Vector3.Cross(ref this.linearJacobianA2, ref supportToContact, out this.angularJacobianB2);

				}
				else
				{
					//If we're not standing on an entity, there are no angular jacobians.
					this.angularJacobianB1 = new Vector3();
					this.angularJacobianB2 = new Vector3();
				}
			}
			else
			{
				//If the character is floating, then the jacobians are simply the 3d movement direction and the perpendicular direction on the character's horizontal plane.
				this.linearJacobianA1 = movementDirectionIn3D;
				this.linearJacobianA2 = Vector3.Cross(this.linearJacobianA1, this.character.Down);
			}

			//Compute the target velocity (in constraint space) for this frame.  The hard work has already been done.
			this.targetVelocity.X = this.maxSpeed;
			this.targetVelocity.Y = 0;

			//the mass matrix is defined entirely by the character.
			Matrix2x2.CreateScale(this.character.Body.Mass, out this.massMatrix);

			//If we're trying to stand still on an object that's moving, use a position correction term to keep the character
			//from drifting due to accelerations. 
			//First thing to do is to check to see if we're moving into a traction/trying to stand still state from a 
			//non-traction || trying to move state.  Either that, or we've switched supports and need to update the offset.
			if (this.supportEntity != null &&
				((this.wasTryingToMove && !isTryingToMove) || (!this.hadTraction && this.supportData.HasTraction) ||
				 this.supportEntity != this.previousSupportEntity))
			{
				//We're transitioning into a new 'use position correction' state.
				//Force a recomputation of the local offset.
				//The time since transition is used as a flag.
				this.timeSinceTransition = 0;
			}

			//The state is now up to date.  Compute an error and velocity bias, if needed.
			if (!isTryingToMove && this.supportData.HasTraction && this.supportEntity != null)
			{
				//Compute the time it usually takes for the character to slow down while it has traction.
				var tractionDecelerationTime = this.speed / (this.maximumForce * this.character.Body.InverseMass);

				var characterBody = this.character.Body;
				var characterHalfHeight = characterBody.Radius + characterBody.Length * .5f;
				if (this.timeSinceTransition >= 0 && this.timeSinceTransition < tractionDecelerationTime)
					this.timeSinceTransition += dt;

				if (this.timeSinceTransition >= tractionDecelerationTime)
				{
					Vector3.Multiply(ref downDirection, characterHalfHeight, out this.positionLocalOffset);
					this.positionLocalOffset = (this.positionLocalOffset + this.character.Body.Position) - this.supportEntity.Position;
					this.positionLocalOffset = Matrix3x3.TransformTranspose(this.positionLocalOffset, this.supportEntity.OrientationMatrix);
					this.timeSinceTransition = -1; //Negative 1 means that the offset has been computed.
				}
				if (this.timeSinceTransition < 0)
				{
					Vector3 targetPosition;
					Vector3.Multiply(ref downDirection, characterHalfHeight, out targetPosition);
					targetPosition += characterBody.Position;
					Vector3 worldSupportLocation = Matrix3x3.Transform(this.positionLocalOffset, this.supportEntity.OrientationMatrix) + this.supportEntity.Position;
					Vector3 error;
					Vector3.Subtract(ref targetPosition, ref worldSupportLocation, out error);
					//If the error is too large, then recompute the offset.  We don't want the character rubber banding around.
					if (error.LengthSquared() > .15f * .15f)
					{
						Vector3.Multiply(ref downDirection, characterHalfHeight, out this.positionLocalOffset);
						this.positionLocalOffset = (this.positionLocalOffset + characterBody.Position) - this.supportEntity.Position;
						this.positionLocalOffset = Matrix3x3.TransformTranspose(this.positionLocalOffset, this.supportEntity.OrientationMatrix);
						this.positionCorrectionBias = new Vector2();
					}
					else
					{
						//The error in world space is now available.  We can't use this error to directly create a velocity bias, though.
						//It needs to be transformed into constraint space where the constraint operates.
						//Use the jacobians!
						Vector3.Dot(ref error, ref this.linearJacobianA1, out this.positionCorrectionBias.X);
						Vector3.Dot(ref error, ref this.linearJacobianA2, out this.positionCorrectionBias.Y);
						//Scale the error so that a portion of the error is resolved each frame.
						Vector2.Multiply(ref this.positionCorrectionBias, .2f / dt, out this.positionCorrectionBias);
					}
				}
			}
			else
			{
				this.timeSinceTransition = 0;
				this.positionCorrectionBias = new Vector2();
			}

			this.wasTryingToMove = isTryingToMove;
			this.hadTraction = this.supportData.HasTraction;
			this.previousSupportEntity = this.supportEntity;
		}

		/// <summary>
		/// Performs any per-frame initialization needed by the constraint that must be done with exclusive access
		/// to the connected objects.
		/// </summary>
		public override void ExclusiveUpdate()
		{
			//Warm start the constraint using the previous impulses and the new jacobians!
			Vector3 impulse = new Vector3();
			float x = this.accumulatedImpulse.X;
			float y = this.accumulatedImpulse.Y;
			impulse.X = this.linearJacobianA1.X * x + this.linearJacobianA2.X * y;
			impulse.Y = this.linearJacobianA1.Y * x + this.linearJacobianA2.Y * y;
			impulse.Z = this.linearJacobianA1.Z * x + this.linearJacobianA2.Z * y;

			this.character.Body.ApplyLinearImpulse(ref impulse);
		}

		/// <summary>
		/// Computes a solution to the constraint.
		/// </summary>
		/// <returns>Impulse magnitude computed by the iteration.</returns>
		public override float SolveIteration()
		{
			Vector2 relativeVelocity = this.RelativeVelocity;
			Vector2.Add(ref relativeVelocity, ref this.positionCorrectionBias, out relativeVelocity);

			//Create the full velocity change, and convert it to an impulse in constraint space.
			Vector2 lambda;
			Vector2.Subtract(ref this.targetVelocity, ref relativeVelocity, out lambda);
			Matrix2x2.Transform(ref lambda, ref this.massMatrix, out lambda);

			//Add and clamp the impulse.

			Vector2 previousAccumulatedImpulse = this.accumulatedImpulse;
			if (this.MovementMode == MovementMode.Floating)
			{
				//If it's floating, clamping rules are different.
				//The constraint is not permitted to slow down the character; only speed it up.
				//This offers a hole for an exploit; by jumping and curving just right,
				//the character can accelerate beyond its maximum speed.  A bit like an HL2 speed run.
				this.accumulatedImpulse.X = MathHelper.Clamp(this.accumulatedImpulse.X + lambda.X, 0, this.maxForce);
				this.accumulatedImpulse.Y = 0;
			}
			else
			{
				Vector2.Add(ref lambda, ref this.accumulatedImpulse, out this.accumulatedImpulse);
				float length = this.accumulatedImpulse.LengthSquared();
				if (length > this.maxForce * this.maxForce)
				{
					Vector2.Multiply(ref this.accumulatedImpulse, this.maxForce / (float) Math.Sqrt(length),
					                 out this.accumulatedImpulse);
				}
			}
			Vector2.Subtract(ref this.accumulatedImpulse, ref previousAccumulatedImpulse, out lambda);

			//Use the jacobians to put the impulse into world space.
			Vector3 impulse = new Vector3();
			float x = lambda.X;
			float y = lambda.Y;
			impulse.X = this.linearJacobianA1.X * x + this.linearJacobianA2.X * y;
			impulse.Y = this.linearJacobianA1.Y * x + this.linearJacobianA2.Y * y;
			impulse.Z = this.linearJacobianA1.Z * x + this.linearJacobianA2.Z * y;

			this.character.Body.ApplyLinearImpulse(ref impulse);

			return (Math.Abs(lambda.X) + Math.Abs(lambda.Y));
		}

		/// <summary>
		/// Gets the current velocity between the character and its support in constraint space.
		/// The X component corresponds to velocity along the movement direction.
		/// The Y component corresponds to velocity perpendicular to the movement direction and support normal.
		/// </summary>
		public Vector2 RelativeVelocity
		{
			get
			{
				//The relative velocity's x component is in the movement direction.
				//y is the perpendicular direction.
				Vector2 relativeVelocity = new Vector2();

				Vector3 bodyVelocity = this.character.Body.LinearVelocity;
				Vector3.Dot(ref this.linearJacobianA1, ref bodyVelocity, out relativeVelocity.X);
				Vector3.Dot(ref this.linearJacobianA2, ref bodyVelocity, out relativeVelocity.Y);

				float x, y;
				if (this.supportEntity != null)
				{
					Vector3 supportLinearVelocity = this.supportEntity.LinearVelocity;
					Vector3 supportAngularVelocity = this.supportEntity.AngularVelocity;


					Vector3.Dot(ref this.linearJacobianB1, ref supportLinearVelocity, out x);
					Vector3.Dot(ref this.linearJacobianB2, ref supportLinearVelocity, out y);
					relativeVelocity.X += x;
					relativeVelocity.Y += y;
					Vector3.Dot(ref this.angularJacobianB1, ref supportAngularVelocity, out x);
					Vector3.Dot(ref this.angularJacobianB2, ref supportAngularVelocity, out y);
					relativeVelocity.X += x;
					relativeVelocity.Y += y;
				}

				return relativeVelocity;
			}
		}

		/// <summary>
		/// Gets the current velocity between the character and its support.
		/// </summary>
		public Vector3 RelativeWorldVelocity
		{
			get
			{
				Vector3 bodyVelocity = this.character.Body.LinearVelocity;
				if (this.supportEntity != null)
					return bodyVelocity -
					       Toolbox.GetVelocityOfPoint(this.supportData.Position, this.supportEntity.Position,
					                                  this.supportEntity.LinearVelocity, this.supportEntity.AngularVelocity);
				return bodyVelocity;
			}
		}
	}

	public enum MovementMode
	{
		Traction,
		Floating
	}
}
