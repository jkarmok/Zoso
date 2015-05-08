using System.Linq;
using DigitalRune.Physics;
using DigitalRune.Physics.Specialized;
using DigitalRune.Mathematics.Algebra;

using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public class RuneCharacterController : KinematicCharacterController, ICharacterController
	{
		#region Constructors and Destructors

		public RuneCharacterController(Simulation simulation, string name, object userData)
			: base(simulation)
		{
			Body.Name = name;
			Body.UserData = userData;
			this.orientation = QuaternionF.Identity;
			this.localVelocity = Vector3F.Zero;
			this.worldVelocity = Vector3F.Zero;
		}

		#endregion

		#region Implementation of ICharacterController

		private QuaternionF orientation;
		private Vector3F localVelocity;
		private Vector3F worldVelocity;

		Vector3 ICharacterController.Position
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

		Quaternion ICharacterController.Orientation
		{
			get
			{
				return this.orientation.ToGameQuaternion();
			}
			set
			{
				this.orientation = value.ToRuneQuaternion();
				this.worldVelocity = this.orientation.Rotate(localVelocity);
			}
		}

		Vector3 ICharacterController.LocalVelocity
		{
			get
			{
				return this.localVelocity.ToGameVector();
			}
			set
			{
				this.localVelocity = value.ToRuneVector();
				this.worldVelocity = this.orientation.Rotate(localVelocity);
			}
		}

		Vector3 ICharacterController.WorldVelocity
		{
			get
			{
				return this.worldVelocity.ToGameVector();
			}
		}

		float ICharacterController.Radius
		{
			get
			{
				return this.Width * 0.5f;
			}
		}

		public void Update(float delta)
		{
			this.Move(worldVelocity, 0, delta);
		}

		#endregion
	}
}
