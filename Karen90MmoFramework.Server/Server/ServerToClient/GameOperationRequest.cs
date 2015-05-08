using System;
using System.Collections.Generic;

namespace Karen90MmoFramework.Server.ServerToClient
{
	public class GameOperationRequest
	{
		private readonly int clientId;
		private readonly Dictionary<byte, object> parameters;

		private readonly bool isValid;

		/// <summary>
		/// Gets the operation code
		/// </summary>
		public byte OperationCode { get; set; }

		/// <summary>
		/// Gets the client id
		/// </summary>
		public int ClientId
		{
			get
			{
				return clientId;
			}
		}

		/// <summary>
		/// Gets the parameters
		/// </summary>
		public Dictionary<byte, object> Parameters
		{
			get
			{
				return parameters;
			}
		}

		/// <summary>
		/// Gets the value indicating whether the operation is valid
		/// </summary>
		public bool IsValid
		{
			get
			{
				return isValid;
			}
		}

		/// <summary>
		/// Creates a new instance of the <see cref="GameOperationRequest"/> class.
		/// </summary>
		/// <param name="parameters"></param>
		public GameOperationRequest(Dictionary<byte, object> parameters)
		{
			this.clientId = Convert.ToInt32(parameters[0]);
			this.OperationCode = Convert.ToByte(parameters[1]);
			this.isValid = parameters.Remove(0) && parameters.Remove(1);
			this.parameters = parameters;
		}
	}
}
