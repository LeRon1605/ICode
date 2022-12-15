using Data.Entity;
using ICode.API.Mapper.ContestMapper;
using ICode.Common;
using ICode.Models;
using Models;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICode.Services.Interfaces
{
    public interface IContestService: IService<Contest>
    {
        Task<ServiceResult> Register(string id, string userId);
        Task<ServiceResult> RemoveUser(string id, string userId);
        Task NotifyUser(string id);
        ContestBase GetDetailById(string id, IContestMapper contestMapper);
        List<UserContest> GetPlayerOfContest(string id, string name, bool? gender, DateTime? registeredAt, string sort, string orderBy);
        PagingList<ContestBase> GetPageContestByFilter(int page, int pageSize, string name, DateTime? date, bool? state, string sort, string orderBy, IContestMapper contestMapper);
        PagingList<UserContest> GetPagePlayerOfContestByFilter(string id, int page, int pageSize, string name, bool? gender, DateTime? registeredAt, string sort, string orderBy);
        List<ContestBase> GetContestByFilter(string name, DateTime? date, bool? state, string sort, string orderBy, IContestMapper contestMapper);
        ServiceResult GetSubmissions(string id);
        Task<ServiceResult> AddPointForUser(string id, string userId, string problemId);
        bool IsUserInContest(string id, string userId);
        bool IsProblemInContest(string id, string problemId);
        bool IsUserSolvedProblem(string id, string userId, string problemId);
    }
}
