using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace ChatServer.OperationHandlers {
	public static class LoginHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
			List<byte> responseData = new List<byte>();

			try {
				string nickname = Encoding.UTF8.GetString(data , 1 , data.Length - 1);
				client.Nickname = nickname;

				responseData.Add((byte)OperationCodes.Login);
				responseData.Add((byte)ResponseCode.Ok);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);

				Console.WriteLine(string.Format("Client login with Nickname {0}" , nickname));
			} catch (Exception e) {
				responseData.Add((byte)OperationCodes.Login);
				responseData.Add((byte)ResponseCode.Error);

				client.CurrentClient.GetStream().Write(responseData.ToArray() , 0 , responseData.Count);

				Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}
	}
}
