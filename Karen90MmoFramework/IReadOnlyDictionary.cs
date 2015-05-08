using System.Collections.Generic;
using System.Collections;

namespace Karen90MmoFramework
{
	public interface IReadOnlyDictionary<TKey, TValue> : IEnumerable
	{
		ICollection<TKey> Keys { get; }
		ICollection<TValue> Values { get; }

		int Count { get; }
		TValue this[TKey key] { get; }

		bool ContainsKey(TKey key);
		bool TryGetValue(TKey key, out TValue value);
	}
}
