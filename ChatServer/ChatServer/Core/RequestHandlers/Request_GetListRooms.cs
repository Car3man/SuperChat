using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Common;
using Common.RequestData;
using Common.ResponseData;

namespace ChatServer.RequestHandlers {
	public static class Request_GetListRooms {
		public static void DoHandle (Client client , byte[] data , Server server) {
			BaseResponseData responseData = null;

			try {
                GetListRoomsData getListRoomsData = Utils.ToObjectFromBytes<GetListRoomsData>(data);

                responseData = new BaseResponseData(RequestTypes.GetListRooms, RequestResult.Ok);
                responseData.AddValue("Rooms", Utils.ToByteArray(server.Rooms));
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);
            } catch (Exception e) {
                responseData = new BaseResponseData(RequestTypes.GetListRooms, RequestResult.Error);
                server.SendResponseData(new List<Client>() { client }, (byte)responseData.Request, responseData);

                Console.WriteLine("Error {0} - {1}" , e.Message , e.StackTrace);
			}
		}
	}
}
