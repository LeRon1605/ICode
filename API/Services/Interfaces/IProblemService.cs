using API.Models.DTO;
using API.Models.Entity;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IProblemService: IService<Problem>
    {
        ICollection<Tag> GetTagsOfProblem(string ID);
        Task<PagingList<Problem>> GetPage(int page, int pageSize, string tag, string keyword);
    }
}
