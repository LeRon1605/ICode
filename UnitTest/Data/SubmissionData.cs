using Data.Common;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTest.Common;

namespace UnitTest.Data
{
    public class SubmissionData
    {
        public static Submission GetSubmission()
        {
            return new Submission
            {
                ID = "ID",
                Code = "Code",
                Language = "Language",
                UserID = UserConstant.ID,
                CreatedAt = DateTime.Now,
                State = SubmitState.Pending,
                SubmissionDetails = new List<SubmissionDetail>()
            };
        }
    }
}
