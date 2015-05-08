﻿using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class SpRemovePower : GameOperation
	{
		/// <summary>
		/// Gets the operation code
		/// </summary>
		public override byte OperationCode
		{
			get
			{
				return (byte) GameOperationCode.SpRemovePower;
			}
		}

		public SpRemovePower(IRpcProtocol protocol, GameOperationRequest request)
			: base(protocol, request)
		{
		}

		/// <summary>
		/// Value
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Value)]
		public int Value { get; set; }

		/// <summary>
		/// Item Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.ObjectId, IsOptional = true)]
		public long? ObjectId { get; set; }

		/// <summary>
		/// Generates an error response
		/// </summary>
		public override GameOperationResponse GetErrorResponse(short errorReturnCode, string message)
		{
			return new GameErrorResponse(OperationCode, errorReturnCode, message);
		}
	}
}