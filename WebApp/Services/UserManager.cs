using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainApi.Models;

namespace WebApp.Services
{
    public class UserManager
    {
        public UsersWithToken Login()
        {

            UsersWithToken user = new UsersWithToken(new Users());


            return user;
        }
    }
}
