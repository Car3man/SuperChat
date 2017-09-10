using System;
using System.Collections.Generic;
using System.Text;

namespace Common.RequestData {
    [Serializable]
    public class CreateRoomData : BaseRequestData {
        public string Name;

        public CreateRoomData(string name) {
            Name = name;
        }
    }
}
