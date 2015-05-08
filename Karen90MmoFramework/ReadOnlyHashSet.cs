using System.Collections.Generic;
using System.Collections;

namespace Karen90MmoFramework
{
	public class ReadOnlyHashSet<T> : IReadOnlyHashSet<T>
	{
		private readonly HashSet<T> hashSet;

		/// <summary>
		/// Creates a ReadOnlyHashSet wrapper from a HashSet
		/// </summary>
		public ReadOnlyHashSet(HashSet<T> hashSet)
		{
			this.hashSet = hashSet;
		}

		/// <summary>
		/// Gets the number of elements contained in a set
		/// </summary>
		public int Count
		{
			get
			{
				return this.hashSet.Count;
			}
		}

		/// <summary>
		/// Determines whether the HashSet contains a specific element
		/// </summary>
		public bool Contains(T item)
		{
			return this.hashSet.Contains(item);
		}

		/// <summary>
		/// Returns an enumerator that interates through the HashSet
		/// </summary>
		public IEnumerator GetEnumerator()
		{
			return this.hashSet.GetEnumerator();
		}
	};
};
