using Models.Statistic;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Data.Entity;

namespace Data.Repository.Interfaces
{
    public interface IProblemRepository : IRepository<Problem>
    {
        Problem GetProblemDetail(Expression<Func<Problem, bool>> expression);
        Problem GetProblemWithTestcase(Expression<Func<Problem, bool>> expression);
        IEnumerable<Problem> GetNewProblem(DateTime date, Expression<Func<Problem, bool>> expression = null);
        IEnumerable<Problem> GetProblemDetailMulti(Expression<Func<Problem, bool>> expression = null);
        IEnumerable<Problem> GetProblemWithSubmission();
        IEnumerable<ProblemStatistic> GetHotProblemInDay(DateTime date, Expression<Func<Problem, bool>> expression = null);
        IEnumerable<ProblemStatistic> GetHotProblem(Expression<Func<Problem, bool>> expression = null);
    }
}
