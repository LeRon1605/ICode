using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class SubmissionDTO
    {
        public string ID { get; set; }
        public string Language { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserID { get; set; }
    }
}
