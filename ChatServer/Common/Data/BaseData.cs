using System;

namespace Common.Data {
    [Serializable]
    public class BaseData {
        public byte Type;
        public byte[] Data;

        public BaseData(byte type, byte[] data) {
            Type = type;
            Data = data;
        }
    }
}
