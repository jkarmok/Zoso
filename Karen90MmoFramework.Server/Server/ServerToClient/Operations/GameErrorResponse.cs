namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class GameErrorResponse : GameOperationResponse
	{
		public GameErrorResponse(byte operationCode)
			: base(operationCode)
		{
		}

		public GameErrorResponse(byte operationCode, short returnCode)
			: base(operationCode)
		{
			ReturnCode = returnCode;
		}

		public GameErrorResponse(byte operationCode, short returnCode, string debugMessage)
			: base(operationCode)
		{
			ReturnCode = returnCode;
			DebugMessage = debugMessage;
		}
	}
}
