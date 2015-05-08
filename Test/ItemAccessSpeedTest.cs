using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Karen90MmoTests
{
	class ItemAccessSpeedTest : IDemo
	{
		#region Implementation of IDemo

		public void Run()
		{
			var sw1 = Stopwatch.StartNew();
			sw1.Start();

			for (var i = 0; i < 10000; i++)
			{
				for (short j = 0; j < 100; j++)
				{
					string value;
					if (!Cache.Instance.TryGetString(j, out value))
						continue;
				}
			}

			sw1.Stop();
			Console.WriteLine(sw1.ElapsedMilliseconds);

			var sw2 = Stopwatch.StartNew();
			sw2.Start();

			for (var i = 0; i < 10000; i++)
			{
				for (short j = 0; j < 100; j++)
				{
					var value = Cache.Instance.GetString(j);
				}
			}

			sw2.Stop();
			Console.WriteLine(sw2.ElapsedMilliseconds);
		}

		#endregion
	}

	internal class Cache
	{
		private static readonly Cache _singleton = new Cache();

		private readonly Dictionary<short, string> cache = new Dictionary<short, string>();
		private readonly string[] cache2 = new string[100];

		public static Cache Instance
		{
			get
			{
				return _singleton;
			}
		}

		public Cache()
		{
			for (short i = 0; i < 100; i++)
			{
				cache.Add(i, i.ToString(CultureInfo.InvariantCulture));
				cache2[i] = i.ToString(CultureInfo.InvariantCulture);
			}
		}

		public bool TryGetString(short id, out string value)
		{
			return this.cache.TryGetValue(id, out value);
		}

		public string GetString(short id)
		{
			return this.cache2[id];
		}
	}
}
