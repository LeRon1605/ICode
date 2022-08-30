using API.Models.DTO;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITagService: IService<Tag>
    {
        bool Exist(string name);
        IEnumerable<TagDTO> GetTagsByFilter(string name, DateTime? date, string sort, string orderBy);
        Task<PagingList<TagDTO>> GetPageByFilter(int page, int pageSize, string name, DateTime? date, string sort, string orderBy);
        IEnumerable<ProblemDTO> GetProblemOfTag(string Id, string name, DateTime? date, string sort, string orderBy);
    }
}
