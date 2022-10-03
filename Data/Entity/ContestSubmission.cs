using Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class ContestSubmission
    {
        public string SubmitID { get; set; }
        public string ContestID { get; set; }
        public virtual Submission Submission { get; set; }
        public virtual Contest Contest { get; set; }
    }
}
