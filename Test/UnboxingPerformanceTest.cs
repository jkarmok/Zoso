using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karen90MmoTests
{
	class UnboxingPerformanceTest : IDemo
	{
		private const int CONTAINER_SIZE = 80;
		private const int INTERATION = 10000;

		#region Implementation of IDemo

		public void Run()
		{
			var genericContainer = new Container<ClassChild>(CONTAINER_SIZE, i => new ClassChild { Id = i, Name = i.ToString(CultureInfo.InvariantCulture) });
			var container = new Container(CONTAINER_SIZE, i => new ClassChild { Id = i, Name = i.ToString(CultureInfo.InvariantCulture) });

			Console.WriteLine("Generic Test Starts ...");
			var sw1 = Stopwatch.StartNew();
			sw1.Start();

			for (var i = 0; i < INTERATION; i++)
			{
				for (var j = 0; j < CONTAINER_SIZE; j++)
				{
					var item = genericContainer.GetItemAt(j);
				}
			}

			sw1.Stop();
			Console.WriteLine(sw1.ElapsedMilliseconds);

			Console.WriteLine("Non-Generic Test Starts ...");
			var sw2 = Stopwatch.StartNew();
			sw2.Start();

			for (var i = 0; i < INTERATION; i++)
			{
				for (var j = 0; j < CONTAINER_SIZE; j++)
				{
					var item = container.GetItemAt(j);
				}
			}

			sw2.Stop();
			Console.WriteLine(sw2.ElapsedMilliseconds);

			Console.WriteLine("Non-Generic Test Starts ...");
			var sw3 = Stopwatch.StartNew();
			sw3.Start();

			for (var i = 0; i < INTERATION; i++)
			{
				for (var j = 0; j < CONTAINER_SIZE; j++)
				{
					var item = container.GetItemAt(j);
				}
			}

			sw3.Stop();
			Console.WriteLine(sw3.ElapsedMilliseconds);

			Console.WriteLine("Generic Test Starts ...");
			var sw4 = Stopwatch.StartNew();
			sw4.Start();

			for (var i = 0; i < INTERATION; i++)
			{
				for (var j = 0; j < CONTAINER_SIZE; j++)
				{
					var item = genericContainer.GetItemAt(j);
				}
			}

			sw4.Stop();
			Console.WriteLine(sw4.ElapsedMilliseconds);

			Console.WriteLine("Non-Generic Test Starts ...");
			var sw5 = Stopwatch.StartNew();
			sw5.Start();

			for (var i = 0; i < INTERATION; i++)
			{
				for (var j = 0; j < CONTAINER_SIZE; j++)
				{
					var item = container.GetItemAt(j);
				}
			}

			sw5.Stop();
			Console.WriteLine(sw5.ElapsedMilliseconds);

			Console.WriteLine("Generic Test Starts ...");
			var sw6 = Stopwatch.StartNew();
			sw6.Start();

			for (var i = 0; i < INTERATION; i++)
			{
				for (var j = 0; j < CONTAINER_SIZE; j++)
				{
					var item = genericContainer.GetItemAt(j);
				}
			}

			sw6.Stop();
			Console.WriteLine(sw6.ElapsedMilliseconds);
		}

		#endregion
	}

	class Container<T> where T : ClassBase
	{
		private readonly Func<int, T> itemCreator;
		private readonly T[] collection;

		public Container(int size, Func<int, T> itemCreator)
		{
			this.collection = new T[size];
			this.itemCreator = itemCreator;

			for (var i = 0; i < size; i++)
				collection[i] = itemCreator(i);
		}

		public T GetItemAt(int slot)
		{
			var t = this.collection[slot];
			t.Id = 0;
			return t;
		}
	}

	class Container
	{
		private readonly Func<int, ClassChild> itemCreator;
		private readonly ClassChild[] collection;

		public Container(int size, Func<int, ClassChild> itemCreator)
		{
			this.collection = new ClassChild[size];
			this.itemCreator = itemCreator;

			for (var i = 0; i < size; i++)
				collection[i] = itemCreator(i);
		}

		public ClassChild GetItemAt(int slot)
		{
			var t = this.collection[slot];
			t.Id = 0;
			return t;
		}
	}

	class ClassBase
	{
		public int Id { get; set; }
	}

	class ClassChild : ClassBase
	{
		public string Name { get; set; }
	}
}
