using System;
using System.Collections.Generic;
using System.Text;

namespace MainApi.Models
{
    class UserRegistration
    {
        public int id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Password_rep { get; set; }
        public int Level { get; set; }

        public UserRegistration()
        {

        }
        public UserRegistration(string Email, string Password,string Password_rep)
        {
            this.Email = Email;
            this.Password = Password;
            this.Password_rep = Password_rep;
        }

        public bool CheckInformation()
        {
            if (!this.Email.Equals("") && !this.Password.Equals("") && !this.Password_rep.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }           
        }
        public bool CheckMatchPassword()
        {
            if (this.Password == this.Password_rep)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
