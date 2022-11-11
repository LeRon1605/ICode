using Data;
using Data.Entity;
using Data.Repository;
using ICode.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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

        public Contest GetContestWithPlayer(Expression<Func<Contest, bool>> expression = null)
        {
            if (expression == null)
            {
                return _context.Contests.Include(x => x.ContestDetails).ThenInclude(x => x.User).FirstOrDefault();
            }
            else
            {
                return _context.Contests.Include(x => x.ContestDetails).ThenInclude(x => x.User).FirstOrDefault(expression);
            }
        }

        public IEnumerable<Contest> GetContestWithPlayerMulti(Expression<Func<Contest, bool>> expression = null)
        {
            if (expression == null)
            {
                return _context.Contests.Include(x => x.ContestDetails).ThenInclude(x => x.User);
            }
            else
            {
                return _context.Contests.Include(x => x.ContestDetails).ThenInclude(x => x.User).Where(expression);
            }
        }

        public IEnumerable<Contest> GetContestWithProblemMulti(Expression<Func<Contest, bool>> expression)
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

        public Contest GetContestWithProblem(Expression<Func<Contest, bool>> expression)
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
