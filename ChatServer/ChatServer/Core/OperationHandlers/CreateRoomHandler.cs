using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace ChatServer.OperationHandlers {
	public static class CreateRoomHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
			List<byte> responseData = new List<byte>();

			try {
				string nameNewRoom = Encoding.UTF8.GetString(data , 1 , data.Length - 1);

				if (!CreateRoom(nameNewRoom , client , server)) {
					responseData.Add((byte)OperationCodes.CreateRoom);
					responseData.Add((byte)ResponseCode.RoomExist);

					client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);
					return;
				}

				responseData.Add((byte)OperationCodes.CreateRoom);
				responseData.Add((byte)ResponseCode.Ok);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);
			} catch (Exception e) {
				responseData.Add((byte)OperationCodes.CreateRoom);
				responseData.Add((byte)ResponseCode.Error);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);

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
