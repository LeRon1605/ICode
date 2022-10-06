using CodeStudy.Models;
using Data.Entity;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProblemService: IService<Problem>
    {
        Task<bool> AddTag(string Id, string[] tagId);
        Task<bool> DeleteTag(string Id, string tagId);
        ProblemDTO GetProblemDetail(string ID);
        IEnumerable<ProblemDTO> GetProblemsByFilter(string name, string author, string tag, DateTime? date, string sort, string orderBy);
        Task<PagingList<ProblemDTO>> GetPageByFilter(int page, int pageSize, string name, string author, string tag, DateTime? date, string sort, string orderBy);
    }
}
