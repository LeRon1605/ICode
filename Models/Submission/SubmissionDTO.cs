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
        public UserDTO User { get; set; }
        public ProblemBase Problem { get; set; }
    }
}
