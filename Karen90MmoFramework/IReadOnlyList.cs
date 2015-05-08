using System.Collections.Generic;

namespace Karen90MmoFramework
{
	public interface IReadOnlyList<T> : IEnumerable<T>
	{
		/// <summary>
		/// Gets the total number of elements
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets the element at the index
		/// </summary>
		T this[int index] { get; }

		/// <summary>
		/// Determines whether an element exists or not
		/// </summary>
		bool Contains(T value);

		/// <summary>
		/// Gets the index of the element
		/// </summary>
		int IndexOf(T value);
	}
}
