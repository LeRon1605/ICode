using API.Models.Data;
using API.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IReportRepository: IRepository<Report>
    {
        IEnumerable<Report> GetReportsDetail(Expression<Func<Report, bool>> expression = null);
        Report GetReportsDetailSingle(Expression<Func<Report, bool>> expression);
        Report GetReportWithProblem(Expression<Func<Report, bool>> expression);
    }
    public class ReportRepository: BaseRepository<Report>, IReportRepository
    {
        public ReportRepository(ICodeDbContext context): base(context)
        {

        }

        public IEnumerable<Report> GetReportsDetail(Expression<Func<Report, bool>> expression)
        {
            if (expression == null)
                return _context.Reports.Include(x => x.Reply);
            else
                return _context.Reports.Include(x => x.Reply).Where(expression);
        }

        public Report GetReportsDetailSingle(Expression<Func<Report, bool>> expression)
        {
            return _context.Reports.Include(x => x.Reply).FirstOrDefault(expression);
        }

        public Report GetReportWithProblem(Expression<Func<Report, bool>> expression = null)
        {
            return _context.Reports.Include(x => x.Problem).Include(x => x.Reply).FirstOrDefault(expression);
        }
    }
}
