using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IReportRepository: IRepository<Report>
    {

    }
    public class ReportRepository: BaseRepository<Report>, IReportRepository
    {
        public ReportRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
