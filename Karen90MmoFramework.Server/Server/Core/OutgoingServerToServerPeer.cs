using System;

using PhotonHostRuntimeInterfaces;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;

namespace Karen90MmoFramework.Server.Core
{
	public class OutgoingServerToServerPeer : ServerPeerBase
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

		public OutgoingServerToServerPeer(IRpcProtocol protocol, IPhotonPeer peer, IOutgoingServerPeer outgoingServerPeerHandler)
			: base(protocol, peer)
		{
			if (outgoingServerPeerHandler == null)
				throw new NullReferenceException("outgoingServerPeerHandler");

			this.outgoingServerPeerHandler = outgoingServerPeerHandler;
			this.outgoingServerPeerHandler.OnConnect(this);
		}

		#endregion

		#region ServerPeerBase Implementation

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
