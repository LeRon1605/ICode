using Data;
using Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public class SubmissionRepository: BaseRepository<Submission>, ISubmissionRepository
    {
        public SubmissionRepository(ICodeDbContext context): base(context)
        {

        }

        public Submission GetSubmissionDetailSingle(Expression<Func<Submission, bool>> expression)
        {
            return _context.Submissions.Include(x => x.SubmissionDetails).FirstOrDefault(expression);
        }

        public IEnumerable<Submission> GetSubmissionsDetail(Expression<Func<Submission, bool>> expression = null)
        {
            if (expression == null)
                return _context.Submissions.Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase);
            else
                return _context.Submissions.Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).Where(expression);
        }

        public IEnumerable<Submission> GetSubmissionsOfProblem(string problemID, Expression<Func<Submission, bool>> expression = null)
        {
            if (expression == null)
                return _context.Submissions.Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).Where(x => x.SubmissionDetails.Any(s => s.TestCase.ProblemID == problemID));
            else
                return _context.Submissions.Include(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).Where(expression).Where(x => x.SubmissionDetails.Any(s => s.TestCase.ProblemID == problemID));

        }
    }
}
