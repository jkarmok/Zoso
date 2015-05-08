namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface ISpellController
	{
		/// <summary>
		/// Adds a spell update event
		/// </summary>
		/// <param name="spell"></param>
		void AddSpellUpdateEvent(Spell spell);

		/// <summary>
		/// Removes a spell update event
		/// </summary>
		/// <param name="spell"></param>
		void RemoveSpellUpdateEvent(Spell spell);

		/// <summary>
		/// Called from a <see cref="Spell"/> when it is started casting.
		/// </summary>
		void OnBeginCasting(Spell spell);

		/// <summary>
		/// Called by a <see cref="Spell"/> when it is finished casting.
		/// </summary>
		void OnEndCasting(Spell spell, bool isInterupt);

		/// <summary>
		/// Called by a <see cref="Spell"/> when its cooldown has started.
		/// </summary>
		/// <param name="spell"></param>
		void OnBeginCooldown(Spell spell);

		/// <summary>
		/// Called by a <see cref="Spell"/> when its cooldown has ended.
		/// </summary>
		/// <param name="spell"></param>
		void OnEndCooldown(Spell spell);
	}
}
