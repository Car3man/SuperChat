using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.RequestData;
using Common.ResponseData;

namespace ChatServer.RequestHandlers {
	public static class Request_CreateRoomHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
            BaseResponseData responseData = null;

			try {
                CreateRoomData createRoomData = Utils.ToObjectFromBytes<CreateRoomData>(data);

				if (!CreateRoom(createRoomData.Name, client , server)) {
                    responseData = new BaseResponseData(RequestTypes.CreateRoom, RequestResult.RoomExist);
                    server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);
                    return;
				}

                responseData = new BaseResponseData(RequestTypes.CreateRoom, RequestResult.Ok);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);

                Request_EnterInRoomHandler.AddClientInRoom(client , server.GetRoomByName(createRoomData.Name).ID , server);
			} catch (Exception e) {
                responseData = new BaseResponseData(RequestTypes.CreateRoom, RequestResult.Error);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);

                Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}

		public static bool CreateRoom (string name , Client client , Server server) {
			if (server.GetRoomByName(name) != null) return false;

			Room room = new Room(name , server.Rooms.Count , client);
			server.Rooms.Add(room);

			Console.WriteLine(string.Format("Room created with name {0} and ID {1}" , name , server.Rooms.Count - 1));
			return true;
		}
	}
}
