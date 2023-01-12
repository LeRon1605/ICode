using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using ICode.Common;
using ICode.Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICode.Mapper
{
    public class UserMapperConfig: Profile
    {
        public UserMapperConfig()
        {
            CreateMap<User, UserBase>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ? "Male" : "Female"));
            
            CreateMap<User, UserDTO>()
                .IncludeBase<User, UserBase>();

            CreateMap<User, UserDetail>()
                .IncludeBase<User, UserBase>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Submissions.Where(x => x.State == SubmitState.Success).Select(x => x.Problem).GroupBy(x => x.ID).Select(x => x.FirstOrDefault()).Sum(x => x.Score)));

            CreateMap<ContestDetail, UserContest>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.RegisteredAt, opt => opt.MapFrom(src => src.RegisteredAt));

            CreateMap<PagingList<User>, PagingList<UserDTO>>();
        }
    }
}
