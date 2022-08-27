using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Data.Entity;

namespace API.Repository
{
    public interface IReportRepository : IRepository<Report>
    {
        Report GetReportsDetailSingle(Expression<Func<Report, bool>> expression);
        Report GetReportWithProblem(Expression<Func<Report, bool>> expression);
        IEnumerable<Report> GetReportsDetail(Expression<Func<Report, bool>> expression = null);
    }
}
