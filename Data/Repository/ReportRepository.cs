using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class ReportRepository: BaseRepository<Report>, IReportRepository
    {
        public ReportRepository(ICodeDbContext context): base(context)
        {

        }

        public IEnumerable<Report> GetReportsDetail(Expression<Func<Report, bool>> expression)
        {
            if (expression == null)
                return _context.Reports.Include(x => x.Problem)
                                       .Include(x => x.User)
                                       .Include(x => x.Reply);
            else
                return _context.Reports.Include(x => x.Problem)
                                       .Include(x => x.User)
                                       .Include(x => x.Reply)
                                       .Where(expression);
        }

        public Report GetReportsDetailSingle(Expression<Func<Report, bool>> expression)
        {
            return _context.Reports.Include(x => x.Problem)
                                   .Include(x => x.User)
                                   .Include(x => x.Reply)
                                   .FirstOrDefault(expression);
        }

        public Report GetReportWithProblem(Expression<Func<Report, bool>> expression = null)
        {
            return _context.Reports.Include(x => x.Problem)
                                   .Include(x => x.User)
                                   .Include(x => x.Problem)
                                   .Include(x => x.Reply)
                                   .FirstOrDefault(expression);
        }
    }
}
