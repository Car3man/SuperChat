using System;

namespace Common.RequestData {
    [Serializable]
    public class LoginData : BaseRequestData {
        public string Nickname;
        public string Password;

        public LoginData(string nickname, string password) {
            Nickname = nickname;
            Password = password;
        }
    }
}
