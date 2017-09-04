using System;
namespace Common {
	public static class Utils {
		public static T[] SubArray<T> (T[] data , int index , int length) {
			T[] result = new T[length];
			Array.Copy(data , index , result , 0 , length);
			return result;
		}
	}
}
