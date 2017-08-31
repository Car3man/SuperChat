using System;
using System.Net.Sockets;

namespace Common {
	public class Client {
		public TcpClient CurrentClient;
		public string Nickname;
		public int CurrentRoomID = -1;

		public Client (TcpClient client) {
			CurrentClient = client;
		}
	}
}
