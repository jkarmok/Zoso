using System;

namespace Karen90MmoFramework.Quantum
{
	public struct Bounds : IEquatable<Bounds>
	{
		#region Constants and Fields

		private Vector3 center;
		private Vector3 extents;

		/// <summary>
		/// Gets or sets the center
		/// </summary>
		public Vector3 Center
		{
			get
			{
				return this.center;
			}

			set
			{
				this.center = value;
			}
		}

		/// <summary>
		/// Gets or sets the size
		/// </summary>
		public Vector3 Size
		{
			get
			{
				return this.extents * 2f;
			}

			set
			{
				this.extents = value * 0.5f;
			}
		}

		/// <summary>
		/// Gets or sets the extents
		/// </summary>
		public Vector3 Extents
		{
			get
			{
				return this.extents;
			}

			set
			{
				this.extents = value;
			}
		}

		/// <summary>
		/// Gets or sets the min
		/// </summary>
		public Vector3 Min
		{
			get
			{
				return this.center - this.extents;
			}

			set
			{
				this.SetMinMax(value, this.Max);
			}
		}

		/// <summary>
		/// Gets or sets the max
		/// </summary>
		public Vector3 Max
		{
			get
			{
				return this.center + this.extents;
			}

			set
			{
				this.SetMinMax(this.Min, value);
			}
		}

		#endregion

		#region Constructors and Destructors

		public Bounds(Vector3 center, Vector3 size)
		{
			this.center = center;
			this.extents = size * 0.5f;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets min and max
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		void SetMinMax(Vector3 min, Vector3 max)
		{
			this.extents = (max - min) * 0.5f;
			this.center = min + this.extents;
		}

		/// <summary>
		/// Compares two bounding boxes for equality.
		/// </summary>
		public static bool operator ==(Bounds a, Bounds b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Compares two bounding boxes for inequality
		/// </summary>
		public static bool operator !=(Bounds a, Bounds b)
		{
			return !a.Equals(b);
		}

		/// <summary>
		/// Creates a bounding box from a polygon.
		/// </summary>
		public static Bounds CreateFromPoints(params Vector3[] points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}

			if (points.Length == 0)
			{
				throw new ArgumentException("points");
			}

			var min = points[0];
			var max = points[0];
			for (var index = 1; index < points.Length; index++)
			{
				var tmp = points[index];
				if (tmp.X < min.X)
				{
					min.X = tmp.X;
				}

				if (tmp.Y < min.Y)
				{
					min.Y = tmp.Y;
				}

				if (tmp.Z < min.Z)
				{
					min.Z = tmp.Z;
				}

				if (tmp.X > max.X)
				{
					max.X = tmp.X;
				}

				if (tmp.Y > max.Y)
				{
					max.Y = tmp.Y;
				}

				if (tmp.Z > max.Z)
				{
					max.Z = tmp.Z;
				}
			}

			return new Bounds { Min = min, Max = max };
		}

		/// <summary>
		/// Checks whether <paramref name = "point" /> exists within bounding box borders.
		/// </summary>
		public bool Contains(Vector3 point)
		{
			// not outside of box?
			return (point.X < this.Min.X || point.X > this.Max.X || point.Y < this.Min.Y || point.Y > this.Max.Y || point.Z < this.Min.Z || point.Z > this.Max.Z) ==
				   false;
		}

		/// <summary>
		/// Clamps a vector within the bounds
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public Vector3 Clamp(Vector3 vector)
		{
			vector.X = Mathf.Clamp(vector.X, Min.X, Max.X - 1);
			vector.Z = Mathf.Clamp(vector.Z, Min.Z, Max.Z - 1);

			return vector;
		}

		/// <summary>
		/// Checks whether <paramref name = "obj" /> is a bounding box with equal values.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (obj is Bounds) && this.Equals((Bounds)obj);
		}

		/// <summary>
		/// Gets all 4 corners of a 2D bounding box with the minimum Z.
		/// </summary>
		public Vector3[] GetCorners2D()
		{
			return new[]
                {
                    new Vector3 { X = this.Min.X, Y = this.Min.Y, Z = this.Min.Z }, new Vector3 { X = this.Max.X, Y = this.Min.Y, Z = this.Min.Z }, 
                    new Vector3 { X = this.Min.X, Y = this.Max.Y, Z = this.Min.Z }, new Vector3 { X = this.Max.X, Y = this.Max.Y, Z = this.Min.Z }
                };
		}

		/// <summary>
		/// Gets all 8 corners of a 3D bounding box.
		/// </summary>
		public Vector3[] GetCorners3D()
		{
			return new[]
                {
                    new Vector3 { X = this.Min.X, Y = this.Min.Y, Z = this.Min.Z }, new Vector3 { X = this.Max.X, Y = this.Min.Y, Z = this.Min.Z }, 
                    new Vector3 { X = this.Min.X, Y = this.Max.Y, Z = this.Min.Z }, new Vector3 { X = this.Max.X, Y = this.Max.Y, Z = this.Min.Z }, 
                    new Vector3 { X = this.Min.X, Y = this.Min.Y, Z = this.Max.Z }, new Vector3 { X = this.Max.X, Y = this.Min.Y, Z = this.Max.Z }, 
                    new Vector3 { X = this.Min.X, Y = this.Max.Y, Z = this.Max.Z }, new Vector3 { X = this.Max.X, Y = this.Max.Y, Z = this.Max.Z }, 
                };
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		public override int GetHashCode()
		{
			return this.Min.GetHashCode() + this.Max.GetHashCode();
		}

		/// <summary>
		/// Intersects this instance with another bounding box.
		/// </summary>
		public Bounds IntersectWith(Bounds other)
		{
			return new Bounds { Min = Vector3.Max(this.Min, other.Min), Max = Vector3.Min(this.Max, other.Max) };
		}

		/// <summary>
		/// Checks whether <see cref = "Max" /> and <see cref = "Min" /> span a valid (positive) area.
		/// </summary>
		public bool IsValid()
		{
			return (this.Max.X < this.Min.X || this.Max.Y < this.Min.Y || this.Max.Z < this.Min.Z) == false;
		}

		/// <summary>
		/// Unites two bounding boxes.
		/// </summary>
		public Bounds UnionWith(Bounds other)
		{
			return new Bounds { Min = Vector3.Min(this.Min, other.Min), Max = Vector3.Max(this.Max, other.Max) };
		}

		#endregion

		#region IEquatable Implementation

		/// <summary>
		/// Compares this instance to another bounding box
		/// </summary>
		public bool Equals(Bounds other)
		{
			if (this.center == other.center)
				return this.extents == other.extents;

			return false;
		}

		#endregion
	}
}