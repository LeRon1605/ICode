using API.Helper;
using API.Repository;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IRoleService
    {
        Role FindById(string ID);
        Role FindByName(string Name);
    }
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public Role FindById(string ID)
        {
            return _roleRepository.FindByID(ID);
        }

        public Role FindByName(string Name)
        {
            return _roleRepository.FindSingle(x => x.Name == Name);
        }
    }
}
