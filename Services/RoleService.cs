using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Data.Repository.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        public RoleDTO FindById(string ID)
        {
            return _mapper.Map<Role, RoleDTO>(_roleRepository.FindByID(ID));
        }

        public RoleDTO FindByName(string Name)
        {
            return _mapper.Map<Role, RoleDTO>(_roleRepository.FindSingle(x => x.Name == Name));
        }
    }
}
