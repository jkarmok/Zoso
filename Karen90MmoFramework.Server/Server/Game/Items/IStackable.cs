namespace Karen90MmoFramework.Server.Game.Items
{
	public interface IStackable
	{
		/// <summary>
		/// Determines whether this item can be stacked
		/// </summary>
		bool IsStackable { get; }

		/// <summary>
		/// Gets the max stack count
		/// </summary>
		int MaxStack { get; }

		/// <summary>
		/// Gets the current stack count
		/// </summary>
		int StackCount { get; }

		/// <summary>
		/// Determines whether this item is at its max stack count
		/// </summary>
		bool IsStackMaxed { get; }

		/// <summary>
		/// Determines whether a certain amount can be stacked
		/// </summary>
		/// <param name="amount"> The amount to stack </param>
		/// <returns> Returns the result </returns>
		bool CanStack(int amount);

		/// <summary>
		/// Stacks a certain amount
		/// </summary>
		/// <param name="amount"> The amount to stack </param>
		/// <returns> Returns the number of items stacked </returns>
		int Stack(int amount);

		/// <summary>
		/// Determines whether a certain amount can be destacked
		/// </summary>
		/// <param name="amount"> The amount to destack </param>
		/// <returns> Returns the result </returns>
		bool CanDestack(int amount);

		/// <summary>
		/// Destacks a certain amount
		/// </summary>
		/// <param name="amount"> The amount to destack </param>
		/// <returns> Returns the number of items stacked </returns>
		int Destack(int amount);
	}
}
