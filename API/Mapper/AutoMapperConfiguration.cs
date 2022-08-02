using API.Models.Entity;
using AutoMapper;
using CodeStudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Mapper
{
    public class AutoMapperConfiguration: Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<UserDTO, User>()
                .ReverseMap();

            CreateMap<RegisterUser, User>()
                .ForMember(dest => dest.ID, otp => otp.MapFrom(src => Guid.NewGuid().ToString()))
                .ReverseMap();

        }
    }
}
