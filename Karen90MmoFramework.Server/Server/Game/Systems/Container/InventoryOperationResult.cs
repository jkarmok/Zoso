namespace Karen90MmoFramework.Server.Game.Systems
{
	public enum InventoryOperationResult : byte
	{
		/// <summary>
		/// Success
		/// </summary>
		Success,

		/// <summary>
		/// Fail
		/// </summary>
		Fail,

		/// <summary>
		/// Added partially
		/// </summary>
		PartialAdditionDueToSpace,

		/// <summary>
		/// Removed partialy
		/// </summary>
		PartialRemoval,
		
		/// <summary>
		/// Container full
		/// </summary>
		NoAdditionDueToSpace,

		/// <summary>
		/// No item found
		/// </summary>
		NoRemovalDueToItemNotFound,
		
		/// <summary>
		/// Operation not allowed
		/// </summary>
		NotAllowed,
	};
}
