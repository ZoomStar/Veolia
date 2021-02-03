using System;
using System.Collections.Generic;

namespace MainApi.Models
{
    public partial class UsersWithToken : Users
    {
        public UsersWithToken(Users user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.Level = user.Level;
        }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Error { get; set; }
    }
}
