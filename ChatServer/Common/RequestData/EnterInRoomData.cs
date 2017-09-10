using System;
using System.Collections.Generic;
using System.Text;

namespace Common.RequestData {
    [Serializable]
    public class EnterInRoomData : BaseRequestData {
        public string Name;

        public EnterInRoomData(string name) {
            Name = name;
        }
    }
}
