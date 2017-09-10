using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.RequestData;
using Common.ResponseData;

namespace ChatServer.RequestHandlers {
	public static class Request_LoginHandler {
		public static void DoHandle (Client client , byte[] data , Server server) {
            BaseResponseData responseData = null;

            try {
                LoginData loginData = Utils.ToObjectFromBytes<LoginData>(data);
				client.Nickname = loginData.Nickname;

                responseData = new BaseResponseData(RequestTypes.Login, RequestResult.Ok);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);

				Console.WriteLine(string.Format("Client login with Nickname {0}" , loginData.Nickname));
			} catch (Exception e) {
                responseData = new BaseResponseData(RequestTypes.Login, RequestResult.Error);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);

                Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}
	}
}
