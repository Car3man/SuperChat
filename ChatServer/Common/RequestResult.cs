using System;

namespace Common {
    [Serializable]
	public enum RequestResult : byte {
		Ok,
		Error,
		UnknowRoom,
		RoomExist
	}
}
