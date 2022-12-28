using Models;
using Models.Statistic;
using System;
using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface IStatisticService
    {
        public IEnumerable<Statistic> GetNewProblemInRange(DateTime startDate, DateTime endDate, string name, string author, string tag);
        public IEnumerable<Statistic> GetNewUserInRange(DateTime startDate, DateTime endDate, string name, bool? gender);
        public IEnumerable<Statistic> GetUserSubmitInRage(DateTime startDate, DateTime endDate, string name, bool? gender);

        public IEnumerable<Statistic> GetSubmitOfProblemInRange(DateTime startDate, DateTime endDate, string name, string author, string tag);
        public IEnumerable<UserRank> GetUserRank(string name, bool? gender);
        public IEnumerable<Statistic> GetUserRankInRange(DateTime startDate, DateTime endDate, string name, bool? gender);
        public IEnumerable<SubmissionStatistic> GetUserSubmit(bool? gender, string name);
        public IEnumerable<ProblemStatistic> GetSubmitOfProblem(string name, string author, string tag);

    }
}
