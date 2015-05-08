using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace Karen90MmoFramework.Server.Social
{
	public interface ISocialSessionCacheAccessor
	{
		/// <summary>
		/// Retrieves a(n) <see cref="SocialSession"/> by session id
		/// </summary>
		bool TryGetSessionBySessionId(int sessionId, out SocialSession session);
		
		/// <summary>
		/// Retrieves a(n) <see cref="SocialSession"/> by session name
		/// </summary>
		bool TryGetSessionBySessionName(string name, out SocialSession session);
	}
	
	/// <summary>
	/// A thread-safe cache for <see cref="SocialSession"/>s.
	/// </summary>
	public class SocialSessionCache : ISocialSessionCacheAccessor, IDisposable, IEnumerable<SocialSession>
	{
		#region Constants and Fields

		/// <summary>
		/// reader writer lock
		/// </summary>
		private readonly ReaderWriterLockSlim rwLock;

		/// <summary>
		/// players collection
		/// </summary>
		private readonly Dictionary<int, SocialSession> sessionsBySessionId;

		/// <summary>
		/// players collection
		/// </summary>
		private readonly Dictionary<string, SocialSession> sessionsBySessionName;

		/// <summary>
		/// max lock ms
		/// </summary>
		private readonly int lockMs;

		#endregion

		#region Construtors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="SocialSessionCache"/> class.
		/// </summary>
		public SocialSessionCache(int maxLockMilliseconds)
		{
			this.lockMs = maxLockMilliseconds;
			this.rwLock = new ReaderWriterLockSlim();

			this.sessionsBySessionId = new Dictionary<int, SocialSession>();
			this.sessionsBySessionName = new Dictionary<string, SocialSession>(StringComparer.CurrentCultureIgnoreCase);
		}

		~SocialSessionCache()
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
				this.sessionsBySessionName.Clear();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a(n) <see cref="SocialSession"/> to the cache
		/// </summary>
		public bool AddSession(SocialSession session)
		{
			if (!this.rwLock.TryEnterWriteLock(this.lockMs))
				return false;

			try
			{
				if (!sessionsBySessionName.ContainsKey(session.Name))
				{
					this.sessionsBySessionName.Add(session.Name, session);
					this.sessionsBySessionId.Add(session.SessionId, session);
					return true;
				}
				return false;
			}
			finally
			{
				this.rwLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Removes a(n) <see cref="SocialSession"/> from the cache
		/// </summary>
		public bool RemoveSession(SocialSession session)
		{
			if (!this.rwLock.TryEnterWriteLock(this.lockMs))
				return false;

			try
			{
				if (sessionsBySessionName.Remove(session.Name))
				{
					this.sessionsBySessionId.Remove(session.SessionId);
					return true;
				}
				return false;
			}
			finally
			{
				this.rwLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Retrieves a(n) <see cref="SocialSession"/> by session id
		/// </summary>
		public bool TryGetSessionBySessionId(int sessionId, out SocialSession session)
		{
			this.rwLock.EnterReadLock();
			try
			{
				return this.sessionsBySessionId.TryGetValue(sessionId, out session);
			}
			finally
			{
				this.rwLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Retrieves a(n) <see cref="SocialSession"/> by session name
		/// </summary>
		public bool TryGetSessionBySessionName(string name, out SocialSession session)
		{
			this.rwLock.EnterReadLock();
			try
			{
				return this.sessionsBySessionName.TryGetValue(name, out session);
			}
			finally
			{
				this.rwLock.ExitReadLock();
			}
		}

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		/// Gets the Enumerator
		/// </summary>
		public IEnumerator<SocialSession> GetEnumerator()
		{
			return this.sessionsBySessionId.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.sessionsBySessionId.Values.GetEnumerator();
		}

		#endregion
	}
}
