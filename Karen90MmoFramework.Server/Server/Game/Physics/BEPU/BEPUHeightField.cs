using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;
using Karen90MmoFramework.Quantum;
using Quaternion = BEPUutilities.Quaternion;
using Vector3 = BEPUutilities.Vector3;

namespace Karen90MmoFramework.Server.Game.Physics.BEPU
{
	public class BEPUHeightField : Terrain, IBEPUHeightField
	{
		private readonly Karen90MmoFramework.Quantum.Vector3 position;
		private readonly int widthX;
		private readonly float[] heights;

		public BEPUHeightField(int widthX, int widthZ, float[,] heights, Karen90MmoFramework.Quantum.Vector3 position)
			: base(heights, new AffineTransform(new Vector3(1, 1, 1), Quaternion.Identity, new Vector3(position.X, position.Y, position.Z)))
		{
			if (heights == null)
				throw new ArgumentNullException("heights");

			if (widthX * widthZ != heights.Length)
				throw new ArgumentOutOfRangeException("heights");

			this.position = position;
			this.widthX = widthX;
			this.heights = new float[widthX * widthZ];

			for (var z = 0; z < widthZ; z++)
				for (var x = 0; x < widthX; x++)
					this.heights[z * widthX + x] = heights[x, z];
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
	}
}
