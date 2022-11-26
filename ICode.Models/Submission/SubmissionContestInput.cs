using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ICode.Models
{
    public class SubmissionContestInput: SubmissionInput
    {
        [Required]
        public string ProblemID { get; set; }
    }
}
