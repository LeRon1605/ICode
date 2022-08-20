using API.Models.Entity;
using Models.Statistic;
using System;
using System.Collections.Generic;

namespace API.Services
{
    public interface IStatisticService
    {
        public IEnumerable<Statistic> GetNewUser(DateTime startDate, DateTime endDate);
        public IEnumerable<Statistic> GetUserSubmitInRage(DateTime startDate, DateTime endDate);

        public IEnumerable<Statistic> GetProblemSubmitInRange(DateTime startDate, DateTime endDate, bool? state = null);
        public IEnumerable<SubmissionStatistic> GetUserSubmit();
        public IEnumerable<ProblemStatistic> GetProblemSubmit();

    }
}
