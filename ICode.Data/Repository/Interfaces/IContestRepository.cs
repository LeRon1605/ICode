using Data.Entity;
using Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ICode.Data.Repository.Interfaces
{
    public interface IContestRepository: IRepository<Contest>
    {
        IEnumerable<Contest> GetContestWithPlayerMulti(Expression<Func<Contest, bool>> expression = null);
        IEnumerable<Contest> GetContestWithProblemMulti(Expression<Func<Contest, bool>> expression = null);

        Contest GetContestWithPlayer(Expression<Func<Contest, bool>> expression = null);
        Contest GetContestWithProblem(Expression<Func<Contest, bool>> expression = null);
    }
}
