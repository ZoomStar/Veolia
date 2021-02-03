using System;
using System.Collections.Generic;

namespace MainApi.Models
{
    public partial class Tokens
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

        public Users IdUserNavigation { get; set; }
    }
}
