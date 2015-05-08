using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Karen90MmoTests
{
	internal class StringCacheComparisonTest : IDemo
	{
		private readonly Dictionary<int, string> cache = new Dictionary<int, string>();

		private const int ITERATION = 100000;
		private const int MAX_VALUE = 40;

		#region Implementation of IDemo

		public void Run()
		{
			var sw1 = Stopwatch.StartNew();
			sw1.Start();

			for (var i = 0; i < ITERATION; i++)
			{
				for (var value = 0; value < MAX_VALUE; value++)
				{
					this.PrintByToString(value);
				}
			}

			sw1.Stop();
			Console.WriteLine(sw1.ElapsedMilliseconds);

			var sw2 = Stopwatch.StartNew();
			sw2.Start();

			for (var i = 0; i < ITERATION; i++)
			{
				for (var value = 0; value < MAX_VALUE; value++)
				{
					this.PrintByCache(value);
				}
			}

			sw2.Stop();
			Console.WriteLine(sw2.ElapsedMilliseconds);
		}

		private string ToStringByToString(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private string ToStringByCache(int value)
		{
			string str;
			if (!cache.TryGetValue(value, out str))
			{
				str = value.ToString(CultureInfo.InvariantCulture);
				cache.Add(value, str);
			}

			return str;
		}

		private void PrintByToString(int val)
		{
			this.ToStringByToString(val);
		}

		private void PrintByCache(int val)
		{
			this.ToStringByCache(val);
		}

		#endregion
	}
}
