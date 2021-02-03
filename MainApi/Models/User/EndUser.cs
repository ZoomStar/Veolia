using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MainApi.Models
{
    public class EndUser
    {
        public int id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Level { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public string token { get; set; }
        public string error { get; set; }
        public object username { get; set; }
        public object password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public object email { get; set; }
        public object status { get; set; }
        public object level { get; set; }
        public EndUser(){}
        public EndUser(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }     
    }
    public class RefreshTok
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
