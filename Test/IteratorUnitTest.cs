using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Karen90MmoTests
{
	class IteratorUnitTest : IDemo, IEnumerable<string>
	{
		private Dictionary<Guid, Tuple<int, string>> cache = new Dictionary<Guid, Tuple<int, string>>();

		#region Implementation of IDemo

		public void Run()
		{
			for (int i = 0; i < 100; i++)
			{
				for (int j = 0; j < 100; j++)
				{
					cache.Add(Guid.NewGuid(), new Tuple<int, string>(i, (i+j).ToString()));
				}
			}
			var w1 = Stopwatch.StartNew();
			w1.Start();

			for (int i = 0; i < 10000; i++)
			{
				foreach (var v in this.GetEnumeratorViaYield())
				{
					v.Trim();
				}
			}

			w1.Stop();
			Console.WriteLine(w1.ElapsedMilliseconds);
			var w2 = Stopwatch.StartNew();
			w2.Start();

			for (int i = 0; i < 10000; i++)
			{
				foreach (var v in this)
				{
					v.Trim();
				}
			}

			w2.Stop();
			Console.WriteLine(w2.ElapsedMilliseconds);
			

		}

		public IEnumerable<string> GetEnumeratorViaYield()
		{
			return this.cache.Values.Select(tuple => tuple.Item2);
		}

		#endregion

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<string> GetEnumerator()
		{
			return new Enumerator(cache.Values.GetEnumerator());
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		struct Enumerator : IEnumerator<string>
		{
			private readonly IEnumerator<Tuple<int, string>> enumerator;

			public Enumerator(IEnumerator<Tuple<int, string>> enumerator)
			{
				this.enumerator = enumerator;
			}

			#region Implementation of IDisposable

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose()
			{
				this.enumerator.Dispose();
			}

			#endregion

			#region Implementation of IEnumerator

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
			public bool MoveNext()
			{
				return this.enumerator.MoveNext();
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
			public void Reset()
			{
				this.enumerator.Reset();
			}

			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			/// <returns>
			/// The element in the collection at the current position of the enumerator.
			/// </returns>
			public string Current
			{
				get
				{
					return this.enumerator.Current.Item2;
				}
			}

			/// <summary>
			/// Gets the current element in the collection.
			/// </summary>
			/// <returns>
			/// The current element in the collection.
			/// </returns>
			/// <filterpriority>2</filterpriority>
			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			#endregion
		}
	}
}
