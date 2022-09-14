using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
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
