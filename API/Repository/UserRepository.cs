using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IUserRepository: IRepository<User>
    {

    }
    public class UserRepository: BaseRepository<User>, IUserRepository
    {
        public UserRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
