using System;
using System.Threading;

namespace Karen90MmoFramework
{
	public class Counter
	{
		private int counter;

		/// <summary>
		/// Creates a new instance of the <see cref="Counter"/> class.
		/// </summary>
		/// <param name="min"></param>
		public Counter(int min)
		{
			this.counter = min;
		}

		/// <summary>
		/// Gets the next Id
		/// </summary>
		public Int32 Next
		{
			get
			{
				return Interlocked.Increment(ref counter);
			}
		}
	}
}
