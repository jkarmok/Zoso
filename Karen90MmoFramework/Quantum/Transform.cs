namespace Karen90MmoFramework.Quantum
{
	public sealed class Transform
	{
		#region Constants and Fields

		private readonly Bounds bounds;
		private Vector3 position;
		private Quaternion rotation;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the bounds
		/// </summary>
		public Bounds Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		/// <summary>
		/// Gets or sets the position
		/// </summary>
		public Vector3 Position
		{
			get
			{
				return this.position;
			}

			set
			{
				this.position = value;
			}
		}

		/// <summary>
		/// Gets or sets the rotation
		/// </summary>
		public Quaternion Rotation
		{
			get
			{
				return this.rotation;
			}

			set
			{
				this.rotation = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the eular angles
		/// </summary>
		public Vector3 EularAngles
		{
			get
			{
				return this.rotation.EularAngles();
			}

			set
			{
				this.rotation = Quaternion.CreateEular(value);
			}
		}

		/// <summary>
		/// Gets or sets the up direction of the transform
		/// </summary>
		public Vector3 Up
		{
			get
			{
				return Vector3.Transform(Vector3.Up, this.rotation);
			}

			set
			{
				this.rotation = Quaternion.FromToRotation(Vector3.Up, value);
			}
		}

		/// <summary>
		/// Gets or sets the right direction of the transform
		/// </summary>
		public Vector3 Right
		{
			get
			{
				return Vector3.Transform(Vector3.Right, this.rotation);
			}

			set
			{
				this.rotation = Quaternion.FromToRotation(Vector3.Right, value);
			}
		}

		/// <summary>
		/// Gets or sets the forward direction of the transform
		/// </summary>
		public Vector3 Forward
		{
			get
			{
				return Vector3.Transform(Vector3.Forward, this.rotation);
			}

			set
			{
				this.rotation = Quaternion.FromToRotation(Vector3.Forward, value);
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of transform
		/// </summary>
		public Transform(Bounds bounds)
			: this(bounds,  Vector3.Zero, Quaternion.Identity)
		{
		}

		/// <summary>
		/// Creates a new instance of transform
		/// </summary>
		public Transform(Bounds bounds, Vector3 position, Quaternion rotation)
		{
			this.bounds = bounds;
			this.Position = position;
			this.Rotation = rotation;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Translates the transform
		/// </summary>
		public void Translate(float x, float y, float z)
		{
			this.Translate(x, y, z, Space.Self);
		}

		/// <summary>
		/// Translates the transform
		/// </summary>
		public void Translate(float x, float y, float z, Space relativeSpace)
		{
			this.Translate(new Vector3(x, y, z), relativeSpace);
		}

		/// <summary>
		/// Translates the transform relative to self space
		/// </summary>
		public void Translate(Vector3 translation)
		{
			this.Translate(translation, Space.Self);
		}

		/// <summary>
		/// Translates the transform
		/// </summary>
		public void Translate(Vector3 translation, Space relativeSpace)
		{
			if (relativeSpace == Space.World)
				this.position += translation;
			else
				this.position += this.TransformDirection(translation);
		}

		/// <summary>
		/// Rotates the transform
		/// </summary>
		public void Rotate(float xAngle, float yAngle, float zAngle)
		{
			this.Rotate(xAngle, yAngle, zAngle, Space.Self);
		}

		/// <summary>
		/// Rotates the transform
		/// </summary>
		public void Rotate(float xAngle, float yAngle, float zAngle, Space relativeSpace)
		{
			this.Rotate(new Vector3(xAngle, yAngle, zAngle), relativeSpace);
		}

		/// <summary>
		/// Rotates the transform
		/// </summary>
		public void Rotate(Vector3 eularAngles)
		{
			this.Rotate(eularAngles, Space.Self);
		}

		/// <summary>
		/// Rotates the transform
		/// </summary>
		public void Rotate(Vector3 eularAngles, Space relativeSpace)
		{
			// NOTE: for now transforms cannot be parented so relativeSpace doesn't have meaning when it comes to rotation
			
			var quaternion = Quaternion.CreateEular(eularAngles);
			if (relativeSpace == Space.World)
				this.rotation = quaternion * this.rotation;
			else
				this.rotation = quaternion * this.rotation;
		}

		/// <summary>
		/// Transforms a vector according to the transform's rotation
		/// </summary>
		public Vector3 TransformDirection(Vector3 direction)
		{
			return Vector3.Transform(direction, rotation);
		}

		#endregion
	}
}
