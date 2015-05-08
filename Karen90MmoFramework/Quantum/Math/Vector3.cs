using System;

namespace Karen90MmoFramework.Quantum
{
	public struct Vector3 : IEquatable<Vector3>
	{
		#region Constants and Fields

		public const float kEpsilon = 1E-05f;

		/// <summary>
		/// The x component of the vector
		/// </summary>
		public float X;

		/// <summary>
		/// The y component of the vector
		/// </summary>
		public float Y;

		/// <summary>
		/// The z component of the vector
		/// </summary>
		public float Z;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the zero vector
		/// </summary>
		public static Vector3 Zero
		{
			get
			{
				return new Vector3(0f, 0f, 0f);
			}
		}

		/// <summary>
		/// Gets the one vector
		/// </summary>
		public static Vector3 One
		{
			get
			{
				return new Vector3(1f, 1f, 1f);
			}
		}

		/// <summary>
		/// Gets the forward vector
		/// </summary>
		public static Vector3 Forward
		{
			get
			{
				return new Vector3(0f, 0f, 1f);
			}
		}

		/// <summary>
		/// Gets the backward vector
		/// </summary>
		public static Vector3 Backward
		{
			get
			{
				return new Vector3(0f, 0f, -1f);
			}
		}

		/// <summary>
		/// Gets the up vector
		/// </summary>
		public static Vector3 Up
		{
			get
			{
				return new Vector3(0f, 1f, 0f);
			}
		}

		/// <summary>
		/// Gets the down vector
		/// </summary>
		public static Vector3 Down
		{
			get
			{
				return new Vector3(0f, -1f, 0f);
			}
		}

		/// <summary>
		/// Gets the left vector
		/// </summary>
		public static Vector3 Left
		{
			get
			{
				return new Vector3(-1f, 0f, 0f);
			}
		}

		/// <summary>
		/// Gets the right vector
		/// </summary>
		public static Vector3 Right
		{
			get
			{
				return new Vector3(1f, 0f, 0f);
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new <see cref="Vector3"/> instance.
		/// </summary>
		public Vector3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Compares two vectors for equality
		/// </summary>
		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			return lhs.X.Equals(rhs.X) && lhs.Y.Equals(rhs.Y) && lhs.Z.Equals(rhs.Z);
		}

		/// <summary>
		/// Compares two Vectors for inequality
		/// </summary>
		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return !lhs.X.Equals(rhs.X) || !lhs.Y.Equals(rhs.Y) || !lhs.Z.Equals(rhs.Z);
		}

		/// <summary>
		/// Negates a vector
		/// </summary>
		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3 { X = -a.X, Y = -a.Y, Z = -a.Z };
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3 { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
		}

		/// <summary>
		/// Substracts one Vector from the other
		/// </summary>
		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3 { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };
		}

		/// <summary>
		/// Devides a vector by a vector
		/// </summary>
		public static Vector3 operator *(Vector3 a, Vector3 b)
		{
			return new Vector3 { X = a.X * b.X, Y = a.Y * b.Y, Z = a.Z * b.Z };
		}

		/// <summary>
		/// Multiples a vector by a value
		/// </summary>
		public static Vector3 operator *(Vector3 a, float b)
		{
			return new Vector3 { X = a.X * b, Y = a.Y * b, Z = a.Z * b };
		}

		/// <summary>
		/// Multiples a vector by a value
		/// </summary>
		public static Vector3 operator *(float a, Vector3 b)
		{
			return new Vector3 { X = b.X * a, Y = b.Y * a, Z = b.Z * a };
		}

		/// <summary>
		/// Devides a vector by a vector
		/// </summary>
		public static Vector3 operator /(Vector3 a, Vector3 b)
		{
			return new Vector3 { X = a.X / b.X, Y = a.Y / b.Y, Z = a.Z / b.Z };
		}

		/// <summary>
		/// Devides a vector by a value
		/// </summary>
		public static Vector3 operator /(Vector3 a, float b)
		{
			return new Vector3 { X = a.X / b, Y = a.Y / b, Z = a.Z / b };
		}

