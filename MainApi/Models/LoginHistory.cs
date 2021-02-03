using System;
using System.Collections.Generic;

namespace MainApi.Models
{
    public partial class LoginHistory
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public DateTime LoginDateTime { get; set; }

        public Users IdUserNavigation { get; set; }
    }
}
