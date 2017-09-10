using System;

namespace Common.Data {
    [Serializable]
    public class DataInfo {
        public enum Types { EventData, ResponseData, RequestData }

        public Types Type;
        public int Length;

        public DataInfo(Types type, int length) {
            Type = type;
            Length = length;
        }
    }
}
