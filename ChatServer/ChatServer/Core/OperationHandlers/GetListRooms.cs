using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Common;

namespace ChatServer.OperationHandlers {
	public static class GetListRooms {
		public static void DoHandle (Client client , byte[] data , Server server) {
			List<byte> responseData = new List<byte>();

			try {
				responseData.Add((byte)OperationCodes.GetListRooms);
				responseData.Add((byte)ResponseCode.Ok);
				responseData.AddRange(ToByteArray(server.Rooms));

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);
			} catch (Exception e) {
				responseData.Add((byte)OperationCodes.GetListRooms);
				responseData.Add((byte)ResponseCode.Error);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);

				Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}

		private static byte[] ToByteArray (object source) {
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream()) {
				formatter.Serialize(stream , source);
				return stream.ToArray();
			}
		}
	}
}
