using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Models.DTO;
using System;

namespace ICode.Mapper
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<User, UserDTO>()
                    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ? "Male" : "Female"));
            CreateMap<Tag, TagDTO>();
            CreateMap<Role, RoleDTO>();
            CreateMap<Report, ReportDTO>();
            CreateMap<Reply, ReplyDTO>();
            CreateMap<TestCase, TestcaseDTO>();


            CreateMap<PagingList<Report>, PagingList<ReportDTO>>();
            CreateMap<PagingList<User>, PagingList<UserDTO>>();
            CreateMap<PagingList<Tag>, PagingList<TagDTO>>();

            CreateMap<RegisterUser, User>()
                .ForMember(dest => dest.ID, otp => otp.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.CreatedAt, otp => otp.MapFrom(src => DateTime.Now));

        }
    }
}
