using System;
using System.Threading;
using Karen90MmoFramework;

namespace Karen90MmoTests
{
	class ConcurrentStorageMapUnitTest : IDemo
	{
		#region Implementation of IDemo

		public void Run()
		{
			var map = new ConcurrentStorageMap<Guid, string>(-1);

			var t1 = new Thread(o =>
				{
					for (var i = 0; i < 10; i++)
						map.Add(Guid.NewGuid(), o + i.ToString());
				});
			t1.Start("one");
			t1.Join();

			var t2 = new Thread(o =>
				{
					for (var i = 0; i < 10; i++)
						map.Add(Guid.NewGuid(), o + i.ToString());
				});
			t2.Start("two");
			t2.Join();

			var t3 = new Thread(o =>
				{
					for (var i = 0; i < 10; i++)
						map.Add(Guid.NewGuid(), o + i.ToString());
				});
			t3.Start("three");
			t3.Join();

			foreach (var value in map)
				Console.WriteLine(value);
		}

		#endregion
	}
}
