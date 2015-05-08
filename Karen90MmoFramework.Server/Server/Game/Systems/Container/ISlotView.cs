namespace Karen90MmoFramework.Server.Game.Systems
{
	public interface ISlotView<out T>
	{
		/// <summary>
		/// Gets the item
		/// </summary>
		T Item { get; }

		/// <summary>
		/// Determines whether the slot is empty or not
		/// </summary>
		bool IsEmpty { get; }
	}
}
