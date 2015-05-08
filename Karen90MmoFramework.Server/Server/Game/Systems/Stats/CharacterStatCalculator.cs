using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public abstract class CharacterStatCalculator : IStatCalculator<short>
	{
		#region Implementation of IStatCalculator<out short>

		/// <summary>
		/// Calculates the resultant value of a <see cref="Stats"/>.
		/// </summary>
		public abstract short CalculateValue(Character character, Stats stat);

		#endregion
	}
}
