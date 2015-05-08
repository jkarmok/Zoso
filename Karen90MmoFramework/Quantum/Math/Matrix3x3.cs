using System;

namespace Karen90MmoFramework.Quantum
{
	public struct Matrix3x3 : IEquatable<Matrix3x3>
	{
		#region Constants and Fields

		public float M00;
		public float M01;
		public float M02;

		public float M10;
		public float M11;
		public float M12;

		public float M20;
		public float M21;
		public float M22;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the 0 matrix
		/// </summary>
		public static Matrix3x3 Zero
		{
			get
			{
				return new Matrix3x3()
					{
						M00 = 0.0f,
						M01 = 0.0f,
						M02 = 0.0f,
						M10 = 0.0f,
						M11 = 0.0f,
						M12 = 0.0f,
						M20 = 0.0f,
						M21 = 0.0f,
						M22 = 0.0f,
					};
			}
		}

		/// <summary>
		/// Gets the identity matrix
		/// </summary>
		public static Matrix3x3 Identity
		{
			get
			{
				return new Matrix3x3()
				{
					M00 = 1.0f,
					M01 = 0.0f,
					M02 = 0.0f,
					M10 = 0.0f,
					M11 = 1.0f,
					M12 = 0.0f,
					M20 = 0.0f,
					M21 = 0.0f,
					M22 = 1.0f,
				};
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new <see cref="Matrix3x3"/> instance.
		/// </summary>
		public Matrix3x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
		{
			M00 = m00;
			M01 = m01;
			M02 = m02;
			M10 = m10;
			M11 = m11;
			M12 = m12;
			M20 = m20;
			M21 = m21;
			M22 = m22;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Compares two matrices for equality
		/// </summary>
		public static bool operator ==(Matrix3x3 lhs, Matrix3x3 rhs)
		{
			return lhs.M00.Equals(rhs.M00) && lhs.M01.Equals(rhs.M01) && lhs.M02.Equals(rhs.M02) &&
			       lhs.M10.Equals(rhs.M10) && lhs.M11.Equals(rhs.M11) && lhs.M12.Equals(rhs.M12) &&
			       lhs.M20.Equals(rhs.M20) && lhs.M21.Equals(rhs.M21) && lhs.M22.Equals(rhs.M22);
		}

		/// <summary>
		/// Compares two matrices for inequality
		/// </summary>
		public static bool operator !=(Matrix3x3 lhs, Matrix3x3 rhs)
		{
			return !lhs.M00.Equals(rhs.M00) || !lhs.M01.Equals(rhs.M01) || !lhs.M02.Equals(rhs.M02) ||
				   !lhs.M10.Equals(rhs.M10) || !lhs.M11.Equals(rhs.M11) || !lhs.M12.Equals(rhs.M12) ||
				   !lhs.M20.Equals(rhs.M20) || !lhs.M21.Equals(rhs.M21) || !lhs.M22.Equals(rhs.M22);
		}

		/// <summary>
		/// Multiplies two matrices
		/// </summary>
		public static Matrix3x3 operator *(Matrix3x3 lhs, Matrix3x3 rhs)
		{
			return new Matrix3x3()
				{
					M00 = (lhs.M00 * rhs.M00 + lhs.M01 * rhs.M10 + lhs.M02 * rhs.M20),
					M01 = (lhs.M00 * rhs.M01 + lhs.M01 * rhs.M11 + lhs.M02 * rhs.M21),
					M02 = (lhs.M00 * rhs.M02 + lhs.M01 * rhs.M12 + lhs.M02 * rhs.M22),
					M10 = (lhs.M10 * rhs.M00 + lhs.M11 * rhs.M10 + lhs.M12 * rhs.M20),
					M11 = (lhs.M10 * rhs.M01 + lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21),
					M12 = (lhs.M10 * rhs.M02 + lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22),
					M20 = (lhs.M20 * rhs.M00 + lhs.M21 * rhs.M10 + lhs.M22 * rhs.M20),
					M21 = (lhs.M20 * rhs.M01 + lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21),
					M22 = (lhs.M20 * rhs.M02 + lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22),
				};
		}

		/// <summary>
		/// Multiplies the matrix by a vector
		/// </summary>
		public static Vector3 operator *(Matrix3x3 lhs, Vector3 v)
		{
			return new Vector3()
				{
					X = lhs.M00 * v.X + lhs.M01 * v.Y + lhs.M02 * v.Z,
					Y = lhs.M10 * v.X + lhs.M11 * v.Y + lhs.M12 * v.Z,
					Z = lhs.M20 * v.X + lhs.M21 * v.Y + lhs.M22 * v.Z
				};
		}

		/// <summary>
		/// Multiplies two matrices
		/// </summary>
		public static Matrix3x3 operator *(Matrix3x3 m, float f)
		{
			return new Matrix3x3()
				{
					M00 = m.M00 * f,
					M01 = m.M01 * f,
					M02 = m.M02 * f,
					M10 = m.M10 * f,
					M11 = m.M11 * f,
					M12 = m.M12 * f,
					M20 = m.M20 * f,
					M21 = m.M21 * f,
					M22 = m.M22 * f,
				};
		}

		/// <summary>
		/// Adds two matrices
		/// </summary>
		public static Matrix3x3 operator +(Matrix3x3 lhs, Matrix3x3 rhs)
		{
			return new Matrix3x3()
				{
					M00 = lhs.M00 + rhs.M00,
					M01 = lhs.M01 + rhs.M01,
					M02 = lhs.M02 + rhs.M02,
					M10 = lhs.M10 + rhs.M10,
					M11 = lhs.M11 + rhs.M11,
					M12 = lhs.M12 + rhs.M12,
					M20 = lhs.M20 + rhs.M20,
					M21 = lhs.M21 + rhs.M21,
					M22 = lhs.M22 + rhs.M22,
				};
		}

		/// <summary>
		/// Returns the determinant of the matrix
		/// </summary>
		/// <returns></returns>
		public float Determinant()
		{
			return
				this.M00 * this.M11 * this.M22 +
				this.M01 * this.M12 * this.M20 +
				this.M02 * this.M10 * this.M21 +
				this.M20 * this.M11 * this.M02 +
				this.M21 * this.M12 * this.M00 +
				this.M22 * this.M10 * this.M01;
		}

		/// <summary>
		/// Returns the transpose of a matrix
		/// </summary>
		public static Matrix3x3 Transpose(Matrix3x3 m)
		{
			return new Matrix3x3()
				{
					M00 = m.M00,
					M01 = m.M10,
					M02 = m.M20,
					M10 = m.M01,
					M11 = m.M11,
					M12 = m.M21,
					M20 = m.M02,
					M21 = m.M12,
					M22 = m.M22,
				};
		}

		/// <summary>
		/// Returns the transpose of this matrix
		/// </summary>
		public Matrix3x3 Transpose()
		{
			return Matrix3x3.Transpose(this);
		}

		/// <summary>
		/// Inverses a matrix
		/// </summary>
		public static Matrix3x3 Inverse(Matrix3x3 m)
		{
			var determinant = m.Determinant();

			return new Matrix3x3()
				{
					M00 = (m.M11 * m.M22 - m.M12 * m.M21) / determinant,
					M01 = (m.M02 * m.M21 - m.M01 * m.M22) / determinant,
					M02 = (m.M01 * m.M12 - m.M11 * m.M02) / determinant,
					M10 = (m.M12 * m.M20 - m.M22 * m.M10) / determinant,
					M11 = (m.M00 * m.M22 - m.M20 * m.M02) / determinant,
					M12 = (m.M02 * m.M10 - m.M12 * m.M00) / determinant,
					M20 = (m.M10 * m.M21 - m.M20 * m.M11) / determinant,
					M21 = (m.M01 * m.M20 - m.M21 * m.M00) / determinant,
					M22 = (m.M00 * m.M11 - m.M10 * m.M01) / determinant,
				};
		}

		/// <summary>
		/// Returns the inverse of the matrix
		/// </summary>
		/// <returns></returns>
		public Matrix3x3 Inverse()
		{
			return Matrix3x3.Inverse(this);
		}

		/// <summary>
		/// Scales the matrix with a vector
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public Matrix3x3 Scale(Vector3 v)
		{
			return new Matrix3x3()
				{
					M00 = v.X,
					M01 = 0.0f,
					M02 = 0.0f,
					M10 = 0.0f,
					M11 = v.Y,
					M12 = 0.0f,
					M20 = 0.0f,
					M21 = 0.0f,
					M22 = v.Z,
				};
		}

		/// <summary>
		/// Compares the matrix to an object
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Matrix3x3)
			{
				return this == (Matrix3x3)obj;
			}

			return false;
		}

		/// <summary>
		/// Gets the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return this.M00.GetHashCode() ^ this.M01.GetHashCode() ^ this.M02.GetHashCode() ^
				this.M10.GetHashCode() ^ this.M11.GetHashCode() ^ this.M12.GetHashCode() ^
				this.M20.GetHashCode() ^ this.M21.GetHashCode() ^ this.M22.GetHashCode();
		}

		/// <summary>
		/// Builds a string showing all the values of the matrix in a row x column fashion
		/// </summary>
		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}]\n[{3}, {4}, {5}]\n[{6}, {7}, {8}]",
			                     M00, M01, M02, M10, M11, M12, M20, M21, M22);
		}

		#endregion

		#region IEquatable Implementation

		/// <summary>
		/// Compares this instance to another matrix
		/// </summary>
		public bool Equals(Matrix3x3 other)
		{
			return this == other;
		}

		#endregion
	}
}
