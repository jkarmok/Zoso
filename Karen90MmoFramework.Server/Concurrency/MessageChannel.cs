using System;

using ExitGames.Concurrency.Channels;

namespace Karen90MmoFramework.Concurrency
{
	/// <summary>
	/// A channel where messages of type <see cref="T"/> can be published.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MessageChannel<T> : Channel<T>, IDisposable
	{
		/// <summary>
		/// Creates a new message channel
		/// </summary>
		public MessageChannel()
		{
		}

		~MessageChannel()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			this.ClearSubscribers();
		}
	}
}
