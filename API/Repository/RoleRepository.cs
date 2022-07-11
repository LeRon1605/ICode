using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IRoleRepository: IRepository<Role>
    {
        Role findByName(string Name);
    }
    public class RoleRepository: BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(ICodeDbContext context): base(context)
        {

        }

        public Role findByName(string Name)
        {
            return _context.Roles.FirstOrDefault(role => role.Name == Name);
        }
    }
}
