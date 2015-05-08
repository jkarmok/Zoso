using DigitalRune.Mathematics.Algebra;
using Karen90MmoFramework.Quantum;

namespace Karen90MmoFramework.Server.Game.Physics.Rune
{
	public static class Extensions
	{
		/// <summary>
		/// Converts game vector to digital rune vector
		/// </summary>
		/// <returns></returns>
		public static Vector3F ToRuneVector(this Vector3 vector)
		{
			return new Vector3F(vector.X, vector.Y, vector.Z);
		}

		/// <summary>
		/// Converts digital rune vector to game vector
		/// </summary>
		/// <returns></returns>
		public static Vector3 ToGameVector(this Vector3F vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}

		/// <summary>
		/// Converts game quaternion to digital rune quaternion
		/// </summary>
		/// <returns></returns>
		public static QuaternionF ToRuneQuaternion(this Quaternion quaternion)
		{
			return new QuaternionF(quaternion.W, quaternion.X, quaternion.Y, quaternion.Z);
		}

		/// <summary>
		/// Converts digital rune quaternion to game quaternion
		/// </summary>
		/// <returns></returns>
		public static Quaternion ToGameQuaternion(this QuaternionF quaternion)
		{
			return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
		}
	}
}
