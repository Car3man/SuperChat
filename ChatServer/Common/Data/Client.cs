using System;
using System.Net.Sockets;

namespace Common {
	[Serializable]
	public class Client {
		[NonSerialized]
		public TcpClient CurrentClient;
		public string Nickname;
		public int CurrentRoomID = -1;

		public Client (TcpClient client) {
			CurrentClient = client;
		}
	}
}
