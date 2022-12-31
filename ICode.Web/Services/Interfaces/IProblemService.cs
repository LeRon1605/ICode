using CodeStudy.Models;
using Models.DTO;
using Models.Statistic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface IProblemService
    {
        Task<ProblemDTO> GetById(string id);
        Task<List<ProblemDTO>> GetAll(string keyword = "", string tag = "", DateTime? date = null, string sort = "", string orderBy = "");
        Task<PagingList<ProblemDTO>> GetPage(int page, int pageSize = 5, string keyword = "", string tag = "", DateTime? date = null, string sort = "", string orderBy = "");
        Task<List<ProblemStatistic>> GetHotProblems();
        Task<List<ProblemDTO>> GetNewProblems();
    }
}