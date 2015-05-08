using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karen90MmoTests
{
	class StructTest : IDemo
	{
		private readonly Dictionary<int, Structure[]> collection = new Dictionary<int, Structure[]>();

		#region Implementation of IDemo

		public void Run()
		{
			var items = new Structure[5];
			for (var i = 0; i < 5; i++)
			{
				var item = items[i];
				item.Id = i;
			}
			this.collection.Add(0, items);

			Structure[] coll;
			if(!collection.TryGetValue(0, out coll))
				return;

			foreach (var structure in coll)
				Console.WriteLine(structure.Id);
		}

		#endregion
	}

	struct Structure
	{
		public int Id;
	}
}
