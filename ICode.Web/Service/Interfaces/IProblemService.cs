using CodeStudy.Models;
using Models.Statistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface IProblemService
    {
        Task<ProblemDTO> GetById(string id);
        Task<List<ProblemDTO>> GetAll();
        Task<List<ProblemStatistic>> GetHotProblems();
    }
}