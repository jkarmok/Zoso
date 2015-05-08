using System.Collections;
using Karen90MmoFramework.Rpc;
using Photon.SocketServer.Rpc;
using Photon.SocketServer;

namespace Karen90MmoFramework.Server.ServerToServer.Operations
{
	public class UpdateSocialProfile : Operation
	{
		public UpdateSocialProfile(IRpcProtocol protocol, OperationRequest operationRequest)
			: base(protocol, operationRequest)
		{
		}

		public UpdateSocialProfile()
		{
		}

		/// <summary>
		/// Session Id
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.SessionId)]
		public int SessionId { get; set; }
		
		/// <summary>
		/// Properties
		/// </summary>
		[DataMember(Code = (byte)ParameterCode.Properties)]
		public Hashtable Properties { get; set; }
	}
}
