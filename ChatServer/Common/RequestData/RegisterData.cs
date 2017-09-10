using System;

namespace Common.RequestData {
    [Serializable]
    public class RegisterData : BaseRequestData {
        public string Name;
        public string Surname;
        public string Nickname;
        public string Email;
        public string City;
        public int Age;
        public string Password;

        public RegisterData(string name, string surname, string nickname, string email, string city, int age, string password) {
            Name = name;
            Surname = surname;
            Nickname = nickname;
            Email = email;
            City = city;
            Age = age;
            Password = password;
        }
    }
}
