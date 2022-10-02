using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class ContestSubmission
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserID { get; set; }
        public string ContestID { get; set; }
        public virtual User User { get; set; }
        public virtual Contest Contest { get; set; }
        public virtual ICollection<SubmissionDetail> SubmissionDetails { get; set; }
    }
}
