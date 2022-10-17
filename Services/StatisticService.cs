using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Data.Repository.Interfaces;
using Models;
using Models.Statistic;
using Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IProblemRepository _problemRepository;
        private readonly IMapper _mapper;
        public StatisticService(IUserRepository userRepository, ISubmissionRepository submissionRepository, IProblemRepository problemRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _submissionRepository = submissionRepository;
            _problemRepository = problemRepository;
            _mapper = mapper;
        }

        public IEnumerable<Statistic> GetNewUserInRange(DateTime startDate, DateTime endDate, string name, bool? gender)
        {
            List<Statistic> statisticList = new List<Statistic>();
            for (DateTime i = startDate.Date;i <= endDate.Date;i = i.AddDays(1))
            {
                IEnumerable<User> newUser = _userRepository.GetNewUser(i);
                statisticList.Add(new Statistic
                {
                    Total = newUser.Count(),
                    Data = _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(newUser),
                    Date = i
                });
            }
            return statisticList;
        }

        public IEnumerable<Statistic> GetUserSubmitInRage(DateTime startDate, DateTime endDate)
        {
            List<Statistic> statisticList = new List<Statistic>();
            for (DateTime i = startDate.Date; i <= endDate.Date; i = i.AddDays(1))
            {
                IEnumerable<SubmissionStatistic> submission = _userRepository.GetTopUserActivityInDay(i).ToList();
                statisticList.Add(new Statistic
                {
                    Total = submission.Count(),
                    Data = submission,
                    Date = i
                });
            }
            return statisticList;
        }

        public IEnumerable<SubmissionStatistic> GetUserSubmit()
        {
            return _userRepository.GetTopUserActivity().ToList();
        }

        public IEnumerable<Statistic> GetSubmitOfProblemInRange(DateTime startDate, DateTime endDate, string name, string author, string tag)
        {
            List<Statistic> statisticList = new List<Statistic>();
            for (DateTime i = startDate.Date; i <= endDate.Date; i = i.AddDays(1))
            {
                IEnumerable<ProblemStatistic> problemStatistic = _problemRepository.GetHotProblemInDay(i, problem => problem.Name.Contains(name) && problem.Article.Username.Contains(author) && (string.IsNullOrEmpty(tag) || problem.Tags.Any(x => x.Name.Contains(tag)))).ToList();
                statisticList.Add(new Statistic
                {
                    Total = problemStatistic.Count(),
                    Data = problemStatistic,
                    Date = i
                });
            }
            return statisticList;
        }

        public IEnumerable<ProblemStatistic> GetSubmitOfProblem(string name, string author, string tag)
        {
            return _problemRepository.GetHotProblem(x => x.Name.Contains(name) && x.Article.Username.Contains(author) && (string.IsNullOrEmpty(tag) || x.Tags.Any(x => x.Name.Contains(tag)))).ToList();
        }

        public IEnumerable<UserRank> GetUserRank()
        {
            List<UserRank> result = _userRepository.GetProblemSolveStatisticOfUser().Select(x => new UserRank
            {
                ProblemSovled =  x.Details.Count(),
                Detail = x.Details,
                User = x.User
            }).ToList();
            result = result.OrderByDescending(x => x.ProblemSovled).ToList();
            for (int i = 0;i < result.Count();i++)
            {
                result[i].Rank = i + 1;
            }
            return result;
        }

        public IEnumerable<Statistic> GetUserRankInRange(DateTime startDate, DateTime endDate)
        {
            List<ProblemSolvedStatistic> data = _userRepository.GetProblemSolveStatisticOfUser().Select(x => new ProblemSolvedStatistic
            {
                User = x.User,
                Details = x.Details.Where(x => x.Submit.CreatedAt.Date >= startDate.Date && x.Submit.CreatedAt.Date <= endDate.Date),
            }).ToList();
            List<Statistic> statisticList = new List<Statistic>();
            for (DateTime i = startDate.Date; i <= endDate.Date; i = i.AddDays(1))
            {
                List<UserRank> userRank = data.Select(x => new UserRank
                {
                    User = x.User,
                    ProblemSovled = x.Details.Where(x => x.Submit.CreatedAt.Date == i).Count(),
                    Detail = x.Details.Where(x => x.Submit.CreatedAt.Date == i).ToList()
                }).ToList();
                userRank = userRank.OrderByDescending(x => x.ProblemSovled).ToList();
                for (int j = 0; j < userRank.Count(); j++)
                {
                    userRank[j].Rank = j + 1;
                }
                statisticList.Add(new Statistic
                {
                    Total = userRank.Count(),
                    Data = userRank,
                    Date = i
                });
            }
            return statisticList;
        }

        public IEnumerable<Statistic> GetNewProblemInRange(DateTime startDate, DateTime endDate, string name, string author, string tag)
        {
            List<Statistic> statistics = new List<Statistic>();
            for (DateTime i = startDate.Date;i <= endDate.Date; i = i.AddDays(1))
            {
                IEnumerable<Problem> problems = _problemRepository.GetNewProblem(i, x => x.Name.Contains(name) && x.Article.Username.Contains(author) && (string.IsNullOrEmpty(tag) || x.Tags.Any(x => x.Name.Contains(tag))));
                statistics.Add(new Statistic
                {
                    Total = problems.Count(),
                    Data = _mapper.Map<IEnumerable<Problem>, IEnumerable<ProblemDTO>>(problems),
                    Date = i
                });
            }
            return statistics;
        }
    }
}
