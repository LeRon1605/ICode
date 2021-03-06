using API.Models.Data;
using API.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IProblemRepository: IRepository<Problem>
    {
        Problem GetProblemDetail(Expression<Func<Problem, bool>> expression);
        IEnumerable<Problem> GetProblemDetailMulti (Expression<Func<Problem, bool>> expression = null);
        Problem GetProblemWithTestcase(Expression<Func<Problem, bool>> expression);
    }
    public class ProblemRepository: BaseRepository<Problem>, IProblemRepository
    {
        public ProblemRepository(ICodeDbContext context): base(context)
        {

        }

        public Problem GetProblemDetail(Expression<Func<Problem, bool>> expression)
        {
            return _context.Problems.Include(x => x.Article).Include(x => x.Tags).FirstOrDefault(expression);
        }

        public IEnumerable<Problem> GetProblemDetailMulti(Expression<Func<Problem, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags);
            }
            else
            {
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Where(expression);
            }
        }

        public Problem GetProblemWithTestcase(Expression<Func<Problem, bool>> expression)
        {
            return _context.Problems.Include(x => x.TestCases).FirstOrDefault(expression);
        }
    }
}
