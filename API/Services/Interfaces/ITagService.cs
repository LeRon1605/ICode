using API.Models.DTO;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITagService: IService<Tag>
    {
        bool Exist(string name);
        IEnumerable<Tag> Find(string name);
        Task<PagingList<Tag>> GetPageAsync(int page, int pageSize, string keyword);
        IEnumerable<Problem> GetProblemOfTag(string Id);
    }
}
