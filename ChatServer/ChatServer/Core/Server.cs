using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Net.NetworkInformation;
using System.Linq;
using Common;

namespace ChatServer {
	public class Server {
		private int port = 6969;

		private Thread serverThread;
		private List<Thread> clientThreads = new List<Thread>();
		private List<Client> clients = new List<Client>();
		private List<Room> rooms = new List<Room>();

		public Server (int port) {
			this.port = port;
		}

		public void StartServer () {
			serverThread = new Thread(ServerWorker);
			serverThread.Start();
		}

		public void StopServer () {
			if (serverThread != null) {
				serverThread.Abort();
				Console.WriteLine("Server was stopped");
			}
		}

		private void ServerWorker () {
			Console.WriteLine("Server started");

			TcpListener server = null;

			IPAddress localAddr = IPAddress.Parse("127.0.0.1");
			server = new TcpListener(localAddr , port);
			server.Start();

			while (Thread.CurrentThread.IsAlive) {
				TcpClient client = server.AcceptTcpClient();

				Console.WriteLine("Client connected");

				clients.Add(new Client(client));

				clientThreads.Add(new Thread(() => ClientThread(clients[clients.Count - 1])));
				clientThreads[clientThreads.Count - 1].Start();

				Thread.Sleep(100);
			}
		}

		private void ClientThread (Client client) {
			byte[] buffer = new byte[4096];

			NetworkStream stream = client.CurrentClient.GetStream();

			int bytesCount = 0;

			while (IsClientConnected(client.CurrentClient)) {
				if (stream.DataAvailable) {
					bytesCount = stream.Read(buffer , 0 , buffer.Length);

					if (bytesCount > 0) {
						HandleOperation(client , buffer);
						buffer = new byte[4096];
					}

					Thread.Sleep(100);
				}
			}

			Console.WriteLine("Client was disconnected");
		}

		private void HandleOperation (Client client , byte[] data) {
			OperationCodes code = (OperationCodes)data[0];

			switch (code) {
				case OperationCodes.Login:
					string nickname = Encoding.UTF8.GetString(data , 1 , data.Length - 1);
					client.Nickname = nickname;

					List<byte> loginResponseData = new List<byte>();

					loginResponseData.Add((byte)OperationCodes.Login);
					loginResponseData.Add((byte)ResponseCode.Ok);

					client.CurrentClient.GetStream().Write(loginResponseData.ToArray() , 0 , loginResponseData.Count);

					Console.WriteLine(string.Format("Client login with Nickname {0}" , nickname));
					break;
				case OperationCodes.Ping:
					Console.WriteLine(string.Format("Client was ping with remote end point - {0}" , client.CurrentClient.Client.RemoteEndPoint.ToString()));
					break;
				case OperationCodes.SendMessage:
					Console.WriteLine("Message from client: " + Encoding.UTF8.GetString(data , 5 , data.Length - 1));

					int roomIDSendMessage = BitConverter.ToInt32(data , 1);

					SendAll(data , roomIDSendMessage);
					break;
				case OperationCodes.EnterInRoom:
					string roomNameEnter = Encoding.UTF8.GetString(data , 1 , data.Length - 1);

					AddClientInRoom(client , GetRoomByName(roomNameEnter).ID);
					break;
				case OperationCodes.CreateRoom:
					string nameNewRoom = Encoding.UTF8.GetString(data , 1 , data.Length - 1);

					CreateRoom(nameNewRoom);
					break;
			}
		}

		public bool IsClientConnected (TcpClient client) {
			if (client.Client.Connected) {
				if ((client.Client.Poll(0 , SelectMode.SelectWrite)) && (!client.Client.Poll(0 , SelectMode.SelectError))) {
					byte[] buffer = new byte[1];
					if (client.Client.Receive(buffer , SocketFlags.Peek) == 0) {
						return false;
					} else {
						return true;
					}
				} else {
					return false;
				}
			} else {
				return false;
			}
		}

		private void AddClientInRoom (Client client , int roomID) {
			Room room = GetRoomByID(roomID);

			if (room != null) {
				if (!room.Clients.Contains(client)) {
					room.Clients.Add(client);
					Console.WriteLine(string.Format("Client was enter in room with ID {0}" , roomID));

					List<byte> data = new List<byte>();

					data.Add((byte)OperationCodes.EnterInRoom);
					data.AddRange(BitConverter.GetBytes(roomID));

					client.CurrentClient.GetStream().Write(data.ToArray() , 0 , data.Count);
				} else {
					Console.WriteLine(string.Format("Client already enter in room with ID {0}" , roomID));
				}
			} else {
				Console.WriteLine(string.Format("Room with ID {0} not exist" , roomID));
			}
		}

		private void RemoveClientFromRoom (Client client , int roomID) {
			Room room = GetRoomByID(roomID);
			if (room != null && room.Clients.Contains(client)) {
				room.Clients.Remove(client);
			}
		}

		private void CreateRoom (string name) {
			if (rooms.Where(x => x.Name.Equals(name)).Count() > 0) return;

			Room room = new Room(name , rooms.Count);
			rooms.Add(room);

			Console.WriteLine(string.Format("Room created with name {0} and ID {1}" , name , rooms.Count - 1));
		}

		private void RemoveRoom (int id) {
			Room room = GetRoomByID(id);
			if (room != null) {
				rooms.Remove(room);
			}
		}

		private void SendAll (byte[] data , int roomID) {
			NetworkStream stream = null;

			Room room = GetRoomByID(roomID);

			if (room == null) {
				Console.WriteLine(string.Format("Room with ID: {0} not exist" , roomID));
				return;
			}

			foreach (Client c in room.Clients) {
				if (c.CurrentClient.Connected) {
					stream = c.CurrentClient.GetStream();
					stream.Write(data , 0 , data.Length);
				}
			}
		}

		private Room GetRoomByID (int id) {
			Room[] rs = rooms.Where(x => x.ID.Equals(id)).ToArray();
			if (rs != null && rs.Length > 0) {
				return rs[0];
			}
			return null;
		}

		private Room GetRoomByName (string name) {
			Room[] rs = rooms.Where(x => x.Name.Equals(name)).ToArray();
			if (rs != null && rs.Length > 0) {
				return rs[0];
			}
			return null;
		}
	}
}
