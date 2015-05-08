using System;

namespace Karen90MmoFramework.Quantum
{
	public struct Matrix4x4 : IEquatable<Matrix4x4>
	{
		#region Constants and Fields

		public float M00;
		public float M01;
		public float M02;
		public float M03;

		public float M10;
		public float M11;
		public float M12;
		public float M13;

		public float M20;
		public float M21;
		public float M22;
		public float M23;

		public float M30;
		public float M31;
		public float M32;
		public float M33;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the value at a specific row, column
		/// </summary>
		public float this[int row, int column]
		{
			get
			{
				return this[row * 4 + column];
			}
		}

		/// <summary>
		/// Gets the value at a specific index
		/// </summary>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return M00;
					case 1:
						return M01;
					case 2:
						return M02;
					case 3:
						return M03;
					case 4:
						return M10;
					case 5:
						return M11;
					case 6:
						return M12;
					case 7:
						return M13;
					case 8:
						return M20;
					case 9:
						return M21;
					case 10:
						return M22;
					case 11:
						return M23;
					case 12:
						return M30;
					case 13:
						return M31;
					case 14:
						return M32;
					case 15:
						return M33;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		/// <summary>
		/// Gets or sets the up vector of the Matrix
		/// </summary>
		public Vector3 Up
		{
			get
			{
				return new Vector3()
					{
						X = this.M10,
						Y = this.M11,
						Z = this.M12,
					};
			}
			set
			{
				this.M10 = value.X;
				this.M11 = value.Y;
				this.M12 = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets the down vector of the Matrix
		/// </summary>
		public Vector3 Down
		{
			get
			{
				return new Vector3()
					{
						X = -this.M10,
						Y = -this.M11,
						Z = -this.M12,
					};
			}
			set
			{
				this.M10 = -value.X;
				this.M11 = -value.Y;
				this.M12 = -value.Z;
			}
		}

		/// <summary>
		/// Gets or sets the right vector of the Matrix
		/// </summary>
		public Vector3 Right
		{
			get
			{
				return new Vector3()
					{
						X = this.M00,
						Y = this.M01,
						Z = this.M02,
					};
			}
			set
			{
				this.M00 = value.X;
				this.M01 = value.Y;
				this.M02 = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets the left vector of the Matrix
		/// </summary>
		public Vector3 Left
		{
			get
			{
				return new Vector3()
					{
						X = -this.M00,
						Y = -this.M01,
						Z = -this.M02,
					};
			}
			set
			{
				this.M00 = -value.X;
				this.M01 = -value.Y;
				this.M02 = -value.Z;
			}
		}

		/// <summary>
		/// Gets or sets the forward vector of the Matrix
		/// </summary>
		public Vector3 Forward
		{
			get
			{
				return new Vector3()
					{
						X = this.M20,
						Y = this.M21,
						Z = this.M22,
					};
			}
			set
			{
				this.M20 = value.X;
				this.M21 = value.Y;
				this.M22 = value.Z;
			}
		}

		/// <summary>
		/// Gets or sets the backward vector of the Matrix
		/// </summary>
		public Vector3 Backward
		{
			get
			{
				return new Vector3()
					{
						X = -this.M20,
						Y = -this.M21,
						Z = -this.M22,
					};
			}
			set
			{
				this.M20 = -value.X;
				this.M21 = -value.Y;
				this.M22 = -value.Z;
			}
		}

		/// <summary>
		/// Gets the 0 matrix
		/// </summary>
		public static Matrix4x4 Zero
		{
			get
			{
				return new Matrix4x4()
					{
						M00 = 0.0f,
						M01 = 0.0f,
						M02 = 0.0f,
						M03 = 0.0f,
						M10 = 0.0f,
						M11 = 0.0f,
						M12 = 0.0f,
						M13 = 0.0f,
						M20 = 0.0f,
						M21 = 0.0f,
						M22 = 0.0f,
						M23 = 0.0f,
						M30 = 0.0f,
						M31 = 0.0f,
						M32 = 0.0f,
						M33 = 0.0f
					};
			}
		}

		/// <summary>
		/// Gets the identity matrix
		/// </summary>
		public static Matrix4x4 Identity
		{
			get
			{
				return new Matrix4x4()
				{
					M00 = 1.0f,
					M01 = 0.0f,
					M02 = 0.0f,
					M03 = 0.0f,
					M10 = 0.0f,
					M11 = 1.0f,
					M12 = 0.0f,
					M13 = 0.0f,
					M20 = 0.0f,
					M21 = 0.0f,
					M22 = 1.0f,
					M23 = 0.0f,
					M30 = 0.0f,
					M31 = 0.0f,
					M32 = 0.0f,
					M33 = 1.0f
				};
			}
		}

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new <see cref="Matrix4x4"/> instance.
		/// </summary>
		public Matrix4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
		{
			M00 = m00;
			M01 = m01;
			M02 = m02;
			M03 = m03;
			M10 = m10;
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M20 = m20;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M30 = m30;
			M31 = m31;
			M32 = m32;
			M33 = m33;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Compares two matrices for equality
		/// </summary>
		public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return lhs.M00.Equals(rhs.M00) && lhs.M01.Equals(rhs.M01) && lhs.M02.Equals(rhs.M02) && lhs.M03.Equals(rhs.M03) &&
			       lhs.M10.Equals(rhs.M10) && lhs.M11.Equals(rhs.M11) && lhs.M12.Equals(rhs.M12) && lhs.M13.Equals(rhs.M13) &&
			       lhs.M20.Equals(rhs.M20) && lhs.M21.Equals(rhs.M21) && lhs.M22.Equals(rhs.M22) && lhs.M23.Equals(rhs.M23) &&
			       lhs.M30.Equals(rhs.M30) && lhs.M31.Equals(rhs.M31) && lhs.M32.Equals(rhs.M32) && lhs.M33.Equals(rhs.M33);
		}

		/// <summary>
		/// Compares two matrices for inequality
		/// </summary>
		public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return !lhs.M00.Equals(rhs.M00) || !lhs.M01.Equals(rhs.M01) || !lhs.M02.Equals(rhs.M02) || !lhs.M03.Equals(rhs.M03) ||
				   !lhs.M10.Equals(rhs.M10) || !lhs.M11.Equals(rhs.M11) || !lhs.M12.Equals(rhs.M12) || !lhs.M13.Equals(rhs.M13) ||
				   !lhs.M20.Equals(rhs.M20) || !lhs.M21.Equals(rhs.M21) || !lhs.M22.Equals(rhs.M22) || !lhs.M23.Equals(rhs.M23) ||
				   !lhs.M30.Equals(rhs.M30) || !lhs.M31.Equals(rhs.M31) || !lhs.M32.Equals(rhs.M32) || !lhs.M33.Equals(rhs.M33);
		}

		/// <summary>
		/// Negates a matrix
		/// </summary>
		public static Matrix4x4 operator -(Matrix4x4 m)
		{
			return new Matrix4x4()
				{
					M00 = -m.M00,
					M01 = -m.M01,
					M02 = -m.M02,
					M03 = -m.M03,
					M10 = -m.M10,
					M11 = -m.M11,
					M12 = -m.M12,
					M13 = -m.M13,
					M20 = -m.M20,
					M21 = -m.M21,
					M22 = -m.M22,
					M23 = -m.M23,
					M30 = -m.M30,
					M31 = -m.M31,
					M32 = -m.M32,
					M33 = -m.M33
				};
		}

		/// <summary>
		/// Negates a matrix
		/// </summary>
		public static void Negate(ref Matrix4x4 m, out Matrix4x4 mResult)
		{
			mResult = new Matrix4x4()
				{
					M00 = -m.M00,
					M01 = -m.M01,
					M02 = -m.M02,
					M03 = -m.M03,
					M10 = -m.M10,
					M11 = -m.M11,
					M12 = -m.M12,
					M13 = -m.M13,
					M20 = -m.M20,
					M21 = -m.M21,
					M22 = -m.M22,
					M23 = -m.M23,
					M30 = -m.M30,
					M31 = -m.M31,
					M32 = -m.M32,
					M33 = -m.M33
				};
		}

		/// <summary>
		/// Adds two matrices
		/// </summary>
		public static Matrix4x4 operator +(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return new Matrix4x4()
				{
					M00 = lhs.M00 + rhs.M00,
					M01 = lhs.M01 + rhs.M01,
					M02 = lhs.M02 + rhs.M02,
					M03 = lhs.M03 + rhs.M03,
					M10 = lhs.M10 + rhs.M10,
					M11 = lhs.M11 + rhs.M11,
					M12 = lhs.M12 + rhs.M12,
					M13 = lhs.M13 + rhs.M13,
					M20 = lhs.M20 + rhs.M20,
					M21 = lhs.M21 + rhs.M21,
					M22 = lhs.M22 + rhs.M22,
					M23 = lhs.M23 + rhs.M23,
					M30 = lhs.M30 + rhs.M30,
					M31 = lhs.M31 + rhs.M31,
					M32 = lhs.M32 + rhs.M32,
					M33 = lhs.M33 + rhs.M33
				};
		}

		/// <summary>
		/// Adds two matrices
		/// </summary>
		public static void Add(ref Matrix4x4 lhs, ref Matrix4x4 rhs, out Matrix4x4 mResult)
		{
			mResult = new Matrix4x4()
				{
					M00 = lhs.M00 + rhs.M00,
					M01 = lhs.M01 + rhs.M01,
					M02 = lhs.M02 + rhs.M02,
					M03 = lhs.M03 + rhs.M03,
					M10 = lhs.M10 + rhs.M10,
					M11 = lhs.M11 + rhs.M11,
					M12 = lhs.M12 + rhs.M12,
					M13 = lhs.M13 + rhs.M13,
					M20 = lhs.M20 + rhs.M20,
					M21 = lhs.M21 + rhs.M21,
					M22 = lhs.M22 + rhs.M22,
					M23 = lhs.M23 + rhs.M23,
					M30 = lhs.M30 + rhs.M30,
					M31 = lhs.M31 + rhs.M31,
					M32 = lhs.M32 + rhs.M32,
					M33 = lhs.M33 + rhs.M33
				};
		}

		/// <summary>
		/// Substracts two matrices
		/// </summary>
		public static Matrix4x4 operator -(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return new Matrix4x4()
				{
					M00 = lhs.M00 - rhs.M00,
					M01 = lhs.M01 - rhs.M01,
					M02 = lhs.M02 - rhs.M02,
					M03 = lhs.M03 - rhs.M03,
					M10 = lhs.M10 - rhs.M10,
					M11 = lhs.M11 - rhs.M11,
					M12 = lhs.M12 - rhs.M12,
					M13 = lhs.M13 - rhs.M13,
					M20 = lhs.M20 - rhs.M20,
					M21 = lhs.M21 - rhs.M21,
					M22 = lhs.M22 - rhs.M22,
					M23 = lhs.M23 - rhs.M23,
					M30 = lhs.M30 - rhs.M30,
					M31 = lhs.M31 - rhs.M31,
					M32 = lhs.M32 - rhs.M32,
					M33 = lhs.M33 - rhs.M33
				};
		}

		/// <summary>
		/// Substracts two matrices
		/// </summary>
		public static void Substract(ref Matrix4x4 lhs, ref Matrix4x4 rhs, out Matrix4x4 mResult)
		{
			mResult = new Matrix4x4()
				{
					M00 = lhs.M00 - rhs.M00,
					M01 = lhs.M01 - rhs.M01,
					M02 = lhs.M02 - rhs.M02,
					M03 = lhs.M03 - rhs.M03,
					M10 = lhs.M10 - rhs.M10,
					M11 = lhs.M11 - rhs.M11,
					M12 = lhs.M12 - rhs.M12,
					M13 = lhs.M13 - rhs.M13,
					M20 = lhs.M20 - rhs.M20,
					M21 = lhs.M21 - rhs.M21,
					M22 = lhs.M22 - rhs.M22,
					M23 = lhs.M23 - rhs.M23,
					M30 = lhs.M30 - rhs.M30,
					M31 = lhs.M31 - rhs.M31,
					M32 = lhs.M32 - rhs.M32,
					M33 = lhs.M33 - rhs.M33
				};
		}

		/// <summary>
		/// Multiplies two matrices
		/// </summary>
		public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return new Matrix4x4()
				{
					M00 = (lhs.M00 * rhs.M00 + lhs.M01 * rhs.M10 + lhs.M02 * rhs.M20 + lhs.M03 * rhs.M30),
					M01 = (lhs.M00 * rhs.M01 + lhs.M01 * rhs.M11 + lhs.M02 * rhs.M21 + lhs.M03 * rhs.M31),
					M02 = (lhs.M00 * rhs.M02 + lhs.M01 * rhs.M12 + lhs.M02 * rhs.M22 + lhs.M03 * rhs.M32),
					M03 = (lhs.M00 * rhs.M03 + lhs.M01 * rhs.M13 + lhs.M02 * rhs.M23 + lhs.M03 * rhs.M33),
					M10 = (lhs.M10 * rhs.M00 + lhs.M11 * rhs.M10 + lhs.M12 * rhs.M20 + lhs.M13 * rhs.M30),
					M11 = (lhs.M10 * rhs.M01 + lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31),
					M12 = (lhs.M10 * rhs.M02 + lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32),
					M13 = (lhs.M10 * rhs.M03 + lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33),
					M20 = (lhs.M20 * rhs.M00 + lhs.M21 * rhs.M10 + lhs.M22 * rhs.M20 + lhs.M23 * rhs.M30),
					M21 = (lhs.M20 * rhs.M01 + lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31),
					M22 = (lhs.M20 * rhs.M02 + lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32),
					M23 = (lhs.M20 * rhs.M03 + lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33),
					M30 = (lhs.M30 * rhs.M00 + lhs.M31 * rhs.M10 + lhs.M32 * rhs.M20 + lhs.M33 * rhs.M30),
					M31 = (lhs.M30 * rhs.M01 + lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31),
					M32 = (lhs.M30 * rhs.M02 + lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32),
					M33 = (lhs.M30 * rhs.M03 + lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33)
				};
		}

		/// <summary>
		/// Multiplies two matrices
		/// </summary>
		public static void Multiply(ref Matrix4x4 lhs, ref Matrix4x4 rhs, out Matrix4x4 mResult)
		{
			mResult = new Matrix4x4()
				{
					M00 = (lhs.M00 * rhs.M00 + lhs.M01 * rhs.M10 + lhs.M02 * rhs.M20 + lhs.M03 * rhs.M30),
					M01 = (lhs.M00 * rhs.M01 + lhs.M01 * rhs.M11 + lhs.M02 * rhs.M21 + lhs.M03 * rhs.M31),
					M02 = (lhs.M00 * rhs.M02 + lhs.M01 * rhs.M12 + lhs.M02 * rhs.M22 + lhs.M03 * rhs.M32),
					M03 = (lhs.M00 * rhs.M03 + lhs.M01 * rhs.M13 + lhs.M02 * rhs.M23 + lhs.M03 * rhs.M33),
					M10 = (lhs.M10 * rhs.M00 + lhs.M11 * rhs.M10 + lhs.M12 * rhs.M20 + lhs.M13 * rhs.M30),
					M11 = (lhs.M10 * rhs.M01 + lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31),
					M12 = (lhs.M10 * rhs.M02 + lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32),
					M13 = (lhs.M10 * rhs.M03 + lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33),
					M20 = (lhs.M20 * rhs.M00 + lhs.M21 * rhs.M10 + lhs.M22 * rhs.M20 + lhs.M23 * rhs.M30),
					M21 = (lhs.M20 * rhs.M01 + lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31),
					M22 = (lhs.M20 * rhs.M02 + lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32),
					M23 = (lhs.M20 * rhs.M03 + lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33),
					M30 = (lhs.M30 * rhs.M00 + lhs.M31 * rhs.M10 + lhs.M32 * rhs.M20 + lhs.M33 * rhs.M30),
					M31 = (lhs.M30 * rhs.M01 + lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31),
					M32 = (lhs.M30 * rhs.M02 + lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32),
					M33 = (lhs.M30 * rhs.M03 + lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33)
				};
		}

		/// <summary>
		/// Multiplies a matrix by a scaler
		/// </summary>
		public static Matrix4x4 operator *(Matrix4x4 m, float f)
		{
			return new Matrix4x4()
				{
					M00 = m.M00 * f,
					M01 = m.M01 * f,
					M02 = m.M02 * f,
					M03 = m.M03 * f,
					M10 = m.M10 * f,
					M11 = m.M11 * f,
					M12 = m.M12 * f,
					M13 = m.M13 * f,
					M20 = m.M20 * f,
					M21 = m.M21 * f,
					M22 = m.M22 * f,
					M23 = m.M23 * f,
					M30 = m.M30 * f,
					M31 = m.M31 * f,
					M32 = m.M32 * f,
					M33 = m.M33 * f
				};
		}

		/// <summary>
		/// Multiplies a matrix by a scaler
		/// </summary>
		public static void Multiply(ref Matrix4x4 m, float f, out Matrix4x4 mResult)
		{
			mResult = new Matrix4x4()
				{
					M00 = m.M00 * f,
					M01 = m.M01 * f,
					M02 = m.M02 * f,
					M03 = m.M03 * f,
					M10 = m.M10 * f,
					M11 = m.M11 * f,
					M12 = m.M12 * f,
					M13 = m.M13 * f,
					M20 = m.M20 * f,
					M21 = m.M21 * f,
					M22 = m.M22 * f,
					M23 = m.M23 * f,
					M30 = m.M30 * f,
					M31 = m.M31 * f,
					M32 = m.M32 * f,
					M33 = m.M33 * f
				};
		}

		/// <summary>
		/// Multiplies a matrix by a scaler
		/// </summary>
		public static Matrix4x4 operator *(float f, Matrix4x4 m)
		{
			return new Matrix4x4()
			{
				M00 = m.M00 * f,
				M01 = m.M01 * f,
				M02 = m.M02 * f,
				M03 = m.M03 * f,
				M10 = m.M10 * f,
				M11 = m.M11 * f,
				M12 = m.M12 * f,
				M13 = m.M13 * f,
				M20 = m.M20 * f,
				M21 = m.M21 * f,
				M22 = m.M22 * f,
				M23 = m.M23 * f,
				M30 = m.M30 * f,
				M31 = m.M31 * f,
				M32 = m.M32 * f,
				M33 = m.M33 * f
			};
		}

		/// <summary>
		/// Multiplies a matrix by a scaler
		/// </summary>
		public static void Multiply(float f, ref Matrix4x4 m, out Matrix4x4 mResult)
		{
			mResult = new Matrix4x4()
				{
					M00 = m.M00 * f,
					M01 = m.M01 * f,
					M02 = m.M02 * f,
					M03 = m.M03 * f,
					M10 = m.M10 * f,
					M11 = m.M11 * f,
					M12 = m.M12 * f,
					M13 = m.M13 * f,
					M20 = m.M20 * f,
					M21 = m.M21 * f,
					M22 = m.M22 * f,
					M23 = m.M23 * f,
					M30 = m.M30 * f,
					M31 = m.M31 * f,
					M32 = m.M32 * f,
					M33 = m.M33 * f
				};
		}

		/// <summary>
		/// Creates a translation matrix
		/// </summary>
		public static Matrix4x4 CreateTranslation(Vector3 t)
		{
			return new Matrix4x4()
				{
					M00 = 1f,
					M01 = 0f,
					M02 = 0f,
					M03 = 0f,
					M10 = 0f,
					M11 = 1f,
					M12 = 0f,
					M13 = 0f,
					M20 = 0f,
					M21 = 0f,
					M22 = 1f,
					M23 = 0f,
					M30 = t.X,
					M31 = t.Y,
					M32 = t.Z,
					M33 = 1f
				};
		}
		
		/// <summary>
		/// Creates a rotation matrix
		/// </summary>
		public static Matrix4x4 CreateRotation(Quaternion r)
		{
			var xx = r.X * r.X;
			var yy = r.Y * r.Y;
			var zz = r.Z * r.Z;
			var xy = r.X * r.Y;
			var zw = r.Z * r.W;
			var zx = r.Z * r.X;
			var yw = r.Y * r.W;
			var yz = r.Y * r.Z;
			var xw = r.X * r.W;

			return new Matrix4x4()
				{
					M00 = (1.0f - 2.0f * (yy + zz)),
					M01 = (2.0f * (xy + zw)),
					M02 = (2.0f * (zx - yw)),
					M03 = 0.0f,
					M10 = (2.0f * (xy - zw)),
					M11 = (1.0f - 2.0f * (zz + xx)),
					M12 = (2.0f * (yz + xw)),
					M13 = 0.0f,
					M20 = (2.0f * (zx + yw)),
					M21 = (2.0f * (yz - xw)),
					M22 = (1.0f - 2.0f * (yy + xx)),
					M23 = 0.0f,
					M30 = 0.0f,
					M31 = 0.0f,
					M32 = 0.0f,
					M33 = 1f
				};
		}

		/// <summary>
		/// Creates a rotation matrix around the x-axis
		/// </summary>
		public static Matrix4x4 CreateRotationX(float angle)
		{
			var cosr = Mathf.Cos(angle * Mathf.DEG2_RAD);
			var sinr = Mathf.Sin(angle * Mathf.DEG2_RAD);
			
			return new Matrix4x4()
			{
				M00 = 1f,
				M01 = 0f,
				M02 = 0f,
				M03 = 0f,
				M10 = 0f,
				M11 = cosr,
				M12 = sinr,
				M13 = 0f,
				M20 = 0f,
				M21 = -sinr,
				M22 = cosr,
				M23 = 0f,
				M30 = 0f,
				M31 = 0f,
				M32 = 0f,
				M33 = 1f
			};
		}

		/// <summary>
		/// Creates a rotation matrix around the y-axis
		/// </summary>
		public static Matrix4x4 CreateRotationY(float angle)
		{
			var cosr = Mathf.Cos(angle * Mathf.DEG2_RAD);
			var sinr = Mathf.Sin(angle * Mathf.DEG2_RAD);

			return new Matrix4x4()
			{
				M00 = cosr,
				M01 = 0f,
				M02 = -sinr,
				M03 = 0f,
				M10 = 0f,
				M11 = 1f,
				M12 = 0f,
				M13 = 0f,
				M20 = sinr,
				M21 = 0f,
				M22 = cosr,
				M23 = 0f,
				M30 = 0f,
				M31 = 0f,
				M32 = 0f,
				M33 = 1f
			};
		}

		/// <summary>
		/// Creates a rotation matrix around the z-axis
		/// </summary>
		public static Matrix4x4 CreateRotationZ(float angle)
		{
			var cosr = Mathf.Cos(angle * Mathf.DEG2_RAD);
			var sinr = Mathf.Sin(angle * Mathf.DEG2_RAD);

			return new Matrix4x4()
			{
				M00 = cosr,
				M01 = sinr,
				M02 = 0f,
				M03 = 0f,
				M10 = -sinr,
				M11 = cosr,
				M12 = 0f,
				M13 = 0f,
				M20 = 0f,
				M21 = 0f,
				M22 = 1f,
				M23 = 0f,
				M30 = 0f,
				M31 = 0f,
				M32 = 0f,
				M33 = 1f
			};
		}

		/// <summary>
		/// Creates a scaling matrix
		/// </summary>
		public static Matrix4x4 CreateScale(Vector3 s)
		{
			return new Matrix4x4()
				{
					M00 = s.X,
					M01 = 0.0f,
					M02 = 0.0f,
					M03 = 0.0f,
					M10 = 0.0f,
					M11 = s.Y,
					M12 = 0.0f,
					M13 = 0.0f,
					M20 = 0.0f,
					M21 = 0.0f,
					M22 = s.Z,
					M23 = 0.0f,
					M30 = 0.0f,
					M31 = 0.0f,
					M32 = 0.0f,
					M33 = 1.0f
				};
		}

		/// <summary>
		/// Transforms a matrix by rotating it
		/// </summary>
		public static Matrix4x4 Transform(Matrix4x4 m, Quaternion r)
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
			var val0 = 1f - qyy2 - qzz2;
			var val1 = qxy2 - qwz2;
			var val2 = qxz2 + qwy2;
			var val3 = qxy2 + qwz2;
			var val4 = 1f - qxx2 - qzz2;
			var val5 = qyz2 - qwx2;
			var val6 = qxz2 - qwy2;
			var val7 = qyz2 + qwx2;
			var val8 = 1f - qxx2 - qyy2;

			return new Matrix4x4()
				{
					M00 = (m.M11 * val0 + m.M12 * val1 + m.M13 * val2),
					M01 = (m.M11 * val3 + m.M12 * val4 + m.M13 * val5),
					M02 = (m.M11 * val6 + m.M12 * val7 + m.M13 * val8),
					M03 = m.M03,
					M10 = (m.M21 * val0 + m.M22 * val1 + m.M23 * val2),
					M11 = (m.M21 * val3 + m.M22 * val4 + m.M23 * val5),
					M12 = (m.M21 * val6 + m.M22 * val7 + m.M23 * val8),
					M13 = m.M13,
					M20 = (m.M31 * val0 + m.M32 * val1 + m.M33 * val2),
					M21 = (m.M31 * val3 + m.M32 * val4 + m.M33 * val5),
					M22 = (m.M31 * val6 + m.M32 * val7 + m.M33 * val8),
					M23 = m.M23,
					M30 = (m.M30 * val0 + m.M31 * val1 + m.M32 * val2),
					M31 = (m.M30 * val3 + m.M31 * val4 + m.M32 * val5),
					M32 = (m.M30 * val6 + m.M31 * val7 + m.M32 * val8),
					M33 = m.M33
				};
		}

		/// <summary>
		/// Creates a matrix that rotates around an axis
		/// </summary>
		public static Matrix4x4 AxisAngle(Vector3 axis, float angle)
		{
			var x = axis.X;
			var y = axis.Y;
			var z = axis.Z;
			var sinr = Mathf.Sin(angle * Mathf.DEG2_RAD);
			var cosr = Mathf.Cos(angle * Mathf.DEG2_RAD);
			var xx = x * x;
			var yy = y * y;
			var zz = z * z;
			var xy = x * y;
			var xz = x * z;
			var yz = y * z;

			return new Matrix4x4()
				{
					M00 = xx + cosr * (1f - xx),
					M01 = (xy - cosr * xy + sinr * z),
					M02 = (xz - cosr * xz - sinr * y),
					M03 = 0.0f,
					M10 = (xy - cosr * xy - sinr * z),
					M11 = yy + cosr * (1f - yy),
					M12 = (yz - cosr * yz + sinr * x),
					M13 = 0.0f,
					M20 = (xz - cosr * xz + sinr * y),
					M21 = (yz - cosr * yz - sinr * x),
					M22 = zz + cosr * (1f - zz),
					M23 = 0.0f,
					M30 = 0.0f,
					M31 = 0.0f,
					M32 = 0.0f,
					M33 = 1f
				};
		}

		/// <summary>
		/// Creates a world matrix
		/// </summary>
		public static Matrix4x4 World(Vector3 position, Vector3 forward, Vector3 up)
		{
			var nF = Vector3.Normalize(forward);
			var nL = Vector3.Normalize(Vector3.Cross(up, nF));
			var nD = Vector3.Cross(nF, nL);

			return new Matrix4x4()
				{
					M00 = nL.X,
					M01 = nL.Y,
					M02 = nL.Z,
					M03 = 0.0f,
					M10 = nD.X,
					M11 = nD.Y,
					M12 = nD.Z,
					M13 = 0.0f,
					M20 = nF.X,
					M21 = nF.Y,
					M22 = nF.Z,
					M23 = 0.0f,
					M30 = position.X,
					M31 = position.Y,
					M32 = position.Z,
					M33 = 1f
				};
		}

		/// <summary>
		/// Returns the determinant of the matrix
		/// </summary>
		/// <returns></returns>
		public float Determinant()
		{
			return
				this.M00 * this.M11 * this.M22 * this.M33 +
				this.M00 * this.M12 * this.M23 * this.M31 +
				this.M00 * this.M13 * this.M21 * this.M32 +
				this.M01 * this.M10 * this.M23 * this.M32 +
				this.M01 * this.M12 * this.M20 * this.M33 +
				this.M01 * this.M13 * this.M22 * this.M30 +
				this.M02 * this.M10 * this.M21 * this.M33 +
				this.M02 * this.M11 * this.M23 * this.M30 +
				this.M02 * this.M13 * this.M20 * this.M31 +
				this.M03 * this.M10 * this.M22 * this.M31 +
				this.M03 * this.M11 * this.M20 * this.M32 +
				this.M03 * this.M12 * this.M21 * this.M30 -
				this.M00 * this.M11 * this.M23 * this.M32 -
				this.M00 * this.M12 * this.M21 * this.M33 -
				this.M00 * this.M13 * this.M22 * this.M31 -
				this.M01 * this.M10 * this.M22 * this.M33 -
				this.M01 * this.M12 * this.M23 * this.M30 -
				this.M01 * this.M13 * this.M20 * this.M32 -
				this.M02 * this.M10 * this.M23 * this.M31 -
				this.M02 * this.M11 * this.M20 * this.M33 -
				this.M02 * this.M13 * this.M21 * this.M30 -
				this.M03 * this.M10 * this.M21 * this.M32 -
				this.M03 * this.M11 * this.M22 * this.M30 -
				this.M03 * this.M12 * this.M20 * this.M31;
		}

		/// <summary>
		/// Returns the transpose of a matrix
		/// </summary>
		public static Matrix4x4 Transpose(Matrix4x4 m)
		{
			return new Matrix4x4()
				{
					M00 = m.M00,
					M01 = m.M10,
					M02 = m.M20,
					M03 = m.M30,
					M10 = m.M01,
					M11 = m.M11,
					M12 = m.M21,
					M13 = m.M31,
					M20 = m.M02,
					M21 = m.M12,
					M22 = m.M22,
					M23 = m.M32,
					M30 = m.M03,
					M31 = m.M13,
					M32 = m.M23,
					M33 = m.M33
				};
		}

		/// <summary>
		/// Returns the transpose of a matrix
		/// </summary>
		public static void Transpose(ref Matrix4x4 m, out Matrix4x4 mResult)
		{
			mResult = new Matrix4x4()
				{
					M00 = m.M00,
					M01 = m.M10,
					M02 = m.M20,
					M03 = m.M30,
					M10 = m.M01,
					M11 = m.M11,
					M12 = m.M21,
					M13 = m.M31,
					M20 = m.M02,
					M21 = m.M12,
					M22 = m.M22,
					M23 = m.M32,
					M30 = m.M03,
					M31 = m.M13,
					M32 = m.M23,
					M33 = m.M33
				};
		}

		/// <summary>
		/// Returns the transpose of this matrix
		/// </summary>
		public Matrix4x4 Transpose()
		{
			return Matrix4x4.Transpose(this);
		}

		/// <summary>
		/// Inverses a matrix
		/// </summary>
		public static Matrix4x4 Inverse(Matrix4x4 m)
		{
			var determinant = m.Determinant();
			return new Matrix4x4()
				{
					M00 = (m.M11 * m.M22 * m.M33 + m.M12 * m.M23 * m.M31 + m.M13 * m.M21 * m.M32 - m.M11 * m.M23 * m.M32 - m.M12 * m.M21 * m.M33 - m.M13 * m.M22 * m.M31) / determinant,
					M01 = (m.M01 * m.M23 * m.M32 + m.M02 * m.M21 * m.M33 + m.M03 * m.M22 * m.M31 - m.M01 * m.M22 * m.M33 - m.M02 * m.M23 * m.M31 - m.M03 * m.M21 * m.M32) / determinant,
					M02 = (m.M01 * m.M12 * m.M33 + m.M02 * m.M13 * m.M31 + m.M03 * m.M11 * m.M32 - m.M01 * m.M13 * m.M32 - m.M02 * m.M11 * m.M33 - m.M03 * m.M12 * m.M31) / determinant,
					M03 = (m.M01 * m.M13 * m.M22 + m.M02 * m.M11 * m.M23 + m.M03 * m.M12 * m.M21 - m.M01 * m.M12 * m.M23 - m.M02 * m.M13 * m.M21 - m.M03 * m.M11 * m.M22) / determinant,
					M10 = (m.M10 * m.M23 * m.M32 + m.M12 * m.M20 * m.M33 + m.M13 * m.M22 * m.M30 - m.M10 * m.M22 * m.M33 - m.M12 * m.M23 * m.M30 - m.M13 * m.M20 * m.M32) / determinant,
					M11 = (m.M00 * m.M22 * m.M33 + m.M02 * m.M23 * m.M30 + m.M03 * m.M20 * m.M32 - m.M00 * m.M23 * m.M32 - m.M02 * m.M20 * m.M33 - m.M03 * m.M22 * m.M30) / determinant,
					M12 = (m.M00 * m.M13 * m.M32 + m.M02 * m.M10 * m.M33 + m.M03 * m.M12 * m.M30 - m.M00 * m.M12 * m.M33 - m.M02 * m.M13 * m.M30 - m.M03 * m.M10 * m.M32) / determinant,
					M13 = (m.M00 * m.M12 * m.M23 + m.M02 * m.M13 * m.M20 + m.M03 * m.M10 * m.M22 - m.M00 * m.M13 * m.M22 - m.M02 * m.M10 * m.M23 - m.M03 * m.M12 * m.M20) / determinant,
					M20 = (m.M10 * m.M21 * m.M33 + m.M11 * m.M23 * m.M30 + m.M13 * m.M20 * m.M31 - m.M10 * m.M23 * m.M31 - m.M11 * m.M20 * m.M33 - m.M13 * m.M21 * m.M30) / determinant,
					M21 = (m.M00 * m.M23 * m.M31 + m.M01 * m.M20 * m.M33 + m.M03 * m.M21 * m.M30 - m.M00 * m.M21 * m.M33 - m.M01 * m.M23 * m.M30 - m.M03 * m.M11 * m.M31) / determinant,
					M22 = (m.M00 * m.M11 * m.M33 + m.M01 * m.M13 * m.M30 + m.M03 * m.M10 * m.M31 - m.M00 * m.M13 * m.M31 - m.M01 * m.M10 * m.M33 - m.M03 * m.M11 * m.M30) / determinant,
					M23 = (m.M00 * m.M13 * m.M21 + m.M01 * m.M10 * m.M23 + m.M03 * m.M11 * m.M20 - m.M00 * m.M11 * m.M23 - m.M01 * m.M13 * m.M20 - m.M03 * m.M10 * m.M21) / determinant,
					M30 = (m.M10 * m.M22 * m.M31 + m.M11 * m.M20 * m.M32 + m.M12 * m.M21 * m.M30 - m.M10 * m.M21 * m.M32 - m.M11 * m.M22 * m.M30 - m.M23 * m.M20 * m.M31) / determinant,
					M31 = (m.M00 * m.M21 * m.M32 + m.M01 * m.M22 * m.M30 + m.M02 * m.M20 * m.M31 - m.M00 * m.M22 * m.M31 - m.M01 * m.M20 * m.M32 - m.M02 * m.M21 * m.M30) / determinant,
					M32 = (m.M00 * m.M12 * m.M31 + m.M01 * m.M10 * m.M32 + m.M02 * m.M11 * m.M30 - m.M00 * m.M11 * m.M32 - m.M01 * m.M12 * m.M30 - m.M02 * m.M10 * m.M31) / determinant,
					M33 = (m.M00 * m.M11 * m.M22 + m.M01 * m.M12 * m.M20 + m.M02 * m.M10 * m.M21 - m.M00 * m.M12 * m.M21 - m.M01 * m.M10 * m.M22 - m.M02 * m.M11 * m.M20) / determinant
				};
		}

		/// <summary>
		/// Inverses a matrix
		/// </summary>
		public static void Inverse(ref Matrix4x4 m, out Matrix4x4 mResult)
		{
			var determinant = m.Determinant();
			mResult = new Matrix4x4()
				{
					M00 = (m.M11 * m.M22 * m.M33 + m.M12 * m.M23 * m.M31 + m.M13 * m.M21 * m.M32 - m.M11 * m.M23 * m.M32 - m.M12 * m.M21 * m.M33 - m.M13 * m.M22 * m.M31) / determinant,
					M01 = (m.M01 * m.M23 * m.M32 + m.M02 * m.M21 * m.M33 + m.M03 * m.M22 * m.M31 - m.M01 * m.M22 * m.M33 - m.M02 * m.M23 * m.M31 - m.M03 * m.M21 * m.M32) / determinant,
					M02 = (m.M01 * m.M12 * m.M33 + m.M02 * m.M13 * m.M31 + m.M03 * m.M11 * m.M32 - m.M01 * m.M13 * m.M32 - m.M02 * m.M11 * m.M33 - m.M03 * m.M12 * m.M31) / determinant,
					M03 = (m.M01 * m.M13 * m.M22 + m.M02 * m.M11 * m.M23 + m.M03 * m.M12 * m.M21 - m.M01 * m.M12 * m.M23 - m.M02 * m.M13 * m.M21 - m.M03 * m.M11 * m.M22) / determinant,
					M10 = (m.M10 * m.M23 * m.M32 + m.M12 * m.M20 * m.M33 + m.M13 * m.M22 * m.M30 - m.M10 * m.M22 * m.M33 - m.M12 * m.M23 * m.M30 - m.M13 * m.M20 * m.M32) / determinant,
					M11 = (m.M00 * m.M22 * m.M33 + m.M02 * m.M23 * m.M30 + m.M03 * m.M20 * m.M32 - m.M00 * m.M23 * m.M32 - m.M02 * m.M20 * m.M33 - m.M03 * m.M22 * m.M30) / determinant,
					M12 = (m.M00 * m.M13 * m.M32 + m.M02 * m.M10 * m.M33 + m.M03 * m.M12 * m.M30 - m.M00 * m.M12 * m.M33 - m.M02 * m.M13 * m.M30 - m.M03 * m.M10 * m.M32) / determinant,
					M13 = (m.M00 * m.M12 * m.M23 + m.M02 * m.M13 * m.M20 + m.M03 * m.M10 * m.M22 - m.M00 * m.M13 * m.M22 - m.M02 * m.M10 * m.M23 - m.M03 * m.M12 * m.M20) / determinant,
					M20 = (m.M10 * m.M21 * m.M33 + m.M11 * m.M23 * m.M30 + m.M13 * m.M20 * m.M31 - m.M10 * m.M23 * m.M31 - m.M11 * m.M20 * m.M33 - m.M13 * m.M21 * m.M30) / determinant,
					M21 = (m.M00 * m.M23 * m.M31 + m.M01 * m.M20 * m.M33 + m.M03 * m.M21 * m.M30 - m.M00 * m.M21 * m.M33 - m.M01 * m.M23 * m.M30 - m.M03 * m.M11 * m.M31) / determinant,
					M22 = (m.M00 * m.M11 * m.M33 + m.M01 * m.M13 * m.M30 + m.M03 * m.M10 * m.M31 - m.M00 * m.M13 * m.M31 - m.M01 * m.M10 * m.M33 - m.M03 * m.M11 * m.M30) / determinant,
					M23 = (m.M00 * m.M13 * m.M21 + m.M01 * m.M10 * m.M23 + m.M03 * m.M11 * m.M20 - m.M00 * m.M11 * m.M23 - m.M01 * m.M13 * m.M20 - m.M03 * m.M10 * m.M21) / determinant,
					M30 = (m.M10 * m.M22 * m.M31 + m.M11 * m.M20 * m.M32 + m.M12 * m.M21 * m.M30 - m.M10 * m.M21 * m.M32 - m.M11 * m.M22 * m.M30 - m.M23 * m.M20 * m.M31) / determinant,
					M31 = (m.M00 * m.M21 * m.M32 + m.M01 * m.M22 * m.M30 + m.M02 * m.M20 * m.M31 - m.M00 * m.M22 * m.M31 - m.M01 * m.M20 * m.M32 - m.M02 * m.M21 * m.M30) / determinant,
					M32 = (m.M00 * m.M12 * m.M31 + m.M01 * m.M10 * m.M32 + m.M02 * m.M11 * m.M30 - m.M00 * m.M11 * m.M32 - m.M01 * m.M12 * m.M30 - m.M02 * m.M10 * m.M31) / determinant,
					M33 = (m.M00 * m.M11 * m.M22 + m.M01 * m.M12 * m.M20 + m.M02 * m.M10 * m.M21 - m.M00 * m.M12 * m.M21 - m.M01 * m.M10 * m.M22 - m.M02 * m.M11 * m.M20) / determinant
				};
		}

		/// <summary>
		/// Returns the inverse of the matrix
		/// </summary>
		/// <returns></returns>
		public Matrix4x4 Inverse()
		{
			return Matrix4x4.Inverse(this);
		}

		/// <summary>
		/// Compares the matrix to an object
		/// </summary>
		public override bool Equals(object obj)
		{
			if(obj is Matrix4x4)
			{
				return this == (Matrix4x4) obj;
			}

			return false;
		}

		/// <summary>
		/// Gets the hash code
		/// </summary>
		public override int GetHashCode()
		{
			return this.M00.GetHashCode() ^ this.M01.GetHashCode() ^ this.M02.GetHashCode() ^ this.M03.GetHashCode() ^
				this.M10.GetHashCode() ^ this.M11.GetHashCode() ^ this.M12.GetHashCode() ^ this.M13.GetHashCode() ^
				this.M20.GetHashCode() ^ this.M21.GetHashCode() ^ this.M22.GetHashCode() ^ this.M23.GetHashCode() ^
				this.M30.GetHashCode() ^ this.M31.GetHashCode() ^ this.M32.GetHashCode() ^ this.M33.GetHashCode();
		}

		/// <summary>
		/// Builds a string showing all the values of the matrix in a row x column fashion
		/// </summary>
		public override string ToString()
		{
			return string.Format("[{0}, {1}, {2}, {3}]\n[{4}, {5}, {6}, {7}]\n[{8}, {9}, {10}, {11}]\n[{12}, {13}, {14}, {15}]",
			                     M00, M01, M02, M03, M10, M11, M12, M13, M20, M21, M22, M23, M30, M31, M32, M33);
		}

		/// <summary>
		/// Builds a string showing all the values of the matrix in a row x column fashion
		/// </summary>
		public string ToRoundedString()
		{
			return string.Format("[{0}, {1}, {2}, {3}]\n[{4}, {5}, {6}, {7}]\n[{8}, {9}, {10}, {11}]\n[{12}, {13}, {14}, {15}]",
			                     Mathf.Round(M00), Mathf.Round(M01), Mathf.Round(M02), Mathf.Round(M03), Mathf.Round(M10),
			                     Mathf.Round(M11), Mathf.Round(M12), Mathf.Round(M13), Mathf.Round(M20), Mathf.Round(M21),
			                     Mathf.Round(M22), Mathf.Round(M23), Mathf.Round(M30), Mathf.Round(M31), Mathf.Round(M32),
			                     Mathf.Round(M33));
		}

		#endregion

		#region IEquatable Implementation

		/// <summary>
		/// Compares this instance to another matrix
		/// </summary>
		public bool Equals(Matrix4x4 other)
		{
			return this == other;
		}

		#endregion
	}
}
