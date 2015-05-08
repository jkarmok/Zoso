﻿using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class SelectDialogue : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte) GameOperationCode.SelectDialogue;
			}
		}

		public SelectDialogue(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Dialogue Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.DialogueId)]
		public short DialogueId { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}