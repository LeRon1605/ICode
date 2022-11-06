using Data;
using Data.Entity;
using Data.Repository;
using ICode.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ICode.Data.Repository
{
    public class ContestRepository: BaseRepository<Contest>, IContestRepository 
    {
        public ContestRepository(ICodeDbContext context): base(context)
        {
            
        }

        public IEnumerable<Contest> GetDetailMulti(Expression<Func<Contest, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Contests.Include(x => x.ProblemContestDetails).ThenInclude(x => x.Problem);
            }
            else
            {
                return _context.Contests.Include(x => x.ProblemContestDetails).ThenInclude(x => x.Problem).Where(expression);
            }
        }

        public Contest GetDetailSingle(Expression<Func<Contest, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Contests.Include(x => x.ProblemContestDetails).ThenInclude(x => x.Problem).FirstOrDefault();
            }
            else
            {
                return _context.Contests.Include(x => x.ProblemContestDetails).ThenInclude(x => x.Problem).Where(expression).FirstOrDefault();
            }
        }
    }
}
