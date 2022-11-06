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
        IEnumerable<Contest> GetDetailMulti(Expression<Func<Contest, bool>> expression = null);
        Contest GetDetailSingle(Expression<Func<Contest, bool>> expression = null);
    }
}
