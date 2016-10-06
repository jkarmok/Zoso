using System;

using PhotonHostRuntimeInterfaces;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;

namespace Karen90MmoFramework.Server.Core
{
	public class OutgoingServerToServerPeer : S2SPeerBase
	{
		#region Constants and Fields

		/// <summary>
		/// the logger
		/// </summary>
		protected static readonly ExitGames.Logging.ILogger Logger = ExitGames.Logging.LogManager.GetCurrentClassLogger();

		/// <summary>
		/// the peer handler
		/// </summary>
		private readonly IOutgoingServerPeer outgoingServerPeerHandler;

		#endregion

		#region Constructors and Destructors

		public OutgoingServerToServerPeer(InitResponse initResponse, IOutgoingServerPeer outgoingServerPeerHandler)
			: base(initResponse)
		{
			if (outgoingServerPeerHandler == null)
				throw new NullReferenceException("outgoingServerPeerHandler");

			this.outgoingServerPeerHandler = outgoingServerPeerHandler;
			this.outgoingServerPeerHandler.OnConnect(this);
		}

		#endregion

		#region OutboundS2SPeer Implementation

		protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
		{
			this.outgoingServerPeerHandler.OnEvent(this.Protocol, eventData, sendParameters);
		}

		protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
		{
			this.outgoingServerPeerHandler.OnOperationResponse(this.Protocol, operationResponse, sendParameters);
		}

		protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
		{
			this.outgoingServerPeerHandler.OnDisconnect(this);
		}

		protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
		{
			throw new System.NotImplementedException("Wont Ever be Called");
		}

		#endregion
	}
}
