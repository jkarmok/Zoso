using System;

using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game
{
	public static class Extensions
	{
		/// <summary>
		/// Converts a vector value to float array
		/// </summary>
		public static float[] ToFloatArray(this Vector3 vector, byte dimensions)
		{
			switch (dimensions)
			{
				case 0:
					return new float[0];

				case 1:
					return new[] { Convert.ToSingle(vector.X) };

				case 2:
					return new[] { Convert.ToSingle(vector.X), Convert.ToSingle(vector.Y) };

				case 3:
					return new[] { Convert.ToSingle(vector.X), Convert.ToSingle(vector.Y), Convert.ToSingle(vector.Z) };
					
				default:
					throw new ArgumentOutOfRangeException("dimensions");
			}
		}

		/// <summary>
		/// Converts a float array to vector
		/// </summary>
		public static Vector3 ToVector(this float[] coordinate)
		{
			switch (coordinate.Length)
			{
				case 0:
					return new Vector3();
					
				case 1:
					return new Vector3() { X = coordinate[0] };

				case 2:
					return new Vector3()
					{ 
						X = coordinate[0],
						Y = coordinate[1]
					};

				default:
					return new Vector3()
					{
						X = coordinate[0],
						Y = coordinate[1],
						Z = coordinate[2],
					};
			}
		}
	}
}
