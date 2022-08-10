using API.Models.DTO;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ITagService
    {
        bool Exist(string name);
        Task Add(string name);
        Tag FindById(string Id);
        Task Remove(Tag tag);
        Task<bool> Update(Tag tag, string name);
        IEnumerable<Tag> GetAll();
        Task<PagingList<Tag>> GetPageAsync(int page, int pageSize, string keyword);
        IEnumerable<Problem> GetProblemOfTag(string Id);
    }
}
