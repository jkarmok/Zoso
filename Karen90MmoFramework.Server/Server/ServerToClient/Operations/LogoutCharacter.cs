﻿using Photon.SocketServer;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class LogoutCharacter : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte) GameOperationCode.LogoutCharacter;
			}
		}

		public LogoutCharacter(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}
