using System;

namespace Karen90MmoFramework.Server.Game
{
	public class ZoneDescription
	{
		private readonly int terrainWidthX;
		private readonly int terrainWidthZ;
		private readonly float[] heights;
		private readonly Colliders colliders;

		/// <summary>
		/// Creates a new instance of the <see cref="ZoneDescription"/> class.
		/// </summary>
		public ZoneDescription(int terrainWidthX, int terrainWidthZ, float[] heights, Colliders colliders)
		{
			if(heights == null)
				throw new NullReferenceException("heights");

			if (colliders == null)
				throw new NullReferenceException("colliders");

			this.terrainWidthX = terrainWidthX;
			this.terrainWidthZ = terrainWidthZ;
			this.heights = heights;
			this.colliders = colliders;
		}

		/// <summary>
		/// Gets the width-X of the terrain
		/// </summary>
		public int TerrainWidthX
		{
			get
			{
				return this.terrainWidthX;
			}
		}

		/// <summary>
		/// Gets the width-Z of the terrain
		/// </summary>
		public int TerrainWidthZ
		{
			get
			{
				return this.terrainWidthZ;
			}
		}

		/// <summary>
		/// Gets the heights
		/// </summary>
		public float[] Heights
		{
			get
			{
				return this.heights;
			}
		}

		/// <summary>
		/// Gets the colliders
		/// </summary>
		public Colliders Colliders
		{
			get
			{
				return this.colliders;
			}
		}
	}
}
