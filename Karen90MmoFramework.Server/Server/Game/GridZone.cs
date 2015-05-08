using System;
using System.Collections.Generic;

using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game
{
	public class GridZone : IDisposable
	{
		#region Constants and Fields

		/// <summary>
		/// The rectangle area.
		/// </summary>
		private readonly Bounds rectangleArea;

		/// <summary>
		/// The tile dimensions.
		/// </summary>
		private readonly Vector3 tileDimensions;

		/// <summary>
		/// The tile dimensions minus 1
		/// </summary>
		private readonly Vector3 tileSize;
		 
		/// <summary>
		/// The world regions.
		/// </summary>
		private readonly Region[][] worldRegions;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="GridZone"/> class using bounding box.
		/// </summary>
		public GridZone(Bounds bounds, Vector3 tileDimensions)
		{
			this.rectangleArea = new Bounds
				{
					Min = new Vector3 {X = bounds.Min.X, Y = bounds.Min.Y, Z = bounds.Min.Z},
					Max = new Vector3 { X = bounds.Max.X, Y = bounds.Max.Y, Z = bounds.Max.Z }
				};

			var size = new Vector3 { X = bounds.Max.X - bounds.Min.X + 1, Z = bounds.Max.Z - bounds.Min.Z + 1 };
			if (tileDimensions.X <= 0)
			{
				tileDimensions.X = size.X;
			}

			if (tileDimensions.Z <= 0)
			{
				tileDimensions.Z = size.Z;
			}

			this.tileDimensions = tileDimensions;
			this.tileSize = new Vector3 { X = tileDimensions.X - 1, Z = tileDimensions.Z - 1 };

			var regionsX = (int)Math.Ceiling(size.X / (double)tileDimensions.X);
			var regionsZ = (int)Math.Ceiling(size.Z / (double)tileDimensions.Z);

			this.worldRegions = new Region[regionsX][];
			var current = bounds.Min;
			for (var x = 0; x < regionsX; x++)
			{
				this.worldRegions[x] = new Region[regionsZ];
				for (var z = 0; z < regionsZ; z++)
				{
					this.worldRegions[x][z] = new Region(current);
					current.Z += tileDimensions.Z;
				}

				current.X += tileDimensions.X;
				current.Z = bounds.Min.Z;
			}
		}

		~GridZone()
		{
			this.Dispose(false);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the area.
		/// </summary>
		public Bounds Bounds
		{
			get
			{
				return this.rectangleArea;
			}
		}

		/// <summary>
		/// Gets the size of the used tiles (size of each <see cref = "Region" />).
		/// </summary>
		public Vector3 TileDimensions
		{
			get
			{
				return this.tileDimensions;
			}
		}

		/// <summary>
		/// Tells whether the object is disposed or not
		/// </summary>
		public bool Disposed { get; set; }

		#endregion

		#region Implemented Interfaces

		#region IDisposable

		/// <summary>
		/// Disposes all used <see cref = "Region">regions</see>.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Disposes all <see cref = "Region" />s if <paramref name = "disposing" /> is true.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var regions in this.worldRegions)
					foreach (var region in regions)
						region.Dispose();
			}
			this.Disposed = true;
		}

		/// <summary>
		/// Gets a region at the given <paramref name = "position" />.
		/// </summary>
		public Region GetRegion(Vector3 position)
		{
			if (rectangleArea.Contains(position))
				return this.GetRegionAt(position);

			return null;
		}

		/// <summary>
		/// Returns a bounding box that is aligned with the Grid.
		/// </summary>
		public Bounds GetRegionAlignedBoundingBox(Bounds area)
		{
			area = this.rectangleArea.IntersectWith(area);
			if (area.IsValid())
			{
				var result = new Bounds { Min = this.GetRegionAt(area.Min).Coordinate, Max = this.GetRegionAt(area.Max).Coordinate + this.tileSize, };
				return result;
			}

			return area;
		}

		/// <summary>
		/// Gets all <see cref = "Region">regions</see> overlapping with a specific <paramref name = "area" />.
		/// </summary>
		public HashSet<Region> GetRegions(Bounds area)
		{
			return new HashSet<Region>(this.GetRegionEnumerable(area));
		}

		/// <summary>
		/// Gets the regions that overlap with the <paramref name = "area" /> except the ones that do also overlap with <paramref name = "except" />.
		/// </summary>
		public HashSet<Region> GetRegionsExcept(Bounds area, Bounds except)
		{
			var result = new HashSet<Region>();

			// min x
			if (area.Min.X < except.Min.X)
			{
				// get all left to except
				var box = new Bounds { Min = area.Min, Max = new Vector3 { X = Math.Min(area.Max.X, except.Min.X - 1), Z = area.Max.Z } };
				result.UnionWith(this.GetRegionEnumerable(box));
			}

			// min z
			if (area.Min.Z < except.Min.Z)
			{
				// get all above except
				var box = new Bounds { Min = area.Min, Max = new Vector3 { X = area.Max.X, Z = Math.Min(area.Max.Z, except.Min.Z - 1) } };
				result.UnionWith(this.GetRegionEnumerable(box));
			}

			// max x
			if (area.Max.X > except.Max.X)
			{
				// get all right to except
				var box = new Bounds { Min = new Vector3 { X = Math.Max(area.Min.X, except.Max.X + 1), Z = area.Min.Z }, Max = area.Max };
				result.UnionWith(this.GetRegionEnumerable(box));
			}

			// max z
			if (area.Max.Z > except.Max.Z)
			{
				// get all below except
				var box = new Bounds { Min = new Vector3 { X = area.Min.X, Z = Math.Max(area.Min.Z, except.Max.Z + 1) }, Max = area.Max };
				result.UnionWith(this.GetRegionEnumerable(box));
			}

			return result;
		}

		/// <summary>
		///   The get region at.
		/// </summary>
		private Region GetRegionAt(Vector3 coordinate)
		{
			var relativePoint = coordinate - this.rectangleArea.Min;
			var indexX = (int)relativePoint.X / (int)this.tileDimensions.X;
			var indexZ = (int)relativePoint.Z / (int)this.tileDimensions.Z;
			return this.worldRegions[indexX][indexZ];
		}

		/// <summary>
		///   Gets all overlapping regions.
		/// </summary>
		private IEnumerable<Region> GetRegionEnumerable(Bounds area)
		{
			var overlap = this.rectangleArea.IntersectWith(area);

			var current = overlap.Min;
			while (current.Z <= overlap.Max.Z)
			{
				foreach (Region region in this.GetRegionsForZ(overlap, current))
				{
					yield return region;
				}

				// go stepwise to the bottom
				current.Z += this.tileDimensions.Z;
			}

			if (current.Z > overlap.Max.Z)
			{
				current.Z = overlap.Max.Z;
				foreach (Region region in this.GetRegionsForZ(overlap, current))
				{
					yield return region;
				}
			}

			yield break;
		}

		/// <summary>
		///   The get region index for z.
		/// </summary>
		private IEnumerable<Region> GetRegionsForZ(Bounds overlap, Vector3 current)
		{
			// start on left side
			current.X = overlap.Min.X;
			while (current.X <= overlap.Max.X)
			{
				yield return this.GetRegionAt(current);

				// go stepwise to the right
				current.X += this.tileDimensions.X;
			}

			if (current.X > overlap.Max.X)
			{
				current.X = overlap.Max.X;
				yield return this.GetRegionAt(current);
			}
		}

		#endregion
	}
}
