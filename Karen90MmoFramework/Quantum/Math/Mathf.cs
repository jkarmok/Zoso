using System;

namespace Karen90MmoFramework.Quantum
{
	public static class Mathf
	{
		public const float INFINITY = float.PositiveInfinity;
		public const float NEGATIVE_INFINITY = float.NegativeInfinity;

		public const float PI = (float) Math.PI;

		public const float DEG2_RAD = PI / 180;
		public const float RAD2_DEG = 180 / PI;

		public const float K_EPSILON = 1.401298E-45f;

		/// <summary>
		/// Returns the sine value of an angle
		/// </summary>
		public static float Sin(float a)
		{
			return (float) Math.Sin(a);
		}

		/// <summary>
		/// Returns the cosine value of an angle
		/// </summary>
		public static float Cos(float a)
		{
			return (float) Math.Cos(a);
		}

		/// <summary>
		/// Returns the tangent value of an angle
		/// </summary>
		public static float Tan(float a)
		{
			return (float) Math.Tan(a);
		}

		/// <summary>
		/// Returns the arc sine of a value
		/// </summary>
		public static float Asin(float d)
		{
			return (float) Math.Asin(d);
		}

		/// <summary>
		/// Returns the arc cosine of a value
		/// </summary>
		public static float Acos(float d)
		{
			return (float) Math.Acos(d);
		}

		/// <summary>
		/// Returns the arc tangent of a value
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static float Atan(float d)
		{
			return (float) Math.Atan(d);
		}

		/// <summary>
		/// Returns the arc tangent of two quotients
		/// </summary>
		public static float Atan2(float y, float x)
		{
			return (float) Math.Atan2(y, x);
		}

		/// <summary>
		/// Gets the square root of a value
		/// </summary>
		public static float Sqrt(float f)
		{
			return (float) Math.Sqrt(f);
		}

		/// <summary>
		/// Gets the absolute of a value
		/// </summary>
		public static float Abs(float f)
		{
			return Math.Abs(f);
		}

		/// <summary>
		/// Gets the absolute of a value
		/// </summary>
		public static int Abs(int value)
		{
			return Math.Abs(value);
		}

		/// <summary>
		/// Clamps a value between min and max
		/// </summary>
		public static float Clamp01(float value)
		{
			return value > 1f ? 1f : (value < 0f ? 0f : value);
		}

		/// <summary>
		/// Clamps a value between min and max
		/// </summary>
		public static float Clamp(float value, float min, float max)
		{
			return value > max ? max : (value < min ? min : value);
		}

		/// <summary>
		/// Clamps a value between min and max
		/// </summary>
		public static int Clamp(int value, int min, int max)
		{
			return value > max ? max : (value < min ? min : value);
		}

		/// <summary>
		/// Returns the max of two
		/// </summary>
		public static int Max(int val1, int val2)
		{
			return (val1 > val2) ? val1 : val2;
		}

		/// <summary>
		/// Returns the max of two
		/// </summary>
		public static float Max(float val1, float val2)
		{
			return (val1 > val2) ? val1 : val2;
		}

		/// <summary>
		/// Returns the min of two
		/// </summary>
		public static int Min(int val1, int val2)
		{
			return (val1 < val2) ? val1 : val2;
		}

		/// <summary>
		/// Returns the min of two
		/// </summary>
		public static float Min(float val1, float val2)
		{
			return (val1 < val2) ? val1 : val2;
		}

		/// <summary>
		/// Gets the floor of a value
		/// </summary>
		public static float Floor(float value)
		{
			return (float) Math.Floor(value);
		}

		/// <summary>
		/// Gets the floor of a value
		/// </summary>
		public static int FloorToInt(float value)
		{
			return (int) Math.Floor(value);
		}

		/// <summary>
		/// Gets the ceil of a value
		/// </summary>
		public static float Ceil(float value)
		{
			return (float) Math.Ceiling(value);
		}

		/// <summary>
		/// Gets the ceil of a value
		/// </summary>
		public static int CeilToInt(float value)
		{
			return (int) Math.Ceiling(value);
		}

		/// <summary>
		/// Rounds a value to two decimal places
		/// </summary>
		public static float Round(float value)
		{
			return (float) Math.Round(value, 2);
		}

		/// <summary>
		/// Rounds a value to an integer
		/// </summary>
		public static int RoundToInt(float value)
		{
			return (int) Math.Round(value);
		}
	}
}
