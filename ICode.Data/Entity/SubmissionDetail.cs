using ICode.Common;

namespace Data.Entity
{
    public class SubmissionDetail
    {
        public string SubmitID { get; set; }
        public string TestCaseID { get; set; }
        public float Time { get; set; }
        public float Memory { get; set; }
        public SubmitState State { get; set; }
        public virtual Submission Submission { get; set; }
        public virtual TestCase TestCase { get; set; }
    }
}
