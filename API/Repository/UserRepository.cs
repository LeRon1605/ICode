using API.Models.Data;
using API.Models.DTO;
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
    public interface IUserRepository: IRepository<User>
    {
        User GetUserWithRole(Expression<Func<User, bool>> expression);
        User GetUserWithSubmit(Expression<Func<User, bool>> expression);
        IEnumerable<User> GetNewUser(DateTime Date, Expression<Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserProductiveInDay(DateTime Date, int take = 5, bool? state = null, Expression <Func<User, bool>> expression = null);
        IEnumerable<SubmissionStatistic> GetTopUserProductive(int take = 5, bool? state = null, Expression<Func<User, bool>> expression = null);
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

        public IEnumerable<SubmissionStatistic> GetTopUserProductive(int take = 5, bool? state = null, Expression<Func<User, bool>> expression = null)
        {
            return _context.Users.Include(user => user.Submissions)
                     .Select(user => new SubmissionStatistic
                     {
                         User = _mapper.Map<User, UserDTO>(user),
                         SubmitCount = user.Submissions.Count(),
                         SuccessSubmitCount = user.Submissions.Where(submission => (state == null || state == (bool)state)).Count(),
                     })
                     .OrderByDescending(x => x.SuccessSubmitCount)
                     .Take(take);
        }

        public IEnumerable<SubmissionStatistic> GetTopUserProductiveInDay(DateTime Date, int take, bool? state, Expression<Func<User, bool>> expression)
        {
            if (expression == null)
            {
                return _context.Users.Include(user => user.Submissions)
                                     .Select(user => new SubmissionStatistic
                                     {
                                         User = _mapper.Map<User, UserDTO>(user),
                                         SubmitCount = user.Submissions.Where(submission => submission.CreatedAt.Date == Date.Date).Count(),
                                         SuccessSubmitCount = user.Submissions.Where(submission => (state == null || state == (bool)state) && submission.CreatedAt.Date == Date.Date).Count(),
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
                                         SuccessSubmitCount = user.Submissions.Where(submission => (state == null || state == (bool)state) && submission.CreatedAt.Date == Date.Date).Count(),
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
