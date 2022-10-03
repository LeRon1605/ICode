using Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class Submission
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public SubmitState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ContestSubmission ContestSubmission { get; set; }
        public virtual ICollection<SubmissionDetail> SubmissionDetails { get; set; }
    }
}
