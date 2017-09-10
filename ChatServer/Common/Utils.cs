using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common {
	public static class Utils {
        public static T[] SubArray<T> (T[] data , int index , int length) {
			T[] result = new T[length];
			Array.Copy(data , index , result , 0 , length);
			return result;
		}

        public static byte[] ToByteArray<T>(T obj) {
            MemoryStream m = new MemoryStream();
            if(obj != null) {
                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(m, obj);
            }
            return m.ToArray();
        }

        public static T ToObjectFromBytes<T>(byte[] arrBytes) {
            if(arrBytes == null || arrBytes.Length < 1)
                return default(T);

            BinaryFormatter binForm = new BinaryFormatter();

            T obj = (T)binForm.Deserialize(new MemoryStream(arrBytes));

            return obj;
        }
    }
}
