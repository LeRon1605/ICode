using CodeStudy.Models;
using ICode.Common;
using Models.DTO;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface IProblemService
    {
        Task<bool> Remove(string id);
        Task<bool> Add(ProblemInput data);
        Task<ProblemDTO> GetById(string id);
        Task<List<ProblemDTO>> GetAll(string keyword = "", string tag = "", DateTime? date = null, string sort = "", string orderBy = "");
        Task<PagingList<ProblemDTO>> GetPage(int page, int pageSize = 5, string keyword = "", string tag = "", DateTime? date = null, string level = "", string sort = "", string orderBy = "");
        Task<List<ProblemStatistic>> GetHotProblems(int take);
        Task<List<ProblemDTO>> GetNewProblems(int take);
    }
}