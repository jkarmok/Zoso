using System;
using BEPUphysics.Constraints;
using BEPUphysics.Entities;
using BEPUutilities;
using BEPUutilities.DataStructures;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Settings;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	/// <summary>
	/// Keeps a character glued to the ground, if possible.
	/// </summary>
	public class VerticalMotionConstraint : SolverUpdateable
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
				}
			}
		}

		private float maximumGlueForce = 5000f;

		/// <summary>
		/// Gets or sets the maximum force that the constraint will apply in attempting to keep the character stuck to the ground.
		/// </summary>
		public float MaximumGlueForce
		{
			get { return this.maximumGlueForce; }
			set
			{
				if (this.maximumGlueForce < 0)
					throw new ArgumentException("Value must be nonnegative.");
				this.maximumGlueForce = value;
			}
		}

		private float maximumForce;

		/// <summary>
		/// Gets the effective mass felt by the constraint.
		/// </summary>
		public float EffectiveMass
		{
			get { return this.effectiveMass; }
		}

		private float effectiveMass;
		private Entity supportEntity;
		private Vector3 linearJacobianA;
		private Vector3 linearJacobianB;
		private Vector3 angularJacobianB;

		private float accumulatedImpulse;
		private float permittedVelocity;

		/// <summary>
		/// Constructs a new vertical motion constraint.
		/// </summary>
		/// <param name="characterController">Character governed by the constraint.</param>
		public VerticalMotionConstraint(BEPUCharacterController characterController)
		{
			this.character = characterController;
		}

		protected override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
		{
			var entityCollidable = this.supportData.SupportObject as EntityCollidable;
			if (entityCollidable != null)
				outputInvolvedEntities.Add(entityCollidable.Entity);
			outputInvolvedEntities.Add(this.character.Body);
		}

		/// <summary>
		/// Updates the activity state of the constraint.
		/// Called automatically by the solver.
		/// </summary>
		public override void UpdateSolverActivity()
		{
			if (this.supportData.HasTraction)
				base.UpdateSolverActivity();
			else
				this.isActiveInSolver = false;
		}

		/// <summary>
		/// Performs any per-frame computation needed by the constraint.
		/// </summary>
		/// <param name="dt">Time step duration.</param>
		public override void Update(float dt)
		{
			//Collect references, pick the mode, and configure the coefficients to be used by the solver.
			if (this.supportData.SupportObject != null)
			{
				//Get an easy reference to the support.
				var support = this.supportData.SupportObject as EntityCollidable;
				this.supportEntity = support != null ? support.Entity : null;
			}
			else
			{
				this.supportEntity = null;
			}

			this.maximumForce = this.maximumGlueForce * dt;

			//If we don't allow the character to get out of the ground, it could apply some significant forces to a dynamic support object.
			//Technically, there exists a better estimate of the necessary speed, but choosing the maximum position correction speed is a nice catch-all.
			//If you change that correction speed, watch out!!! It could significantly change the way the character behaves when trying to glue to surfaces.
			if (this.supportData.Depth > 0)
				this.permittedVelocity = CollisionResponseSettings.MaximumPenetrationRecoverySpeed;
			else
				this.permittedVelocity = 0;

			//Compute the jacobians and effective mass matrix.  This constraint works along a single degree of freedom, so the mass matrix boils down to a scalar.

			this.linearJacobianA = this.supportData.Normal;
			Vector3.Negate(ref this.linearJacobianA, out this.linearJacobianB);
			this.effectiveMass = this.character.Body.InverseMass;
			if (this.supportEntity != null)
			{
				Vector3 offsetB = this.supportData.Position - this.supportEntity.Position;
				Vector3.Cross(ref offsetB, ref this.linearJacobianB, out this.angularJacobianB);
			}
			this.effectiveMass = 1 / this.effectiveMass;
			//So much nicer and shorter than the horizontal constraint!
		}

		/// <summary>
		/// Performs any per-frame computations needed by the constraint that require exclusive access to the involved entities.
		/// </summary>
		public override void ExclusiveUpdate()
		{
		}

		/// <summary>
		/// Computes a solution to the constraint.
		/// </summary>
		/// <returns>Magnitude of the applied impulse.</returns>
		public override float SolveIteration()
		{
			//The relative velocity's x component is in the movement direction.
			//y is the perpendicular direction.
			float relativeVelocity = this.RelativeVelocity + this.permittedVelocity;

			//Create the full velocity change, and convert it to an impulse in constraint space.
			float lambda = -relativeVelocity * this.effectiveMass;

			//Add and clamp the impulse.
			float previousAccumulatedImpulse = this.accumulatedImpulse;
			this.accumulatedImpulse = MathHelper.Clamp(this.accumulatedImpulse + lambda, 0, this.maximumForce);
			lambda = this.accumulatedImpulse - previousAccumulatedImpulse;
			//Use the jacobians to put the impulse into world space.

			return Math.Abs(lambda);
		}

		/// <summary>
		/// Gets the relative velocity between the character and its support along the support normal.
		/// </summary>
		public float RelativeVelocity
		{
			get
			{
				float relativeVelocity;

				Vector3 bodyVelocity = this.character.Body.LinearVelocity;
				Vector3.Dot(ref this.linearJacobianA, ref bodyVelocity, out relativeVelocity);

				if (this.supportEntity != null)
				{
					Vector3 supportLinearVelocity = this.supportEntity.LinearVelocity;
					Vector3 supportAngularVelocity = this.supportEntity.AngularVelocity;

					float supportVelocity;
					Vector3.Dot(ref this.linearJacobianB, ref supportLinearVelocity, out supportVelocity);
					relativeVelocity += supportVelocity;
					Vector3.Dot(ref this.angularJacobianB, ref supportAngularVelocity, out supportVelocity);
					relativeVelocity += supportVelocity;

				}
				return relativeVelocity;
			}
		}
	}
}
