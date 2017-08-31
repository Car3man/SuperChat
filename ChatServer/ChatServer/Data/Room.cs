using System;
using System.Collections.Generic;
using Common;

namespace ChatServer {
	public class Room {
		public string Name;
		public int ID;
		public List<Client> Clients = new List<Client>();

		public Room (string name , int id) {
			Name = name;
			ID = id;
		}
	}
}
