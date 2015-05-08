namespace Karen90MmoFramework.Server.ServerToClient
{
	public abstract class GameOperationResponse
	{
		protected GameOperationResponse(byte operationCode)
		{
			OperationCode = operationCode;
		}

		/// <summary>
		/// Gets the operation code
		/// </summary>
		public byte OperationCode { get; private set; }

		/// <summary>
		/// Gets or sets the return code
		/// </summary>
		public short ReturnCode { get; set; }

		/// <summary>
		/// Gets or sets the debug message
		/// </summary>
		public string DebugMessage { get; set; }
	}
}
