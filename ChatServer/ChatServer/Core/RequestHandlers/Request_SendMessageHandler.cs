using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.RequestData;
using Common.EventData;
using Common.ResponseData;
using System.Threading;

namespace ChatServer.RequestHandlers {
	public static class Request_SendMessageHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
			BaseResponseData responseData = null;

			try {
                SendMessageData sendMessageData = Utils.ToObjectFromBytes<SendMessageData>(data);

				Console.WriteLine("Message from client: " + sendMessageData.Message);

				int roomIDSendMessage = client.CurrentRoomID;

                ChatMessageData chatMessageData = new ChatMessageData(client.Nickname, sendMessageData.Message);
                server.SendEventData(server.GetRoomByID(roomIDSendMessage).Clients, (byte)EventTypes.ChatMessage, chatMessageData);

                responseData = new BaseResponseData(RequestTypes.SendMessage, RequestResult.Ok);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);
            } catch (Exception e) {
                responseData = new BaseResponseData(RequestTypes.SendMessage, RequestResult.Ok);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);

                Console.WriteLine("Error {0} - {1}", e.Message, e.StackTrace);
            }
		}
	}
}
