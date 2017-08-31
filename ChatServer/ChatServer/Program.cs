using System;
using System.Runtime.InteropServices;

namespace ChatServer {
	class MainClass {
		private static Server server;

		public static void Main (string[] args) {
			server = new Server(6969);
			server.StartServer();
		}
	}
}
