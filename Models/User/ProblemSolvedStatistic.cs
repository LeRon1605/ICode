using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ProblemSolvedStatisticDetail
    {
        public SubmissionBase Submit { get; set; }
        public ProblemBase Problem { get; set; }
    }
    public class ProblemSolvedStatistic
    {
        public UserDTO User { get; set; }
        public IEnumerable<ProblemSolvedStatisticDetail> Details { get; set; }
    }
}
