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
        ReportDTO GetDetailById(string Id);
        Task<bool> Reply(string reportId, ReplyInput input);
        Task<bool> UpdateReply(string reportId, ReplyInput input);
        Task<bool> RemoveReply(string reportId);
        IEnumerable<ReportDTO> GetReportByFilter(string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy);
        IEnumerable<ReportDTO> GetReportOfUserByFilter(string title, string userId, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy);
        Task<PagingList<ReportDTO>> GetPageReportOfUser(int page, int pageSize, string title, string userId, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy);
        Task<PagingList<ReportDTO>> GetPageAsync(int page, int pageSize, string title, string user, string problem, DateTime? createdAt, bool? reply, string sort, string orderBy);
    }
}
