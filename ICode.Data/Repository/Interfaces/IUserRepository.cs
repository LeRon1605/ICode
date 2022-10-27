using Models.Statistic;
using Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using Data.Entity;

namespace Data.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserWithRole(Expression<Func<User, bool>> expression);
        User GetUserWithSubmit(Expression<Func<User, bool>> expression);
        Task<IEnumerable<Problem>> GetProblemSolvedByUser(string UserID, Func<Problem, bool> expression = null);
        IEnumerable<ProblemSolvedStatistic> GetProblemSolveStatisticOfUser();
        IEnumerable<User> GetNewUserInDay(DateTime Date, Expression<Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserActivityInDay(DateTime Date, int take = 5, Expression<Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserActivity(int take = 5, Expression<Func<User, bool>> expression = null);
    }
}