		/// <summary>
		/// Gets the max values from two vectors
		/// </summary>
		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			return new Vector3 { X = Mathf.Max(value1.X, value2.X), Y = Mathf.Max(value1.Y, value2.Y), Z = Mathf.Max(value1.Z, value2.Z) };
		}

		/// <summary>
		/// Gets the min values from two vectors
		/// </summary>
		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			return new Vector3 { X = Mathf.Min(value1.X, value2.X), Y = Mathf.Min(value1.Y, value2.Y), Z = Mathf.Min(value1.Z, value2.Z) };
		}

		/// <summary>
		/// Clamps a vector within min and max vectors
		/// </summary>
		public static Vector3 Clamp(Vector3 v, Vector3 min, Vector3 max)
		{
			var x = v.X;
			var mX = (double)x > (double)max.X ? max.X : x;
			var nX = (double)mX < (double)min.X ? min.X : mX;
			var y = v.Y;
			var mY = (double)y > (double)max.Y ? max.Y : y;
			var nY = (double)mY < (double)min.Y ? min.Y : mY;
			var z = v.Z;
			var mZ = (double)z > (double)max.Z ? max.Z : z;
			var nZ = (double)mZ < (double)min.Z ? min.Z : mZ;

			return new Vector3()
				{
					X = nX,
					Y = nY,
					Z = nZ
				};
		}

		/// <summary>
		/// Gets the dot product of two vectors
		/// </summary>
		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		/// Gets the cross product of two vector
		/// </summary>
		public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(lhs.Y*rhs.Z - lhs.Z*rhs.Y,
			                   lhs.Z*rhs.X - lhs.X*rhs.Z,
			                   lhs.X*rhs.Y - lhs.Y*rhs.X);
		}

		/// <summary>
		/// Gets the magnitude of a vector
		/// </summary>
		public static float Magnitude(Vector3 value)
		{
			return Mathf.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
		}

		/// <summary>
		/// Gets the magnitude of the vector
		/// </summary>
		public float Magnitude()
		{
			return Mathf.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
		}

		/// <summary>
		/// Gets the squared magnitude of a vector
		/// </summary>
		public static float SqrMagnitude(Vector3 value)
		{
			return value.X * value.X + value.Y * value.Y + value.Z * value.Z;
		}

		/// <summary>
		/// Gets the squared magnitude of the vector
		/// </summary>
		public float SqrMagnitude()
		{
			return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
		}

		/// <summary>
		/// Gets the distance between two vectors
		/// </summary>
		public static float Distance(Vector3 a, Vector3 b)
		{
			var dx = a.X - b.X;
			var dy = a.Y - b.Y;
			var dz = a.Z - b.Z;

			return Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
		}

		/// <summary>
		/// Gets the squared distance between two vectors
		/// </summary>
		public static float SqrDistance(Vector3 a, Vector3 b)
		{
			var dx = a.X - b.X;
			var dy = a.Y - b.Y;
			var dz = a.Z - b.Z;

			return dx * dx + dy * dy + dz * dz;
		}

		/// <summary>
		/// Normalizes a vector
		/// </summary>
		public static Vector3 Normalize(Vector3 value)
		{
			var factor = 1f / Mathf.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
			return new Vector3(value.X * factor, value.Y * factor, value.Z * factor);
		}

		/// <summary>
		/// Normalizes this vector
		/// </summary>
		public Vector3 Normalize()
		{
			return Vector3.Normalize(this);
		}

		/// <summary>
		/// Sets x, y, z
		/// </summary>
		public void Set(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		/// <summary>
		/// Transforms a vector using a matrix
		/// </summary>
		public static Vector3 Transform(Vector3 v, Matrix4x4 m)
		{
			return new Vector3()
			{
				X = (v.X * m.M00 + v.Y * m.M10 + v.Z * m.M20) + m.M30,
				Y = (v.X * m.M01 + v.Y * m.M11 + v.Z * m.M21) + m.M31,
				Z = (v.X * m.M02 + v.Y * m.M12 + v.Z * m.M22) + m.M32
			};
		}

		/// <summary>
		/// Transforms a vector using a rotation
		/// </summary>
		public static Vector3 Transform(Vector3 v, Quaternion r)
		{
			var qx2 = r.X + r.X;
			var qy2 = r.Y + r.Y;
			var qz2 = r.Z + r.Z;
			var qwx2 = r.W * qx2;
			var qwy2 = r.W * qy2;
			var qwz2 = r.W * qz2;
			var qxx2 = r.X * qx2;
			var qxy2 = r.X * qy2;
			var qxz2 = r.X * qz2;
			var qyy2 = r.Y * qy2;
			var qyz2 = r.Y * qz2;
			var qzz2 = r.Z * qz2;
			
			return new Vector3()
				{
					X = (v.X * (1.0f - qyy2 - qzz2) + v.Y * (qxy2 - qwz2) + v.Z * (qxz2 + qwy2)),
					Y = (v.X * (qxy2 + qwz2) + v.Y * (1.0f - qxx2 - qzz2) + v.Z * (qyz2 - qwx2)),
					Z = (v.X * (qxz2 - qwy2) + v.Y * (qyz2 + qwx2) + v.Z * (1.0f - qxx2 - qyy2))
				};
		}

		/// <summary>
		/// Lerps one vector to another over a time
		/// </summary>
		public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector3(from.X + (to.X - from.X) * t, from.Y + (to.Y - from.Y) * t, from.Z + (to.Z - from.Z) * t);
		}

		/// <summary>
		/// Moves a vector towards another
		/// </summary>
		/// <returns></returns>
		public static Vector3 MoveTowards(Vector3 from, Vector3 to, float maxDistanceDelta)
		{
			var direction = (to - from);
			var magnitude = Vector3.Magnitude(direction);
			if (magnitude <= maxDistanceDelta || magnitude == 0f)
			{
				return to;
			}

			return from + to / magnitude * maxDistanceDelta;
		}

		/// <summary>
		/// Returns the angle between two vectors
		/// </summary>
		public static float Angle(Vector3 from, Vector3 to)
		{
			var dot = Vector3.Dot(from, to);
			var w = Mathf.Sqrt(from.SqrMagnitude() * to.SqrMagnitude());

			return Mathf.Acos(dot / w) * Mathf.RAD2_DEG;
		}

		/// <summary>
		/// Compares the Vector to an object
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Vector3)
			{
				return this == (Vector3)obj;
			}

			return false;
		}

		/// <summary>
		/// Gets the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
		}

		/// <summary>
		/// Builds a string showing all the x, y, z components
		/// </summary>
		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}]", this.X, this.Y, this.Z);
		}

		/// <summary>
		/// Builds a string showing all the x, y, z components
		/// </summary>
		public string ToRoundedString()
		{
			return string.Format("[{0}, {1}, {2}]", Mathf.Round(X), Mathf.Round(Y), Mathf.Round(Z));
		}

		#endregion

		#region IEquatable Implementation

		/// <summary>
		/// Compares this instance to another vector
		/// </summary>
		public bool Equals(Vector3 other)
		{
			return this == other;
		}

		#endregion
	}
}