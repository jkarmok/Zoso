using System;
using ExitGames.Logging;

namespace Karen90MmoFramework.Server
{
	public static class Utils
	{
		private static readonly Random _rnd = new Random();
		
		private static readonly ExitGames.Logging.ILogger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Gets the Random Number Generator
		/// </summary>
		public static Random Rnd
		{
			get
			{
				return _rnd;
			}
		}

		/// <summary>
		/// Gets the current class logger
		/// </summary>
		public static ExitGames.Logging.ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Generates a new Guid using the default <see cref="GuidCreationCulture"/> (<see cref="GuidCreationCulture.SplitGuid"/>)
		/// </summary>
		/// <returns></returns>
		public static int NewGuidInt32()
		{
			return NewGuidInt32(GuidCreationCulture.SplitGuid);
		}

		/// <summary>
		/// Generates a new Guid using a specific <see cref="GuidCreationCulture"/>.
		/// </summary>
		/// <returns></returns>
		public static int NewGuidInt32(GuidCreationCulture creationCulture)
		{
			switch (creationCulture)
			{
				// Hash
				default:
					{
						int id;
						do
						{
							var bytes = Guid.NewGuid().ToByteArray();
							id = BitConverter.ToInt32(bytes, bytes.Length - 4);
						}
						while (id < 0);

						return id;
					}

				case GuidCreationCulture.Utc:
					{
						return DateTime.UtcNow.Millisecond ^ _rnd.Next();
					}
			}
		}

		/// <summary>
		/// Generates a new Guid using the default <see cref="GuidCreationCulture"/> (<see cref="GuidCreationCulture.SplitGuid"/>)
		/// </summary>
		/// <returns></returns>
		public static long NewGuidInt64()
		{
			return NewGuidInt32(GuidCreationCulture.SplitGuid);
		}

		/// <summary>
		/// Generates a new Guid using a specific <see cref="GuidCreationCulture"/>.
		/// </summary>
		/// <returns></returns>
		public static long NewGuidInt64(GuidCreationCulture creationCulture)
		{
			switch (creationCulture)
			{
				// Hash
				default:
					{
						long id;
						do
						{
							var bytes = Guid.NewGuid().ToByteArray();
							id = BitConverter.ToInt64(bytes, bytes.Length - 8);
						}
						while (id < 0);

						return id;
					}

				case GuidCreationCulture.Utc:
					{
						return DateTime.UtcNow.Ticks ^ _rnd.Next();
					}
			}
		}
	}

	public enum GuidCreationCulture
	{
		/// <summary>
		/// Default Guid creation culture. Creates a unique Id using <see cref="Guid"/>. Subject to Id collision but works for general usage.
		/// </summary>
		SplitGuid,
		
		/// <summary>
		/// Uses the current time to create a unique Id. Looses uniqueness the longer the Id is used. Good for callback Ids.
		/// </summary>
		Utc,
	}
}
