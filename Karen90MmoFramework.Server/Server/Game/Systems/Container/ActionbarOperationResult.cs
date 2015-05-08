namespace Karen90MmoFramework.Server.Game.Systems
{
	public enum ActionbarOperationResult : byte
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
		/// Actionbar full
		/// </summary>
		ActionbarFull,
		
		/// <summary>
		/// Item not found
		/// </summary>
		ItemNotFound,
		
		/// <summary>
		/// Operation not allowed
		/// </summary>
		NotAllowed,
	};
}
