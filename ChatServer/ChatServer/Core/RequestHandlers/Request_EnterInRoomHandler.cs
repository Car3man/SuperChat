using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.RequestData;
using Common.ResponseData;

namespace ChatServer.RequestHandlers {
	public static class Request_EnterInRoomHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
			BaseResponseData responseData = null;

			try {
                EnterInRoomData enterInRoomData = Utils.ToObjectFromBytes<EnterInRoomData>(data);

				Room room = server.GetRoomByName(enterInRoomData.Name);

				if (room == null) {
                    responseData = new BaseResponseData(RequestTypes.EnterInRoom, RequestResult.UnknowRoom);
                    server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);
                    return;
				}

				server.RemoveClientFromRoom(client , client.CurrentRoomID);

				AddClientInRoom(client , room.ID , server);

				Console.WriteLine("Client entered in room");
			} catch (Exception e) {
                responseData = new BaseResponseData(RequestTypes.EnterInRoom, RequestResult.Ok);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);

                Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}

		public static void AddClientInRoom (Client client , int roomID , Server server) {
			Room room = server.GetRoomByID(roomID);

			if (room != null) {
				if (!room.Clients.Contains(client)) {
					room.Clients.Add(client);

					client.CurrentRoomID = roomID;

					Console.WriteLine(string.Format("Client was enter in room with ID {0}" , roomID));

					//List<byte> data = new List<byte>();

					//data.Add((byte)RequestTypes.EnterInRoom);
					//data.Add((byte)RequestResult.Ok);
					//data.AddRange(Encoding.UTF8.GetBytes(client.Nickname.ToString()));

					//server.SendDataClientsInRoom(data.ToArray() , roomID);
				} else {
					Console.WriteLine(string.Format("Client already enter in room with ID {0}" , roomID));
				}
			} else {
				Console.WriteLine(string.Format("Room with ID {0} not exist" , roomID));
			}
		}
	}
}
