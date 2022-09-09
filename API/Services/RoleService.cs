﻿using API.Helper;
using API.Repository;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IRoleService
    {
        RoleDTO FindById(string ID);
        RoleDTO FindByName(string Name);
    }
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
