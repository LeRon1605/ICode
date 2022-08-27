using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Data.Entity;

namespace API.Repository
{
    public interface ISubmissionRepository : IRepository<Submission>
    {
        Submission GetSubmissionDetailSingle(Expression<Func<Submission, bool>> expression);
        IEnumerable<Submission> GetSubmissionsDetail(Expression<Func<Submission, bool>> expression = null);
        IEnumerable<Submission> GetSubmissionsOfProblem(string problemID, Expression<Func<Submission, bool>> expression = null);
    }
}
