using System.Collections;
using System.Collections.Generic;

namespace Karen90MmoFramework
{
	public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
	{
		/// <summary>
		/// Gets all the keys
		/// </summary>
		public ICollection<TKey> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		/// <summary>
		/// Gets all the values
		/// </summary>
		public ICollection<TValue> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		/// <summary>
		/// Gets the total items count
		/// </summary>
		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		/// <summary>
		/// Gets the value using a key
		/// </summary>
		public TValue this[TKey key]
		{
			get
			{
				return this.dictionary[key];
			}
		}

		private readonly IDictionary<TKey, TValue> dictionary;

		/// <summary>
		/// Creates a ReadOnly dictionary from an IDictionary
		/// </summary>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			this.dictionary = dictionary;
		}

		/// <summary>
		/// Determines whether this dictionary has a specific key
		/// </summary>
		public bool ContainsKey(TKey key)
		{
			return this.dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Tries to get a value associated with a specific key
		/// </summary>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets the enumerator for this collection
		/// </summary>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator for this collection
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}
	};
};
