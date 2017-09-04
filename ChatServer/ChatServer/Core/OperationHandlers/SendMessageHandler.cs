using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace ChatServer.OperationHandlers {
	public static class SendMessageHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
			List<byte> responseData = new List<byte>();

			try {
				Console.WriteLine("Message from client: " + Encoding.UTF8.GetString(data , 1 , data.Length - 1));

				int roomIDSendMessage = client.CurrentRoomID;

				server.SendDataClientsInRoom(data , roomIDSendMessage);

				responseData.Add((byte)OperationCodes.SendMessage);
				responseData.Add((byte)ResponseCode.Ok);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);
			} catch (Exception e) {
				responseData.Add((byte)OperationCodes.SendMessage);
				responseData.Add((byte)ResponseCode.Error);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);

				Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}
	}
}
