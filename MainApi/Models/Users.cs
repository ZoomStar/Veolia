using System;
using System.Collections.Generic;

namespace MainApi.Models
{
    public partial class Users
    {
        public Users()
        {
            LoginHistory = new HashSet<LoginHistory>();
            Messages = new HashSet<Messages>();
            Tokens = new HashSet<Tokens>();
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public int Id { get; set; }
        public DateTime LastLogin { get; set; }

        public ICollection<LoginHistory> LoginHistory { get; set; }
        public ICollection<Messages> Messages { get; set; }
        public ICollection<Tokens> Tokens { get; set; }
    }
}
