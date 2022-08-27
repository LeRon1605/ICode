using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class TestCase
    {
        public string ID { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public float TimeLimit { get; set; }
        public float MemoryLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ProblemID { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual ICollection<SubmissionDetail> SubmissionDetails { get; set; }
    }
}
