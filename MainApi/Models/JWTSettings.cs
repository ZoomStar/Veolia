using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainApi.Models
{
    public class JWTSettings
    {

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string SecretKey { get; set; }

    }
}
