using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Contest
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PlayerLimit { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<ContestDetail> ContestDetails { get; set; }
        public virtual ICollection<ProblemContestDetail> ProblemContestDetails { get; set; }
        public virtual ICollection<ContestSubmission> Submissions { get; set; }
    }
}
