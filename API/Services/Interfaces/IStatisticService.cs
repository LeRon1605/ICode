using Models;
using Models.Statistic;
using System;
using System.Collections.Generic;

namespace API.Services
{
    public interface IStatisticService
    {
        public IEnumerable<Statistic> GetNewProblemInRange(DateTime startDate, DateTime endDate);
        public IEnumerable<Statistic> GetNewUserInRange(DateTime startDate, DateTime endDate);
        public IEnumerable<Statistic> GetUserSubmitInRage(DateTime startDate, DateTime endDate);

        public IEnumerable<Statistic> GetSubmitOfProblemInRange(DateTime startDate, DateTime endDate, bool? state = null);
        public IEnumerable<UserRank> GetUserRank();
        public IEnumerable<Statistic> GetUserRankInRange(DateTime startDate, DateTime endDate);
        public IEnumerable<SubmissionStatistic> GetUserSubmit();
        public IEnumerable<ProblemStatistic> GetSubmitOfProblem();

    }
}
