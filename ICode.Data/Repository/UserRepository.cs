using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Data.Repository.Interfaces;
using ICode.Common;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class UserRepository: BaseRepository<User>, IUserRepository
    {
        private readonly IMapper _mapper;
        public UserRepository(ICodeDbContext context, IMapper mapper): base(context)
        {
            _mapper = mapper;
        }

        public User GetDetail(Expression<Func<User, bool>> expression)
        {
            return _context.Users.Include(x => x.Submissions).ThenInclude(x => x.Problem).FirstOrDefault(expression);
        }

        public IEnumerable<User> GetNewUserInDay(DateTime Date, Expression<Func<User, bool>> expression)
        {
            if (expression == null)
                return _context.Users.Where(user => user.CreatedAt.Date == Date.Date);
            else
                return _context.Users.Where(user => user.CreatedAt.Date == Date.Date).Where(expression);
        }

        public Task<IEnumerable<Problem>> GetProblemSolvedByUser(string UserID, Func<Problem, bool> expression)
        {
            User user = _context.Users.Include(x => x.Submissions).ThenInclude(x => x.Problem).ThenInclude(x => x.Tags).FirstOrDefault(x => x.ID == UserID);
            if (user == null)
            {
                return null;
            }
            if (expression == null)
                return Task.FromResult(user.Submissions.Where(x => x.State == SubmitState.Success).Select(x => x.Problem).GroupBy(x => x.ID).Select(x => x.FirstOrDefault()));
            else
                return Task.FromResult(user.Submissions.Where(x => x.State == SubmitState.Success).Select(x => x.Problem).GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).Where(expression));
        }

        public IEnumerable<ProblemSolvedStatistic> GetProblemSolveStatisticOfUser()
        {
            return _context.Users.Include(x => x.Submissions).ThenInclude(x => x.Problem).Where(x => x.Submissions.Count > 0).AsEnumerable().Select(x => new ProblemSolvedStatistic
            {
                User = _mapper.Map<User, UserDTO>(x),
                Details = x.Submissions.Where(x => x.State == SubmitState.Success).GroupBy(x => x.Problem).Select(x => new ProblemSolvedStatisticDetail
                {
                    Problem = _mapper.Map<Problem, ProblemDTO>(x.Key),
                    Submit = _mapper.Map<Submission, SubmissionDTO>(x.OrderBy(x => x.CreatedAt).FirstOrDefault())
                })
            });
        }

        public IEnumerable<SubmissionStatistic> GetTopUserActivity(int take = 5, Expression<Func<User, bool>> expression = null)
        {
            if (expression == null)
            {
                return _context.Users.Include(user => user.Submissions)
                                     .Select(user => new SubmissionStatistic
                                     {
                                         User = _mapper.Map<User, UserDTO>(user),
                                         SubmitCount = user.Submissions.Count(),
                                         SuccessSubmitCount = user.Submissions.Where(submission => submission.State == SubmitState.Success).Count(),
                                     })
                                     .OrderByDescending(x => x.SuccessSubmitCount)
                                     .Take(take);
            }
            else
            {
                return _context.Users.Include(user => user.Submissions)
                                     .Where(expression)
                                     .Select(user => new SubmissionStatistic
                                     {
                                         User = _mapper.Map<User, UserDTO>(user),
                                         SubmitCount = user.Submissions.Count(),
                                         SuccessSubmitCount = user.Submissions.Where(submission => submission.State == SubmitState.Success).Count(),
                                     })
                                     .OrderByDescending(x => x.SuccessSubmitCount)
                                     .Take(take);
            }
        }

        public IEnumerable<SubmissionStatistic> GetTopUserActivityInDay(DateTime Date, int take, Expression<Func<User, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Users.Include(user => user.Submissions)
                                     .Where(user => user.CreatedAt.Date < Date.Date)
                                     .Select(user => new SubmissionStatistic
                                     {
                                         User = _mapper.Map<User, UserDTO>(user),
                                         SubmitCount = user.Submissions.Where(submission => submission.CreatedAt.Date == Date.Date).Count(),
                                         SuccessSubmitCount = user.Submissions.Where(submission => submission.State == SubmitState.Success && submission.CreatedAt.Date == Date.Date).Count(),
                                     })
                                     .OrderByDescending(x => x.SubmitCount)
                                     .Take(take);
            }
            else
            {
                return _context.Users.Include(user => user.Submissions)
                                     .Where(user => user.CreatedAt.Date < Date.Date)
                                     .Where(expression)
                                     .Select(user => new SubmissionStatistic
                                     {
                                         User = _mapper.Map<User, UserDTO>(user),
                                         SubmitCount = user.Submissions.Where(submission => submission.CreatedAt.Date == Date.Date).Count(),
                                         SuccessSubmitCount = user.Submissions.Where(submission => submission.State == SubmitState.Success && submission.CreatedAt.Date == Date.Date).Count(),
                                     })
                                     .OrderByDescending(x => x.SubmitCount)
                                     .Take(take);
            }
        }

        public User GetUserWithRole(Expression<Func<User, bool>> expression)
        {
            return _context.Users.Include(user => user.Role).FirstOrDefault(expression);
        }

        public User GetUserWithSubmit(Expression<Func<User, bool>> expression)
        {
            return _context.Users.Include(user => user.Submissions).FirstOrDefault(expression);
        }
    }
}
