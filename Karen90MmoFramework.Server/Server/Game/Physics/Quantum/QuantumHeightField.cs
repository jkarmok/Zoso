using System;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Quantum
{
	public class QuantumHeightField : IHeightField, IShape
	{
		private readonly Bounds bounds;
		private readonly Vector3 position;
		private readonly int widthX;
		private readonly float[] heights;

		public QuantumHeightField(int widthX, int widthZ, float[,] heights, Vector3 position)
		{
			if (heights == null)
				throw new ArgumentNullException("heights");

			if (widthX * widthZ != heights.Length)
				throw new ArgumentOutOfRangeException("heights");

			this.position = position;
			this.widthX = widthX;
			this.heights = new float[widthX * widthZ];

			var minY = 0f;
			var maxY = 0f;
			for (var z = 0; z < widthZ; z++)
			{
				for (var x = 0; x < widthX; x++)
				{
					var height = heights[x, z];
					this.heights[z * widthX + x] = height;
					if (height < minY)
						minY = height;
					if (height > maxY)
						maxY = height;
				}
			}

			this.bounds = new Bounds {Min = new Vector3(0, minY, 0), Max = new Vector3(widthX, maxY, widthZ)};
		}

		/// <summary>
		/// Gets the vertex height at a certain position
		/// </summary>
		float GetHeight(int x, int z)
		{
			return this.heights[z * widthX + x];
		}

		public float GetHeight(float x, float z)
		{
			x = x - position.X;
			z = z - position.Z;

			var xmin = (int)x;
			var xmax = Mathf.CeilToInt(x);
			var zmin = (int)z;
			var zmax = Mathf.CeilToInt(z);

			var hx0z0 = GetHeight(xmin, zmin);
			var hx1z0 = GetHeight(xmax, zmin);
			var hx0z1 = GetHeight(xmin, zmax);
			var hx1z1 = GetHeight(xmax, zmax);

			var dX = x - xmin;
			var dZ = z - zmin;

			return position.Y + hx0z0 + (hx1z0 - hx0z0) * dX + (hx0z1 - hx0z0) * dZ + (hx0z0 - hx1z0 - hx0z1 + hx1z1) * dX * dZ;
		}

		#region Implementation of IShape

		public Bounds GetBounds()
		{
			return bounds;
		}

		#endregion
	}
}
