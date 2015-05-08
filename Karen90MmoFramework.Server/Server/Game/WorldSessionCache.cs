using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Karen90MmoFramework.Server.Game
{
	public interface IPlayerSessionCacheAccessor
	{
		/// <summary>
		/// Retrieves a(n) <see cref="WorldSession"/> by session id
		/// </summary>
		bool TryGetSessionBySessionId(int sessionId, out WorldSession session);

		/// <summary>
		/// Retrieves a(n) <see cref="WorldSession"/> by session name
		/// </summary>
		bool TryGetSessionBySessionName(string name, out WorldSession session);

		/// <summary>
		/// Retrieves a(n) <see cref="WorldSession"/> by player guid
		/// </summary>
		bool TryGetSessionByPlayerGuid(int guid, out WorldSession session);
	}

	/// <summary>
	/// A thread-safe cache for <see cref="WorldSession"/>s.
	/// </summary>
	public class WorldSessionCache : IPlayerSessionCacheAccessor, IDisposable, IEnumerable<WorldSession>
	{
		#region Constants and Fields

		/// <summary>
		/// reader writer lock
		/// </summary>
		private readonly ReaderWriterLockSlim rwLock;

		/// <summary>
		/// players collection
		/// </summary>
		private readonly Dictionary<int, WorldSession> sessionsBySessionId;

		/// <summary>
		/// players collection
		/// </summary>
		private readonly Dictionary<int, WorldSession> sessionsByPlayerGuid;

		/// <summary>
		/// players collection
		/// </summary>
		private readonly Dictionary<string, WorldSession> sessionsByPlayerName;

		/// <summary>
		/// max lock ms
		/// </summary>
		private readonly int lockMs;

		#endregion

		#region Construtors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="WorldSessionCache"/> class.
		/// </summary>
		public WorldSessionCache(int maxLockMilliseconds)
		{
			this.lockMs = maxLockMilliseconds;
			this.rwLock = new ReaderWriterLockSlim();

			this.sessionsByPlayerGuid = new Dictionary<int, WorldSession>();
			this.sessionsBySessionId = new Dictionary<int, WorldSession>();
			this.sessionsByPlayerName = new Dictionary<string, WorldSession>(StringComparer.CurrentCultureIgnoreCase);
		}

		~WorldSessionCache()
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

		protected void Dispose(bool disposing)
		{
			this.rwLock.Dispose();

			if (disposing)
			{
				foreach (var entry in this.sessionsBySessionId)
				{
					entry.Value.Destroy(DestroySessionReason.KickedByServer);
				}

				this.sessionsBySessionId.Clear();
				this.sessionsByPlayerName.Clear();
				this.sessionsByPlayerGuid.Clear();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a(n) <see cref="WorldSession"/> to the cache
		/// </summary>
		public bool AddSession(WorldSession session)
		{
			if (!rwLock.TryEnterWriteLock(lockMs))
				return false;

			try
			{
				if (!sessionsByPlayerGuid.ContainsKey(session.Guid))
				{
					this.sessionsByPlayerGuid.Add(session.Guid, session);
					this.sessionsBySessionId.Add(session.SessionId, session);
					this.sessionsByPlayerName.Add(session.Name, session);

					return true;
				}
				return false;
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Removes a(n) <see cref="WorldSession"/> from the cache
		/// </summary>
		public bool RemoveSession(WorldSession session)
		{
			if (!rwLock.TryEnterWriteLock(lockMs))
				return false;

			try
			{
				if (sessionsByPlayerGuid.Remove(session.Guid))
				{
					this.sessionsBySessionId.Remove(session.SessionId);
					this.sessionsByPlayerName.Remove(session.Name);

					return true;
				}
				return false;
			}
			finally
			{
				rwLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Retrieves a(n) <see cref="WorldSession"/> by session id
		/// </summary>
		public bool TryGetSessionBySessionId(int sessionId, out WorldSession session)
		{
			rwLock.EnterReadLock();
			try
			{
				return this.sessionsBySessionId.TryGetValue(sessionId, out session);
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Retrieves a(n) <see cref="WorldSession"/> by session name
		/// </summary>
		public bool TryGetSessionBySessionName(string name, out WorldSession session)
		{
			rwLock.EnterReadLock();
			try
			{
				return this.sessionsByPlayerName.TryGetValue(name, out session);
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Retrieves a(n) <see cref="WorldSession"/> by player guid
		/// </summary>
		public bool TryGetSessionByPlayerGuid(int guid, out WorldSession session)
		{
			rwLock.EnterReadLock();
			try
			{
				return this.sessionsByPlayerGuid.TryGetValue(guid, out session);
			}
			finally
			{
				rwLock.ExitReadLock();
			}
		}

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		/// Gets the Enumerator
		/// </summary>
		public IEnumerator<WorldSession> GetEnumerator()
		{
			return sessionsBySessionId.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return sessionsBySessionId.Values.GetEnumerator();
		}

		#endregion
	}
}
