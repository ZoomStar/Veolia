using System;
using System.Collections.Generic;

namespace MainApi.Models
{
    public partial class Messages
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string MessageBody { get; set; }
        public string CreateDate { get; set; }
        public int CreateBy { get; set; }
        public string DeepLinkAction { get; set; }
        public int ImportanceLevel { get; set; }

        public Users CreateByNavigation { get; set; }
    }
}
