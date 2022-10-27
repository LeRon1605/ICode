using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class SubmissionDetailDTO
    {
        public float Time { get; set; }
        public float Memory { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
    }
}
