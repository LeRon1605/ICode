using API.Models.Entity;
using CodeStudy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IReportService: IService<Report>
    {
        Task<bool> Reply(Report report, ReplyInput input);
        Task<bool> UpdateReply(Report report, ReplyInput input);
        Task<bool> RemoveReply(Report report);
        IEnumerable<Report> GetReportsOfUser(string userID);
    }
}
