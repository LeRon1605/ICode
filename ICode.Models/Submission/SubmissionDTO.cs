using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class SubmissionDTO: SubmissionBase
    {
        public UserDTO User { get; set; }
        public ProblemBase Problem { get; set; }
    }
}
