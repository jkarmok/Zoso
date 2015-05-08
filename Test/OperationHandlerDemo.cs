using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Karen90MmoTests
{
	class OperationHandlerDemo : IDemo
	{
		#region Implementation of IDemo

		private readonly Dictionary<byte, Action<OperationResponse>> responseHandlers = new Dictionary<byte, Action<OperationResponse>>();

		public void Run()
		{
			var d1 = this.RegisterOperationResponseHandler(0, (r) => Console.WriteLine("received message"));
			var d2 = this.RegisterOperationResponseHandler(0, (r) => Console.WriteLine("received message2"));

			d1.Dispose();

			responseHandlers[0](new OperationResponse());
		}

		public IDisposable RegisterOperationResponseHandler(byte operationCode, Action<OperationResponse> responseHandler)
		{
			var disposable = new SubscriptionDisposable(operationCode, responseHandler, this);

			Action<OperationResponse> existingHandler;
			if (responseHandlers.TryGetValue(operationCode, out existingHandler))
			{
				responseHandler = (Action<OperationResponse>) Delegate.Combine(existingHandler, responseHandler);
				responseHandlers[operationCode] = responseHandler;
				return disposable;
			}
			responseHandlers.Add(operationCode, responseHandler);
			return disposable;
		}

		void DeregisterOperationResponseHandler(byte operationCode, Action<OperationResponse> responseHandler)
		{
			Action<OperationResponse> existingHandler;
			if (responseHandlers.TryGetValue(operationCode, out existingHandler))
			{
				existingHandler = (Action<OperationResponse>)Delegate.RemoveAll(existingHandler, responseHandler);
				if (existingHandler != null)
				{
					responseHandlers[operationCode] = existingHandler;
					return;
				}

				responseHandlers.Remove(operationCode);
			}
		}

		#endregion

		class SubscriptionDisposable : IDisposable
		{
			private readonly byte operationCode;
			private readonly Action<OperationResponse> handler;
			private readonly OperationHandlerDemo demo;

			public SubscriptionDisposable(byte operationCode, Action<OperationResponse> action, OperationHandlerDemo demo)
			{
				this.operationCode = operationCode;
				this.demo = demo;
				this.handler = action;
			}

			#region Implementation of IDisposable

			/// <summary>
			/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose()
			{
				demo.DeregisterOperationResponseHandler(operationCode, handler);
			}

			#endregion
		}
	}
}
