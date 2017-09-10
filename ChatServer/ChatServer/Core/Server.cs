using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using Common;
using ChatServer.RequestHandlers;
using Common.Data;
using Common.EventData;
using Common.ResponseData;

namespace ChatServer {
	public class Server {
		private int port = 6969;

		private Thread serverThread;
		private List<Thread> clientThreads = new List<Thread>();

		public List<Client> Clients = new List<Client>();
		public List<Room> Rooms = new List<Room>();

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
			TcpListener server = null;

			string hostName = Dns.GetHostName();
            //string ip = Dns.GetHostEntry(hostName).AddressList[0].ToString();
            string ip = "192.168.0.87";

            Console.WriteLine("Server started, IP: " + ip);

			IPAddress localAddr = IPAddress.Parse(ip);
			server = new TcpListener(localAddr , port);
			server.Start();

			while (Thread.CurrentThread.IsAlive) {
				TcpClient client = server.AcceptTcpClient();

				Console.WriteLine("Client connected");

				Clients.Add(new Client(client));

				clientThreads.Add(new Thread(() => ClientThread(Clients[Clients.Count - 1])));
				clientThreads[clientThreads.Count - 1].Start();

				Thread.Sleep(100);
			}
		}

		private void ClientThread (Client client) {
			byte[] buffer = new byte[4096];
			byte[] trimBuffer;

			NetworkStream stream = client.CurrentClient.GetStream();

			int bytesCount = 0;

			while (IsClientConnected(client.CurrentClient)) {
				if (stream.DataAvailable) {
					bytesCount = stream.Read(buffer , 0 , buffer.Length);

					if (bytesCount > 0) {
						trimBuffer = new byte[bytesCount];
						Array.Copy(buffer , trimBuffer , bytesCount);

                        DataInfo dataInfo = Utils.ToObjectFromBytes<DataInfo>(trimBuffer);
                        buffer = new byte[dataInfo.Length];
                        bytesCount = stream.Read(buffer, 0, dataInfo.Length);

                        BaseData data = Utils.ToObjectFromBytes<BaseData>(buffer);

                        switch(dataInfo.Type) {
                            case DataInfo.Types.RequestData:
                                HandleRequest(client, data.Type, data.Data);
                                break;
                            case DataInfo.Types.ResponseData:
                                HandleResponse(client, data.Type, data.Data);
                                break;
                            default:
                                Console.WriteLine("Unknow data type");
                                break;
                        }

						buffer = new byte[4096];
					}

					Thread.Sleep(100);
				}
			}

			RemoveClientFromRoom(client , client.CurrentRoomID);
			RemoveUserRooms(client);
			Console.WriteLine(string.Format("Client with nickname {0} was disconnected" , client.Nickname));
		}

        private void HandleResponse(Client client, byte type, byte[] data) {
            RequestTypes code = (RequestTypes)type;

            switch(code) {
                default:
                    Console.WriteLine("Operation response '{0}'", code);
                    break;
            }
        }

        private void HandleRequest(Client client, byte type, byte[] data) {
            RequestTypes code = (RequestTypes)type;

            switch(code) {
                case RequestTypes.Login:
                    Request_LoginHandler.DoHandle(client, data, this);
                    break;
                case RequestTypes.SendMessage:
                    Request_SendMessageHandler.DoHandle(client, data, this);
                    break;
                case RequestTypes.EnterInRoom:
                    Request_EnterInRoomHandler.DoHandle(client, data, this);
                    break;
                case RequestTypes.CreateRoom:
                    Request_CreateRoomHandler.DoHandle(client, data, this);
                    break;
                case RequestTypes.GetListRooms:
                    Request_GetListRooms.DoHandle(client, data, this);
                    break;
                default:
                    Console.WriteLine("Operation unknow '{0}'", code);
                    break;
            }
        }

		public bool IsClientConnected (TcpClient client) {
            try {
                if(client.Client.Connected) {
                    if((client.Client.Poll(0, SelectMode.SelectWrite)) && (!client.Client.Poll(0, SelectMode.SelectError))) {
                        byte[] buffer = new byte[1];
                        if(client.Client.Receive(buffer, SocketFlags.Peek) == 0) {
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
            } catch(Exception e) {
                return false;
            }
		}

		#region Private methods

		private void RemoveUserRooms (Client client) {
			Rooms.RemoveAll(x => x.Owner == client);
		}

		#endregion

		#region Public methods

		public void RemoveClientFromRoom (Client client , int roomID) {
			Room room = GetRoomByID(roomID);
			if (room != null && room.Clients.Contains(client)) {
				room.Clients.Remove(client);

				if (room.Clients.Count == 0) {
					RemoveRoom(room.ID);
				}
			}
		}

		public void RemoveRoom (int id) {
			Room room = GetRoomByID(id);
			if (room != null) {
				Rooms.Remove(room);
			}
		}

		public void SendDataAllClients (byte[] data) {
			NetworkStream stream = null;

			foreach (Client c in Clients) {
				if (c.CurrentClient.Connected) {
					stream = c.CurrentClient.GetStream();
					stream.Write(data , 0 , data.Length);
				}
			}
		}

		public void SendDataClientsInRoom (byte[] data , int roomID) {
			NetworkStream stream = null;

			Room room = GetRoomByID(roomID);

			if (room == null) {
				Console.WriteLine(string.Format("Room with ID: {0} not exist" , roomID));
				return;
			}

			Console.WriteLine(room.Clients.Count);

			foreach (Client c in room.Clients) {
				if (c.CurrentClient.Connected) {
					stream = c.CurrentClient.GetStream();
					stream.Write(data , 0 , data.Length);
				}
			}
		}

		public Room GetRoomByID (int id) {
			foreach (Room r in Rooms) {
				if (r.ID.Equals(id)) {
					return r;
				}
			}
			return null;
		}

		public Room GetRoomByName (string name) {
			foreach (Room r in Rooms) {
				if (r.Name.Equals(name)) {
					return r;
				}
			}
			return null;
		}

        #endregion

        #region Core public methods

        public void SendEventData(List<Client> clients, byte type, BaseEventData data) {
            foreach(Client client in clients) {
                NetworkStream stream = client.CurrentClient.GetStream();

                BaseData d = new BaseData(type, Utils.ToByteArray(data));

                List<byte> bytesInfo = new List<byte>();
                List<byte> bytes = new List<byte>();

                bytes.AddRange(Utils.ToByteArray(d));
                bytesInfo.AddRange(Utils.ToByteArray(new DataInfo(DataInfo.Types.EventData, bytes.Count)));

                stream.Write(bytesInfo.ToArray(), 0, bytesInfo.Count);
                Thread.Sleep(50);
                stream.Write(bytes.ToArray(), 0, bytes.Count);
            }
        }

        public void SendResponseData(List<Client> clients, byte type, BaseResponseData data) {
            foreach(Client client in clients) {
                NetworkStream stream = client.CurrentClient.GetStream();

                BaseData d = new BaseData(type, Utils.ToByteArray(data));

                List<byte> bytesInfo = new List<byte>();
                List<byte> bytes = new List<byte>();

                bytes.AddRange(Utils.ToByteArray(d));
                bytesInfo.AddRange(Utils.ToByteArray(new DataInfo(DataInfo.Types.ResponseData, bytes.Count)));

                stream.Write(bytesInfo.ToArray(), 0, bytesInfo.Count);
                Thread.Sleep(50);
                stream.Write(bytes.ToArray(), 0, bytes.Count);
            }
        }

        #endregion
    }
}
