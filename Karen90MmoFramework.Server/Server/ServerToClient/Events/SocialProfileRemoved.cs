﻿using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;

namespace Karen90MmoFramework.Server.ServerToClient.Events
{
	public class SocialProfileRemoved : GameEvent
	{
		/// <summary>
		/// Gets the event code
		/// </summary>
		public override byte EventCode
		{
			get
			{
				return (byte)ClientEventCode.SocialProfileRemoved;
			}
		}

		/// <summary>
		/// Name of profile
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.CharacterName)]
		public string NameOfProfile { get; set; }
	}
}