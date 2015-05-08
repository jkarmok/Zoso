using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace Karen90MmoFramework
{
	/// <summary>
	/// A thread-safe general purpose container
	/// </summary>
	public sealed class ConcurrentStorageMap<TKey, TValue> : IDisposable, IStorageMapAccessor<TKey, TValue>
	{
		#region Constants and Fields

		/// <summary>
		/// items
		/// </summary>
		private readonly Dictionary<TKey, TValue> items;

		/// <summary>
		/// lock ms
		/// </summary>
		private readonly int maxLockMs;

		/// <summary>
		/// rw lock
		/// </summary>
		private readonly ReaderWriterLockSlim rwLock;

		#endregion

		#region Constructor and Destructor

		/// <summary>
		/// Creates a new instance of the <see cref="ConcurrentStorageMap{TKey, TValue}"/> class.
		/// </summary>
		/// <param name="maxLockMs"></param>
		public ConcurrentStorageMap(int maxLockMs)
		{
			this.maxLockMs = maxLockMs;
			this.rwLock = new ReaderWriterLockSlim();
			this.items = new Dictionary<TKey, TValue>();
		}

		~ConcurrentStorageMap()
		{
			this.Dispose(false);
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.items.Clear();
				this.rwLock.Dispose();
			}
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
			if (!rwLock.TryEnterReadLock(this.maxLockMs))
				yield break;

			try
			{
				foreach (var value in items.Values)
					yield return value;
			}
			finally
			{
				this.rwLock.ExitReadLock();
			}
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
			if (!rwLock.TryEnterWriteLock(this.maxLockMs))
				return false;

			try
			{
				if (this.items.ContainsKey(key))
					return false;

				this.items.Add(key, value);
				return true;
			}
			finally
			{
				this.rwLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Removes an item
		/// </summary>
		public bool Remove(TKey key)
		{
			if (!rwLock.TryEnterWriteLock(this.maxLockMs))
				return false;

			try
			{
				return this.items.Remove(key);
			}
			finally
			{
				this.rwLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Tries to retrieve an item
		/// </summary>
		public bool TryGetValue(TKey key, out TValue value)
		{
			value = default(TValue);
			if (!rwLock.TryEnterReadLock(this.maxLockMs))
				return false;

			try
			{
				return this.items.TryGetValue(key, out value);
			}
			finally
			{
				this.rwLock.ExitReadLock();
			}
		}

		#endregion
	}

	/// <summary>
	/// A thread-safe general purpose container
	/// </summary>
	public sealed class ConcurrentStorageMap<TKey0, TKey1, TValue> : IDisposable, IStorageMapAccessor<TKey0, TKey1, TValue>
	{
		#region Constants and Fields

		/// <summary>
		/// caches
		/// </summary>
		private readonly Dictionary<TKey0, ConcurrentStorageMap<TKey1, TValue>> caches;

		/// <summary>
		/// rw lock
		/// </summary>
		private readonly ReaderWriterLockSlim rwLock;

		/// <summary>
		/// max lock ms
		/// </summary>
		private readonly int maxLockMs;

		#endregion

		#region Construtor and Destructor

		/// <summary>
		/// Creates a new instance of the <see cref="ConcurrentStorageMap{TKey0, TKey1, TValue}"/> class.
		/// </summary>
		/// <param name="maxLockMilliseconds"></param>
		public ConcurrentStorageMap(int maxLockMilliseconds)
		{
			this.maxLockMs = maxLockMilliseconds;
			this.rwLock = new ReaderWriterLockSlim();
			this.caches = new Dictionary<TKey0, ConcurrentStorageMap<TKey1, TValue>>();
		}

		~ConcurrentStorageMap()
		{
			this.Dispose(false);
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
			if (!rwLock.TryEnterReadLock(this.maxLockMs))
				yield break;

			try
			{
				foreach (var cache in caches.Values)
					foreach (var value in cache)
						yield return value;
			}
			finally
			{
				this.rwLock.ExitReadLock();
			}
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

		#region IDisposable Implementation

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.rwLock.Dispose();
				foreach (var cache in this.caches.Values)
					cache.Dispose();

				this.caches.Clear();
			}
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
		/// Gets the level 2 cache or creates a new one if not available
		/// </summary>
		private ConcurrentStorageMap<TKey1, TValue> GetLevel2Cache(TKey0 key0)
		{
			if (!rwLock.TryEnterReadLock(this.maxLockMs))
				return null;

			try
			{
				ConcurrentStorageMap<TKey1, TValue> cache;
				if (this.caches.TryGetValue(key0, out cache))
				{
					return cache;
				}
			}
			finally
			{
				this.rwLock.ExitReadLock();
			}

			if (!rwLock.TryEnterWriteLock(this.maxLockMs))
				return null;

			try
			{
				ConcurrentStorageMap<TKey1, TValue> cache;
				if (false == this.caches.TryGetValue(key0, out cache))
				{
					cache = new ConcurrentStorageMap<TKey1, TValue>(this.maxLockMs);
					this.caches.Add(key0, cache);
				}

				return cache;
			}
			finally
			{
				this.rwLock.ExitWriteLock();
			}
		}

		#endregion
	}
}
