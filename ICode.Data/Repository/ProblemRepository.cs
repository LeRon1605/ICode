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
                                        .Include(problem => problem.Submissions)
                                        .Select(problem => new ProblemStatistic
                                        {
                                            problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                            SubmitCount = problem.Submissions.Count(),
                                            SubmitSuccessCount = problem.Submissions.Where(x => x.State == SubmitState.Success).Count()
                                        })
                                        .OrderByDescending(x => x.SubmitCount);
            }
            else
            {
                return _context.Problems.AsNoTracking()
                                        .Include(problem => problem.Article)
                                        .Include(problem => problem.Tags)
                                        .Include(problem => problem.Submissions)
                                        .Where(expression)
                                        .Select(problem => new ProblemStatistic
                                        {
                                            problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                            SubmitCount = problem.Submissions.Count(),
                                            SubmitSuccessCount = problem.Submissions.Where(x => x.State == SubmitState.Success).Count()
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
                                        .Include(problem => problem.Submissions)
                                        .Where(problem => problem.CreatedAt.Date < date.Date && problem.Submissions.Where(x => x.CreatedAt.Date == date.Date).Count() > 0)
                                        .Select(problem => new ProblemStatistic
                                        {
                                            problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                            SubmitCount = problem.Submissions.Where(x => x.CreatedAt.Date == date.Date).Count(),
                                            SubmitSuccessCount = problem.Submissions.Where(x => x.CreatedAt.Date == date.Date && x.State == SubmitState.Success).Count()
                                        })
                                        .OrderByDescending(x => x.SubmitCount);
            }
            else
            {
                return _context.Problems.AsNoTracking()
                                       .Include(problem => problem.Article)
                                       .Include(problem => problem.Tags)
                                       .Include(problem => problem.Submissions)
                                       .Where(problem => problem.CreatedAt.Date < date.Date && problem.Submissions.Where(x => x.CreatedAt.Date == date.Date).Count() > 0)
                                       .Where(expression)
                                       .Select(problem => new ProblemStatistic
                                       {
                                            problem = _mapper.Map<Problem, ProblemDTO>(problem),
                                            SubmitCount = problem.Submissions.Where(x => x.CreatedAt.Date == date.Date).Count(),
                                            SubmitSuccessCount = problem.Submissions.Where(x => x.CreatedAt.Date == date.Date && x.State == SubmitState.Success).Count()
                                       })
                                       .OrderByDescending(x => x.SubmitCount);
            }
            
        }

        public IEnumerable<Problem> GetNewProblem(DateTime date, Expression<Func<Problem, bool>> expression = null)
        {
            if (expression == null)
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Include(x => x.Submissions).Where(x => x.CreatedAt.Date == date);
            else
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Include(x => x.Submissions).Where(x => x.CreatedAt.Date == date).Where(expression);
        }

        public Problem GetProblemDetail(Expression<Func<Problem, bool>> expression)
        {
            return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Include(x => x.Submissions).FirstOrDefault(expression);
        }

        public IEnumerable<Problem> GetProblemDetailMulti(Expression<Func<Problem, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Include(x => x.Submissions);
            }
            else
            {
                return _context.Problems.Include(x => x.Article).Include(x => x.Tags).Include(x => x.Submissions).Where(expression);
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
