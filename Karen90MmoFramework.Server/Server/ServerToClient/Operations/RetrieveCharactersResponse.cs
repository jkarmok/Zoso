﻿using Photon.SocketServer.Rpc;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Operations
{
	public class RetrieveCharactersResponse : GameOperationResponse
	{
		public RetrieveCharactersResponse(byte operationCode)
			: base(operationCode)
		{
		}

		/// <summary>
		/// Characters
		/// </summary>
		[DataMember(Code=(byte)ParameterCode.Data)]
		public CharacterStructure[] Characters { get; set; } 
	}
}
