using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace ChatServer.OperationHandlers {
	public static class EnterInRoomHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
			List<byte> responseData = new List<byte>();

			try {
				string roomNameEnter = Encoding.UTF8.GetString(data , 1 , data.Length - 1);

				Room room = server.GetRoomByName(roomNameEnter);

				if (room == null) {
					responseData.Add((byte)OperationCodes.EnterInRoom);
					responseData.Add((byte)ResponseCode.UnknowRoom);

					client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);
					return;
				}

				server.RemoveClientFromRoom(client , client.CurrentRoomID);

				AddClientInRoom(client , room.ID , server);

				Console.WriteLine("Client entered in room");
			} catch (Exception e) {
				responseData.Add((byte)OperationCodes.EnterInRoom);
				responseData.Add((byte)ResponseCode.Error);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);

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

					List<byte> data = new List<byte>();

					data.Add((byte)OperationCodes.EnterInRoom);
					data.Add((byte)ResponseCode.Ok);
					data.AddRange(Encoding.UTF8.GetBytes(client.Nickname.ToString()));

					server.SendDataClientsInRoom(data.ToArray() , roomID);
				} else {
					Console.WriteLine(string.Format("Client already enter in room with ID {0}" , roomID));
				}
			} else {
				Console.WriteLine(string.Format("Room with ID {0} not exist" , roomID));
			}
		}
	}
}
