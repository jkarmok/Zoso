using System;

using ExitGames.Concurrency.Fibers;

using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Server.Game.Messages;

namespace Karen90MmoFramework.Server.Game
{
	public interface IRegion
	{
		/// <summary>
		/// Gets the coordinate.
		/// </summary>
		Vector3 Coordinate { get; }

		/// <summary>
		/// Gets the number of listeners
		/// </summary>
		int NumListeners { get; }

		/// <summary>
		/// Subscribes to the region
		/// </summary>
		/// <param name="fiber"></param>
		/// <param name="receive"></param>
		/// <returns></returns>
		IDisposable Subscribe(IFiber fiber, Action<RegionMessage> receive);
	}
}
