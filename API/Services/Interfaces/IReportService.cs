using API.Models.Entity;
using CodeStudy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IReportService
    {
        Report FindByID(string ID);
        Task Add(ReportInput input, string authorID, string problemID);
        Task Update(Report report, ReportInput input);
        Task Remove(Report report);
        Task<bool> Reply(Report report, ReplyInput input);
        Task<bool> UpdateReply(Report report, ReplyInput input);
        Task<bool> RemoveReply(Report report);
        IEnumerable<Report> GetAll();
        IEnumerable<Report> GetReportsOfUser(string userID);
    }
}
