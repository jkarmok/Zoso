using System;

namespace Karen90MmoFramework.Quantum
{
	public struct Quaternion : IEquatable<Quaternion>
	{
		#region Constants and Fields

		public const float kEpsilon = 1E-06f;

		/// <summary>
		/// The x component of the quaternion
		/// </summary>
		public float X;

		/// <summary>
		/// The y component of the quaternion
		/// </summary>
		public float Y;

		/// <summary>
		/// The z component of the quaternion
		/// </summary>
		public float Z;

		/// <summary>
		/// The w component of the quaternion
		/// </summary>
		public float W;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the identity quaternion
		/// </summary>
		public static Quaternion Identity
		{
			get
			{
				return new Quaternion(0f, 0f, 0f, 1f);
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new <see cref="Quaternion"/> instance.
		/// </summary>
		public Quaternion(Vector3 vectorComponent, float scalarComponent)
		{
			this.X = vectorComponent.X;
			this.Y = vectorComponent.Y;
			this.Z = vectorComponent.Z;
			this.W = scalarComponent;
		}

		/// <summary>
		/// Initializes a new <see cref="Quaternion"/> instance.
		/// </summary>
		public Quaternion(float x, float y, float z, float w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Compares two quaternions for equality
		/// </summary>
		public static bool operator ==(Quaternion lhs, Quaternion rhs)
		{
			return lhs.X.Equals(rhs.X) && lhs.Y.Equals(rhs.Y) && lhs.Z.Equals(rhs.Z) && lhs.W.Equals(rhs.W);
		}

		/// <summary>
		/// Compares two quaternions inequality
		/// </summary>
		public static bool operator !=(Quaternion lhs, Quaternion rhs)
		{
			return !lhs.X.Equals(rhs.X) || !lhs.Y.Equals(rhs.Y) || !lhs.Z.Equals(rhs.Z) || !lhs.W.Equals(rhs.W);
		}

		/// <summary>
		/// Negates a quaternion
		/// </summary>
		public static Quaternion operator -(Quaternion a)
		{
			return new Quaternion(-a.X, -a.Y, -a.Z, -a.W);
		}
		
		/// <summary>
		/// Adds two quaternions
		/// </summary>
		public static Quaternion operator+(Quaternion a, Quaternion b)
		{
			return new Quaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}

		/// <summary>
		/// Multiplies two quaternions
		/// </summary>
		public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion(
				lhs.W * rhs.X + lhs.X * rhs.W + lhs.Y * rhs.Z - lhs.Z * rhs.Y, 
				lhs.W * rhs.Y + lhs.Y * rhs.W + lhs.Z * rhs.X - lhs.X * rhs.Z, 
				lhs.W * rhs.Z + lhs.Z * rhs.W + lhs.X * rhs.Y - lhs.Y * rhs.X, 
				lhs.W * rhs.W - lhs.X * rhs.X - lhs.Y * rhs.Y - lhs.Z * rhs.Z);
		}

		/// <summary>
		/// Multiplies two quaternions
		/// </summary>
		public static Quaternion operator *(Quaternion a, float f)
		{
			return new Quaternion(a.X * f, a.Y * f, a.Z * f, a.W * f);
		}

		/// <summary>
		/// Gets the dot product of two quaternions
		/// </summary>
		public static float Dot(Quaternion a, Quaternion b)
		{
			return (a.X * b.X + a.Y * b.Y + a.Z * a.Z + a.W * a.W);
		}

		/// <summary>
		/// Normalizes a quaternion
		/// </summary>
		public static Quaternion Normalize(Quaternion value)
		{
			// ReSharper disable CompareOfFloatsByEqualityOperator
			var magnitude = Mathf.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W);
			if(magnitude == 0f)
			{
				return Quaternion.Identity;
			}
			// ReSharper restore CompareOfFloatsByEqualityOperator

			var factor = 1f / magnitude;
			return new Quaternion()
				{
					X = value.X * factor,
					Y = value.Y * factor,
					Z = value.Z * factor,
					W = value.W * factor
				};
		}

		/// <summary>
		/// Normalizes the quaternion
		/// </summary>
		public Quaternion Normalize()
		{
			return Quaternion.Normalize(this);
		}

		/// <summary>
		/// Returns the conjugate of a quaternion
		/// </summary>
		public static Quaternion Conjugate(Quaternion value)
		{
			return new Quaternion(-value.X, -value.Y, -value.Z, -value.W);
		}

		/// <summary>
		/// Inverses a quaternion
		/// </summary>
		public static Quaternion Inverse(Quaternion value)
		{
			// ReSharper disable CompareOfFloatsByEqualityOperator
			var sqrMagnitude = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
			if (sqrMagnitude == 0f)
			{
				return Quaternion.Identity;
			}
			// ReSharper restore CompareOfFloatsByEqualityOperator

			var factor = 1f / sqrMagnitude;
			return new Quaternion()
				{
					X = -value.X * factor,
					Y = -value.Y * factor,
					Z = -value.Z * factor,
					W = -value.W * factor
				};
		}

		/// <summary>
		/// Creates a quaternion whose rotation is from a vector to another
		/// </summary>
		public static Quaternion FromToRotation(Vector3 from, Vector3 to)
		{
			var sqMag = from.SqrMagnitude() * to.SqrMagnitude();
			if (sqMag < kEpsilon * kEpsilon)
			{
				return Quaternion.Identity;
			}

			var sqrt = Mathf.Sqrt(sqMag);
			var dot = Vector3.Dot(from, to);
			var cost = dot / sqrt;

			// vectors are the same
			if(cost >= 1.0f)
			{
				return Quaternion.Identity;
			}

			// vectors are parallel opposite
			if (cost < kEpsilon - 1.0f)
			{
				var axis = Vector3.Cross(Vector3.Up, from);
				if (axis.SqrMagnitude() < Vector3.kEpsilon)
					axis = Vector3.Cross(Vector3.Right, from);

				return Quaternion.CreateAxisAngle(axis, 180f).Normalize();
			}

			var perp = Vector3.Cross(from, to);
			return new Quaternion(perp.X, perp.Y, perp.Z, sqrt + dot).Normalize();
		}

		/// <summary>
		/// Creates a quaternion whose rotation is about an axis
		/// </summary>
		public static Quaternion CreateAxisAngle(Vector3 axis, float angle)
		{
			var ah = angle * 0.5f * Mathf.DEG2_RAD;
			var sinrh = Mathf.Sin(ah);
			var cosrh = Mathf.Cos(ah);

			return new Quaternion()
				{
					X = axis.X * sinrh,
					Y = axis.Y * sinrh,
					Z = axis.Z * sinrh,
					W = cosrh
				};
		}

		/// <summary>
		/// Creates a quaternion from eular angles
		/// </summary>
		public static Quaternion CreateEular(float x, float y, float z)
		{
			return Quaternion.CreateEular(new Vector3(x, y, z));
		}

		/// <summary>
		/// Creates a quaternion from eular angles
		/// </summary>
		public static Quaternion CreateEular(Vector3 eularAngles)
		{
			var xh = eularAngles.X * 0.5f * Mathf.DEG2_RAD;
			var sinxh = Mathf.Sin(xh);
			var cosxh = Mathf.Cos(xh);
			var yh = eularAngles.Y * 0.5f * Mathf.DEG2_RAD;
			var sinyh = Mathf.Sin(yh);
			var cosyh = Mathf.Cos(yh);
			var zh = eularAngles.Z * 0.5f * Mathf.DEG2_RAD;
			var sinzh = Mathf.Sin(zh);
			var coszh = Mathf.Cos(zh);

			return new Quaternion()
			{
				X = (cosyh * sinxh * coszh + sinyh * cosxh * sinzh),
				Y = (sinyh * cosxh * coszh - cosyh * sinxh * sinzh),
				Z = (cosyh * cosxh * sinzh - sinyh * sinxh * coszh),
				W = (cosyh * cosxh * coszh + sinyh * sinxh * sinzh)
			};
		}

		/// <summary>
		/// Converts the quaternion to eular angles
		/// </summary>
		/// <returns></returns>
		public Vector3 EularAngles()
		{
			float ax, ay, az;

			var singularity = 2.0f * (this.X * this.W - this.Y * this.Z);
			if(singularity > 0.98f)
			{
				ax = Mathf.PI / 2f;
				ay = Mathf.Atan2(2.0f * (this.Z * this.X + this.Y * this.W), 2.0f * (this.Z * this.X - this.Y * this.W));
				az = 0;
			}
			else if(singularity < -0.98f)
			{
				ax = -Mathf.PI / 2f;
				ay = Mathf.Atan2(2.0f * (this.Z * this.X + this.Y * this.W), 2.0f * (this.Z * this.X - this.Y * this.W));
				az = 0;
			}
			else
			{
				ax = Mathf.Asin(singularity);
				ay = Mathf.Atan2(2.0f * (this.Z * this.X + this.Y * this.W), 1.0f - 2.0f * (this.Y * this.Y + this.X * this.X));
				az = Mathf.Atan2(2.0f * (this.X * this.Y + this.Z * this.W), 1.0f - 2.0f * (this.Z * this.Z + this.X * this.X));
			}

			return new Vector3()
				{
					X = (360 + ax * Mathf.RAD2_DEG) % 360f,
					Y = (360 + ay * Mathf.RAD2_DEG) % 360f,
					Z = (360 + az * Mathf.RAD2_DEG) % 360f,
				};
		}

		/// <summary>
		/// Sets x, y, z, w
		/// </summary>
		public void Set(float x, float y, float z, float w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		/// <summary>
		/// Compares the Quaternion to an object
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Quaternion)
			{
				return this == (Quaternion)obj;
			}

			return false;
		}

		/// <summary>
		/// Gets the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode() ^ this.W.GetHashCode();
		}

		/// <summary>
		/// Builds a string showing all the x, y, z components
		/// </summary>
		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}, {3}]", this.X, this.Y, this.Z, this.W);
		}

		/// <summary>
		/// Builds a string showing all the x, y, z components
		/// </summary>
		public string ToRoundedString()
		{
			return string.Format("[{0}, {1}, {2}, {3}]", Mathf.Round(X), Mathf.Round(Y), Mathf.Round(Z), Mathf.Round(W));
		}

		#endregion

		#region IEquatable Implementation

		/// <summary>
		/// Compares this instance to another quaternion
		/// </summary>
		public bool Equals(Quaternion other)
		{
			return this == other;
		}

		#endregion
	}
}
