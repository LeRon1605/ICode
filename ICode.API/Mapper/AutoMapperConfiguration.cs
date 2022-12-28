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
            CreateMap<Tag, TagDTO>();
            CreateMap<Role, RoleDTO>();
            CreateMap<Report, ReportDTO>();
            CreateMap<Reply, ReplyDTO>();
            CreateMap<TestCase, TestcaseDTO>();


            CreateMap<PagingList<Report>, PagingList<ReportDTO>>();

            CreateMap<PagingList<Tag>, PagingList<TagDTO>>();
        }
    }
}
