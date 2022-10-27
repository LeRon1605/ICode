using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Statistic
{
    public class ProblemStatistic
    {
        public int SubmitCount { get; set; }
        public int SubmitSuccessCount { get; set; }
        public ProblemDTO problem { get; set; }
    }
}
