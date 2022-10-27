using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class ReportDTO
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDTO User { get; set; }
        public ProblemBase Problem { get; set; }
        public ReplyDTO Reply { get; set; }
    }
}
