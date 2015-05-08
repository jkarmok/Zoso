using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public class DefaultStatCalculator : CharacterStatCalculator
	{
		#region Overrides of CharacterStatCalculator

		/// <summary>
		/// Calculates the resultant value of a <see cref="Stats"/>.
		/// </summary>
		public override short CalculateValue(Character character, Stats stat)
		{
			return 0;
		}

		#endregion
	}
}
