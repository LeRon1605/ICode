using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICode.Web.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserRank>> GetUserRank();
    }
}
