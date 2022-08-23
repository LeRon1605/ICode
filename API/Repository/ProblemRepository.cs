using API.Models.Data;
using API.Models.Entity;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.EntityFrameworkCore;
using Models.Statistic;
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
        Problem GetProblemWithTestcase(Expression<Func<Problem, bool>> expression);
        IEnumerable<Problem> GetNewProblem(DateTime date, Expression<Func<Problem, bool>> expression = null);
        IEnumerable<Problem> GetProblemDetailMulti(Expression<Func<Problem, bool>> expression = null);
        IEnumerable<Problem> GetProblemWithSubmission();
        IEnumerable<ProblemStatistic> GetHotProblemInDay(DateTime date);
        IEnumerable<ProblemStatistic> GetHotProblem();
    }
    public class ProblemRepository: BaseRepository<Problem>, IProblemRepository
    {
        private readonly IMapper _mapper;
        public ProblemRepository(ICodeDbContext context, IMapper mapper): base(context)
        {
            _mapper = mapper;
        }

        public IEnumerable<ProblemStatistic> GetHotProblem()
        {
            return _context.Problems.Include(problem => problem.TestCases)
                                    .ThenInclude(testcase => testcase.SubmissionDetails)
                                    .ThenInclude(detail => detail.Submission)
                                    .Select(problem => new ProblemStatistic
                                    {
                                        problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                        SubmitCount = problem.TestCases.First().SubmissionDetails.Count(),
                                        SubmitSuccessCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.Status).Count()
                                    })
                                    .OrderByDescending(x => x.SubmitCount);
        }

        public IEnumerable<ProblemStatistic> GetHotProblemInDay(DateTime date)
        {
            return _context.Problems.Include(problem => problem.TestCases)
                                    .ThenInclude(testcase => testcase.SubmissionDetails)
                                    .ThenInclude(detail => detail.Submission)
                                    .Select(problem => new ProblemStatistic
                                    {
                                        problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                        SubmitCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date).Count(),
                                        SubmitSuccessCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date && x.Submission.Status).Count()
                                    })
                                    .OrderByDescending(x => x.SubmitCount);
        }

        public IEnumerable<Problem> GetNewProblem(DateTime date, Expression<Func<Problem, bool>> expression = null)
        {
            if (expression == null)
                return _context.Problems.Include(x => x.Tags).Where(x => x.CreatedAt.Date == date);
            else
                return _context.Problems.Include(x => x.Tags).Where(x => x.CreatedAt.Date == date).Where(expression);
        }

        public Problem GetProblemDetail(Expression<Func<Problem, bool>> expression)
        {
            return _context.Problems.Include(x => x.Tags).FirstOrDefault(expression);
        }

        public IEnumerable<Problem> GetProblemDetailMulti(Expression<Func<Problem, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Problems.Include(x => x.Tags);
            }
            else
            {
                return _context.Problems.Include(x => x.Tags).Where(expression);
            }
        }

        public Problem GetProblemWithTestcase(Expression<Func<Problem, bool>> expression)
        {
            return _context.Problems.Include(x => x.TestCases).FirstOrDefault(expression);
        }

        IEnumerable<Problem> IProblemRepository.GetProblemWithSubmission()
        {
            return _context.Problems.Include(x => x.TestCases).ThenInclude(x => x.SubmissionDetails);
        }
    }
}
