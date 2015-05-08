using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface IEngager
	{
		/// <summary>
		/// Filters the <see cref="mmoObject"/> for combat eligibility and takes appropriate action
		/// </summary>
		void ProcessThreat(MmoObject mmoObject);
	}
}
