using API.Models.DTO;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IReportService: IService<Report>
    {
        Report GetDetailById(string Id);
        Task<bool> Reply(Report report, ReplyInput input);
        Task<bool> UpdateReply(Report report, ReplyInput input);
        Task<bool> RemoveReply(Report report);
        IEnumerable<Report> GetReportByFilter(string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy);
        IEnumerable<Report> GetReportOfUserByFilter(string title, string userId, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy);
        Task<PagingList<Report>> GetReportsOfUser(int page, int pageSize, string userID, string problem);
        Task<PagingList<Report>> GetPageAsync(int page, int pageSize, string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy);
    }
}
