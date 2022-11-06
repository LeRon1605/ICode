using System.Collections.Generic;

namespace Models
{
    public class ContestDTO: ContestBase
    {
        public List<ProblemContest> Problems { get; set; }
    }
}
