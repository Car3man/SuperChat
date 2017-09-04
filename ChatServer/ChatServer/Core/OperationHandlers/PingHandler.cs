using System;
using System.Collections.Generic;
using Common;

namespace ChatServer.OperationHandlers {
	public static class PingHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
			List<byte> responseData = new List<byte>();

			try {
				Console.WriteLine(string.Format("Client was ping with remote end point - {0}" , client.CurrentClient.Client.RemoteEndPoint.ToString()));

				responseData.Add((byte)OperationCodes.Ping);
				responseData.Add((byte)ResponseCode.Ok);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);
			} catch (Exception e) {
				responseData.Add((byte)OperationCodes.Ping);
				responseData.Add((byte)ResponseCode.Error);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);

				Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}
	}
}
