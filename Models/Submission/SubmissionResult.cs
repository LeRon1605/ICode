using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class SubmissionResult
    {
        public string ID { get; set; }
        public string Language { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
