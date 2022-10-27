using AutoMapper;
using CodeStudy.Models;
using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using ICode.Common;
using Microsoft.EntityFrameworkCore;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class ProblemRepository: BaseRepository<Problem>, IProblemRepository
    {
        private readonly IMapper _mapper;
        public ProblemRepository(ICodeDbContext context, IMapper mapper): base(context)
        {
            _mapper = mapper;
        }

        public IEnumerable<ProblemStatistic> GetHotProblem(Expression<Func<Problem, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Problems.AsNoTracking()
                                        .Include(problem => problem.Article)
                                        .Include(problem => problem.Tags)
                                        .Include(problem => problem.TestCases)
                                        .ThenInclude(testcase => testcase.SubmissionDetails)
                                        .ThenInclude(detail => detail.Submission)
                                        .Where(problem => problem.TestCases.First().SubmissionDetails.Count() > 0)
                                        .Select(problem => new ProblemStatistic
                                        {
                                            problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                            SubmitCount = problem.TestCases.First().SubmissionDetails.Count(),
                                            SubmitSuccessCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.State == SubmitState.Success).Count()
                                        })
                                        .OrderByDescending(x => x.SubmitCount);
            }
            else
            {
                return _context.Problems.AsNoTracking()
                                        .Include(problem => problem.Article)
                                        .Include(problem => problem.Tags)
                                        .Include(problem => problem.TestCases)
                                        .ThenInclude(testcase => testcase.SubmissionDetails)
                                        .ThenInclude(detail => detail.Submission)
                                        .Where(problem => problem.TestCases.First().SubmissionDetails.Count() > 0)
                                        .Where(expression)
                                        .Select(problem => new ProblemStatistic
                                        {
                                            problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                            SubmitCount = problem.TestCases.First().SubmissionDetails.Count(),
                                            SubmitSuccessCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.State == SubmitState.Success).Count()
                                        })
                                        .OrderByDescending(x => x.SubmitCount);
            }
        }

        public IEnumerable<ProblemStatistic> GetHotProblemInDay(DateTime date, Expression<Func<Problem, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Problems.AsNoTracking()
                                        .Include(problem => problem.Article)
                                        .Include(problem => problem.Tags)
                                        .Include(problem => problem.TestCases)
                                        .ThenInclude(testcase => testcase.SubmissionDetails)
                                        .ThenInclude(detail => detail.Submission)
                                        .Where(problem => problem.CreatedAt.Date < date.Date && problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date).Count() > 0)
                                        .Select(problem => new ProblemStatistic
                                        {
                                            problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                            SubmitCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date).Count(),
                                            SubmitSuccessCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date && x.Submission.State == SubmitState.Success).Count()
                                        })
                                        .OrderByDescending(x => x.SubmitCount);
            }
            else
            {
                return _context.Problems.AsNoTracking()
                                       .Include(problem => problem.Article)
                                       .Include(problem => problem.Tags)
                                       .Include(problem => problem.TestCases)
                                       .ThenInclude(testcase => testcase.SubmissionDetails)
                                       .ThenInclude(detail => detail.Submission)
                                       .Where(problem => problem.CreatedAt.Date < date.Date && problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date).Count() > 0)
                                       .Where(expression)
                                       .Select(problem => new ProblemStatistic
                                       {
                                           problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                           SubmitCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date).Count(),
                                           SubmitSuccessCount = problem.TestCases.First().SubmissionDetails.Where(x => x.Submission.CreatedAt.Date == date.Date && x.Submission.State == SubmitState.Success).Count()
                                       })
                                       .OrderByDescending(x => x.SubmitCount);
            }
            
        }

        public IEnumerable<Problem> GetNewProblem(DateTime date, Expression<Func<Problem, bool>> expression = null)
        {
            if (expression == null)
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Where(x => x.CreatedAt.Date == date);
            else
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Where(x => x.CreatedAt.Date == date).Where(expression);
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

        public IEnumerable<Problem> GetProblemWithSubmission()
        {
            return _context.Problems.Include(x => x.TestCases).ThenInclude(x => x.SubmissionDetails);
        }
    }
}
