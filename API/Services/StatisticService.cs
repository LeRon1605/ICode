using API.Models.Entity;
using API.Repository;
using AutoMapper;
using CodeStudy.Models;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Services
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

        public IEnumerable<Statistic> GetNewUser(DateTime startDate, DateTime endDate)
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
                IEnumerable<SubmissionStatistic> submission = _userRepository.GetTopUserProductiveInDay(i).ToList();
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
            return _userRepository.GetTopUserProductive();
        }

        public IEnumerable<Statistic> GetProblemSubmitInRange(DateTime startDate, DateTime endDate, bool? state)
        {
            List<Statistic> statisticList = new List<Statistic>();
            for (DateTime i = startDate.Date; i <= endDate.Date; i = i.AddDays(1))
            {
                IEnumerable<ProblemStatistic> problemStatistic = _problemRepository.GetHotProblemInDay(i).ToList();
                statisticList.Add(new Statistic
                {
                    Total = problemStatistic.Count(),
                    Data = problemStatistic,
                    Date = i
                });
            }
            return statisticList;
        }

        public IEnumerable<ProblemStatistic> GetProblemSubmit()
        {
            return _problemRepository.GetHotProblem();
        }
    }
}
