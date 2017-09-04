using System;
using System.Collections.Generic;

namespace Common {
	[Serializable]
	public class Room {
		public string Name;
		public int ID;
		public List<Client> Clients = new List<Client>();
		public Client Owner;

		public Room (string name , int id , Client owner) {
			Name = name;
			ID = id;
			Owner = owner;
		}
	}
}
