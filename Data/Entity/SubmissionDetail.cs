using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Entity
{
    public class SubmissionDetail
    {
        public string SubmitID { get; set; }
        public string TestCaseID { get; set; }
        public float Time { get; set; }
        public float Memory { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public virtual Submission Submission { get; set; }
        public virtual TestCase TestCase { get; set; }
    }
}
