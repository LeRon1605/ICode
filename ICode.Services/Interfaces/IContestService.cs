using Data.Entity;
using ICode.API.Mapper.ContestMapper;
using ICode.Common;
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
        Task NotifyUser(string id);
        ContestBase GetDetailById(string id, IContestMapper contestMapper);
        PagingList<ContestBase> GetPageContestByFilter(int page, int pageSize, string name, DateTime? date, bool? state, string sort, string orderBy, IContestMapper contestMapper);
        List<ContestBase> GetContestByFilter(string name, DateTime? date, bool? state, string sort, string orderBy, IContestMapper contestMapper);
    }
}
