using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Data.Entity;

namespace Data.Repository.Interfaces
{
    public interface ISubmissionRepository : IRepository<Submission>
    {
        Submission GetSubmissionDetailSingle(Expression<Func<Submission, bool>> expression);
        IEnumerable<Submission> GetSubmissionsDetail(Expression<Func<Submission, bool>> expression = null);
        IEnumerable<Submission> GetSubmissionsOfProblem(string problemID, Expression<Func<Submission, bool>> expression = null);
    }
}
