using CodeStudy.Models;
using ICode.Common;

namespace Models
{
    public class ProblemContest: ProblemBase
    {
        public int Score { get; set; }
        public Level Level { get; set; }
    }
}
