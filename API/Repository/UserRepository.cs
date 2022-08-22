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
        IEnumerable<User> GetNewUser(DateTime Date, Expression<Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserProductiveInDay(DateTime Date, int take = 5, Expression <Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserProductive(int take = 5, Expression<Func<User, bool>> expression = null);
        IEnumerable<UserRank> GetUserProblemSolve(int take = 5, Expression<Func<User, bool>> expression = null);
        IEnumerable<UserRank> GetUserProblemSolveInDay(DateTime date, int take = 5, Expression<Func<User, bool>> expression = null);
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

        public IEnumerable<SubmissionStatistic> GetTopUserProductive(int take = 5, Expression<Func<User, bool>> expression = null)
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

        public IEnumerable<SubmissionStatistic> GetTopUserProductiveInDay(DateTime Date, int take, Expression<Func<User, bool>> expression)
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

        public IEnumerable<UserRank> GetUserProblemSolve(int take, Expression<Func<User, bool>> expression = null)
        {
            IEnumerable<User> users;
            if (expression == null)
            {
                 users = _context.Users.Include(user => user.Submissions).ThenInclude(x => x.SubmissionDetails).ThenInclude(x => x.TestCase);
            }
            else
            {
                users = _context.Users.Include(user => user.Submissions).ThenInclude(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).Where(expression);
            }
            List<UserRank> result = new List<UserRank>();
            foreach (User user in users)
            {
                List<string> problemSolve = new List<string>();
                foreach (Submission submission in user.Submissions.Where(x => x.Status))
                {
                    problemSolve.Add(submission.SubmissionDetails.First().TestCase.ProblemID);
                }
                result.Add(new UserRank
                {
                    User = _mapper.Map<User, UserDTO>(user),
                    TotalSubmit = user.Submissions.Count(),
                    ProblemSovled = problemSolve.Distinct().Count(),
                    Problems = problemSolve.Distinct()
                });
            }
            result = result.OrderByDescending(x => x.ProblemSovled).Take(take).ToList();
            for (int i = 0;i < result.Count(); i++)
            {
                result[i].Rank = i + 1;
            }
            return result;
        }

        public IEnumerable<UserRank> GetUserProblemSolveInDay(DateTime date, int take = 5, Expression<Func<User, bool>> expression = null)
        {
            IEnumerable<User> users;
            if (expression == null)
            {
                users = _context.Users.Include(user => user.Submissions).ThenInclude(x => x.SubmissionDetails).ThenInclude(x => x.TestCase);
            }
            else
            {
                users = _context.Users.Include(user => user.Submissions).ThenInclude(x => x.SubmissionDetails).ThenInclude(x => x.TestCase).Where(expression);
            }
            List<UserRank> result = new List<UserRank>();
            foreach (User user in users)
            {
                List<string> problemSolved = new List<string>();
                List<string> problemSolveInDay = new List<string>();
                foreach (Submission submission in user.Submissions.Where(x => x.Status))
                {
                    if (!problemSolved.Contains(submission.SubmissionDetails.First().TestCase.ProblemID))
                    {
                        problemSolved.Add(submission.SubmissionDetails.First().TestCase.ProblemID);
                        if (!problemSolveInDay.Contains(submission.SubmissionDetails.First().TestCase.ProblemID) && submission.CreatedAt.Date == date.Date)
                            problemSolveInDay.Add(submission.SubmissionDetails.First().TestCase.ProblemID);
                    }    
                }
                result.Add(new UserRank
                {
                    User = _mapper.Map<User, UserDTO>(user),
                    TotalSubmit = user.Submissions.Where(x => x.CreatedAt.Date == date.Date).Count(),
                    ProblemSovled = problemSolveInDay.Count(),
                    Problems = problemSolved
                });
            }
            result = result.OrderByDescending(x => x.ProblemSovled).Take(take).ToList();
            for (int i = 0; i < result.Count(); i++)
            {
                result[i].Rank = i + 1;
            }
            return result;
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
