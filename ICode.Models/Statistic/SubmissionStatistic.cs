using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Statistic
{
    public class SubmissionStatistic
    {
        public UserDTO User { get; set; }
        public int SubmitCount { get; set; }
        public int SuccessSubmitCount { get; set; }
    }
}
