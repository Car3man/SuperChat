using System;

namespace Common {
    [Serializable]
    public enum RequestTypes : byte {
        Register,
        Login,
        SendMessage,
        EnterInRoom,
        ExitFromRoom,
        CreateRoom,
        RemoveRoom,
        GetListRooms
    }
}
