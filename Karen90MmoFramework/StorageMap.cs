using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Karen90MmoFramework
{
	/// <summary>
	/// A non thread-safe general purpose container
	/// </summary>
	public sealed class StorageMap<TKey, TValue> : IStorageMapAccessor<TKey, TValue>
	{
		#region Constants and Fields

		/// <summary>
		/// items
		/// </summary>
		private readonly Dictionary<TKey, TValue> items;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Creates a new instance of the <see cref="StorageMap{TKey, TValue}"/> class.
		/// </summary>
		public StorageMap()
		{
			this.items = new Dictionary<TKey, TValue>();
		}
		
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<TValue> GetEnumerator()
		{
			return this.items.Values.GetEnumerator();
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

		#region Public Methods

		/// <summary>
		/// Adds an item
		/// </summary>
		public bool Add(TKey key, TValue value)
		{
			if (this.items.ContainsKey(key))
				return false;

			this.items.Add(key, value);
			return true;
		}

		/// <summary>
		/// Removes an item
		/// </summary>
		public bool Remove(TKey key)
		{
			return this.items.Remove(key);
		}

		/// <summary>
		/// Tries to retrieve an item
		/// </summary>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.items.TryGetValue(key, out value);
		}

		/// <summary>
		/// Clears the cache
		/// </summary>
		public void Clear()
		{
			this.items.Clear();
		}

		#endregion
	}

	/// <summary>
	/// A non thread-safe general purpose container
	/// </summary>
	public sealed class StorageMap<TKey0, TKey1, TValue> : IStorageMapAccessor<TKey0, TKey1, TValue>
	{
		#region Constants and Fields

		/// <summary>
		/// caches
		/// </summary>
		private readonly Dictionary<TKey0, StorageMap<TKey1, TValue>> caches;

		#endregion

		#region Construtor and Destructor

		/// <summary>
		/// Creates a new instance of the <see cref="StorageMap{TKey0, TKey1, TValue}"/> class.
		/// </summary>
		public StorageMap()
		{
			this.caches = new Dictionary<TKey0, StorageMap<TKey1, TValue>>();
		}

		#endregion

		#region Implementation of IEnumerator

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<TValue> GetEnumerator()
		{
			return this.caches.Values.SelectMany(cache => cache).GetEnumerator();
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

		#region Public Methods

		/// <summary>
		/// Adds an item to the cache
		/// </summary>
		public bool Add(TKey0 key0, TKey1 key1, TValue value)
		{
			var l2Cache = this.GetLevel2Cache(key0);
			return l2Cache != null && l2Cache.Add(key1, value);
		}

		/// <summary>
		/// Removes an item from the cache
		/// </summary>
		public bool Remove(TKey0 key0, TKey1 key1)
		{
			var l2Cache = this.GetLevel2Cache(key0);
			return l2Cache != null && l2Cache.Remove(key1);
		}

		/// <summary>
		/// Tries to retrieve an item
		/// </summary>
		public bool TryGetValue(TKey0 key0, TKey1 key1, out TValue value)
		{
			value = default(TValue);
			var l2Cache = this.GetLevel2Cache(key0);
			return l2Cache != null && l2Cache.TryGetValue(key1, out value);
		}

		/// <summary>
		/// Clears the cache
		/// </summary>
		public void Clear()
		{
			foreach (var cache in caches.Values)
				cache.Clear();
		}

		/// <summary>
		/// Loops through the cache
		/// </summary>
		/// <param name="method"></param>
		public void ForEach(Action<TValue> method)
		{
			foreach (var value in this.caches.Values.SelectMany(cache => cache))
				method(value);
		}

		/// <summary>
		/// Gets the level 2 cache or creates a new one if not available
		/// </summary>
		private StorageMap<TKey1, TValue> GetLevel2Cache(TKey0 key0)
		{
			StorageMap<TKey1, TValue> cache;
			if (caches.TryGetValue(key0, out cache))
				return cache;

			cache = new StorageMap<TKey1, TValue>();
			this.caches.Add(key0, cache);
			return cache;
		}

		#endregion
	}
}
