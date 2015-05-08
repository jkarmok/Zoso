using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface IStatCalculator<out T>
	{
		/// <summary>
		/// Calculates the resultant value of a <see cref="Stats"/>.
		/// </summary>
		T CalculateValue(Character character, Stats stat);
	}
}
