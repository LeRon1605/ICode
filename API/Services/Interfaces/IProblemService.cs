using API.Models.DTO;
using API.Models.Entity;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProblemService
    {
        Problem FindByID(string ID);
        IEnumerable<Problem> FindAll();
        ICollection<Tag> GetTagsOfProblem(string ID);
        Task<PagingList<Problem>> GetPage(int page, int pageSize, string tag, string keyword);
        Task Add(ProblemInput input, string authorID);
        Task<bool> Remove(string ID);
        Task<bool> Update(string ID, ProblemInputUpdate input);
    }
}
