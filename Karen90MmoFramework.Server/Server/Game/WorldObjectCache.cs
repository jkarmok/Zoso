using System;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game
{
	public interface IWorldObjectCacheAccessor
	{
		/// <summary>
		/// Tries to retrieve an item
		/// </summary>
		bool TryGetItem(MmoGuid guid, out MmoObject mmoObject);
	}

	/// <summary>
	/// A thread-safe cache for <see cref="MmoObject"/>s
	/// </summary>
	public class WorldObjectCache : IDisposable, IWorldObjectCacheAccessor
	{
		#region Constants and Fields

		/// <summary>
		/// caches
		/// </summary>
		private readonly ConcurrentStorageMap<byte, int, MmoObject> objects;

		#endregion

		#region Construtor and Destructor

		/// <summary>
		/// Creates a new instance of the <see cref="WorldObjectCache"/> class.
		/// </summary>
		/// <param name="maxLockMilliseconds"></param>
		public WorldObjectCache(int maxLockMilliseconds)
		{
			this.objects = new ConcurrentStorageMap<byte, int, MmoObject>(maxLockMilliseconds);
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose()
		{
			this.objects.Dispose();
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds an item to the cache
		/// </summary>
		public bool AddItem(MmoObject mmoObject)
		{
			var guid = mmoObject.Guid;
			return objects.Add(guid.Type, guid.Id, mmoObject);
		}

		/// <summary>
		/// Removes an item from the cache
		/// </summary>
		public bool RemoveItem(MmoGuid guid)
		{
			return objects.Remove(guid.Type, guid.Id);
		}

		/// <summary>
		/// Tries to retrieve an item
		/// </summary>
		public bool TryGetItem(MmoGuid guid, out MmoObject mmoObject)
		{
			return objects.TryGetValue(guid.Type, guid.Id, out mmoObject);
		}

		#endregion
	}
}
