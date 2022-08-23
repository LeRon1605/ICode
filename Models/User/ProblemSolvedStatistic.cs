using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ProblemSolvedStatisticDetail
    {
        public SubmissionDTO Submit { get; set; }
        public ProblemDTO Problem { get; set; }
    }
    public class ProblemSolvedStatistic
    {
        public UserDTO User { get; set; }
        public IEnumerable<ProblemSolvedStatisticDetail> Details { get; set; }
    }
}
