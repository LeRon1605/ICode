using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class ReplyDTO
    {
        public string ID { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
