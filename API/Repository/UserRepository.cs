using API.Models.Data;
using API.Models.DTO;
using API.Models.Entity;
using AutoMapper;
using CodeStudy.Models;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IUserRepository: IRepository<User>
    {
        User GetUserWithRole(Expression<Func<User, bool>> expression);
        User GetUserWithSubmit(Expression<Func<User, bool>> expression);
        Task<IEnumerable<Problem>> GetProblemSolvedByUser(string UserID, Func<Problem, bool> expression = null);
        IEnumerable<ProblemSolvedStatistic> GetProblemSolveStatisticOfUser();
        IEnumerable<User> GetNewUser(DateTime Date, Expression<Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserActivityInDay(DateTime Date, int take = 5, Expression <Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserActivity(int take = 5, Expression<Func<User, bool>> expression = null);
    }
    public class UserRepository: BaseRepository<User>, IUserRepository
    {
        private readonly IMapper _mapper;
        public UserRepository(ICodeDbContext context, IMapper mapper): base(context)
        {
            _mapper = mapper;
        }

        public IEnumerable<User> GetNewUser(DateTime Date, Expression<Func<User, bool>> expression)
        {
            if (expression == null)
                return _context.Users.Where(user => user.CreatedAt.Date == Date.Date);
            else
                return _context.Users.Where(user => user.CreatedAt.Date == Date.Date).Where(expression);
        }

        public Task<IEnumerable<Problem>> GetProblemSolvedByUser(string UserID, Func<Problem, bool> expression)
        {
            User user = _context.Users.Include(x => x.Submissions).ThenInclude(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).ThenInclude(x => x.Problem).ThenInclude(x => x.Tags).FirstOrDefault(x => x.ID == UserID);
            if (user == null)
            {
                return null;
            }
            if (expression == null)
                return Task.FromResult(user.Submissions.Where(x => x.Status).Select(x => x.SubmissionDetails.First().TestCase.Problem).GroupBy(x => x.ID).Select(x => x.FirstOrDefault()));
            else
                return Task.FromResult(user.Submissions.Where(x => x.Status).Select(x => x.SubmissionDetails.First().TestCase.Problem).GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).Where(expression));
        }

        public IEnumerable<ProblemSolvedStatistic> GetProblemSolveStatisticOfUser()
        {
            return  (from user in _context.Users
                    join submission in _context.Submissions
                    on user.ID equals submission.UserID
                    join submissionDetail in _context.SubmissionDetails
                    on submission.ID equals submissionDetail.SubmitID
                    join testCase in _context.TestCases
                    on submissionDetail.TestCaseID equals testCase.ID
                    join testcase in _context.TestCases
                    on submissionDetail.TestCaseID equals testcase.ID
                    join problem in _context.Problems
                    on testCase.ProblemID equals problem.ID
                    where submission.Status == true
                    select new
                    {
                        UserID = user.ID,
                        User = _mapper.Map<User, UserDTO>(user),
                        Problem = _mapper.Map<Problem, ProblemDTO>(problem),
                        Submit = _mapper.Map<Submission, SubmissionDTO>(submission)
                    })
                    .AsEnumerable()
                    .GroupBy(x => x.UserID)
                    .Select(group => new ProblemSolvedStatistic
                    {
                        User = group.Select(x => x.User).FirstOrDefault(),
                        Details = group.GroupBy(x => x.Problem.ID).Select(x => new ProblemSolvedStatisticDetail
                        {
                            Submit = x.Select(x => x.Submit).OrderBy(x => x.CreatedAt).FirstOrDefault(),
                            Problem = x.Select(x => x.Problem).GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).FirstOrDefault()
                        }).ToList()
                    });
        }

        public IEnumerable<SubmissionStatistic> GetTopUserActivity(int take = 5, Expression<Func<User, bool>> expression = null)
        {
            return _context.Users.Include(user => user.Submissions)
                                 .Select(user => new SubmissionStatistic
                                 {
                                     User = _mapper.Map<User, UserDTO>(user),
                                     SubmitCount = user.Submissions.Count(),
                                     SuccessSubmitCount = user.Submissions.Where(submission => submission.Status).Count(),
                                 })
                                 .OrderByDescending(x => x.SuccessSubmitCount)
                                 .Take(take);
        }

        public IEnumerable<SubmissionStatistic> GetTopUserActivityInDay(DateTime Date, int take, Expression<Func<User, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Users.Include(user => user.Submissions)
                                     .Select(user => new SubmissionStatistic
                                     {
                                         User = _mapper.Map<User, UserDTO>(user),
                                         SubmitCount = user.Submissions.Where(submission => submission.CreatedAt.Date == Date.Date).Count(),
                                         SuccessSubmitCount = user.Submissions.Where(submission => submission.Status && submission.CreatedAt.Date == Date.Date).Count(),
                                     })
                                     .OrderByDescending(x => x.SubmitCount)
                                     .Take(take);
            }
            else
            {
                return _context.Users.Include(user => user.Submissions)
                                     .Where(expression)
                                     .Select(user => new SubmissionStatistic
                                     {
                                         User = _mapper.Map<User, UserDTO>(user),
                                         SubmitCount = user.Submissions.Where(submission => submission.CreatedAt.Date == Date.Date).Count(),
                                         SuccessSubmitCount = user.Submissions.Where(submission => submission.Status && submission.CreatedAt.Date == Date.Date).Count(),
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
