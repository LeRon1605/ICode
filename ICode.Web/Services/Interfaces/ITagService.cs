using CodeStudy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagDTO>> GetAll();
    }
}
