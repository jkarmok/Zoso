using System.Collections.Generic;

namespace Karen90MmoFramework
{
	public interface IStorageMapAccessor<in TKey, TValue> : IEnumerable<TValue>
	{
		/// <summary>
		/// Tries to retrieve an item
		/// </summary>
		bool TryGetValue(TKey key, out TValue value);
	}

	public interface IStorageMapAccessor<in TKey0, in TKey1, TValue> : IEnumerable<TValue>
	{
		/// <summary>
		/// Tries to retrieve an item
		/// </summary>
		bool TryGetValue(TKey0 key0, TKey1 key1, out TValue value);
	}
}
