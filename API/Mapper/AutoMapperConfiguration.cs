using API.Models.DTO;
using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
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
            CreateMap<User, UserDTO>()
                    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ? "Male" : "Female"));
            CreateMap<Tag, TagDTO>();
            CreateMap<Role, RoleDTO>();
            CreateMap<Report, ReportDTO>();
            CreateMap<Reply, ReplyDTO>();
            CreateMap<TestCase, TestcaseDTO>();
            CreateMap<Submission, SubmissionDTO>();
            CreateMap<SubmissionDetail, SubmissionDetailDTO>();
            CreateMap<Problem, ProblemDTO>()
                .ForMember(dest => dest.AuthorId, otp => otp.MapFrom(src => src.ArticleID));

            CreateMap<PagingList<Report>, PagingList<ReportDTO>>();
            CreateMap<PagingList<Problem>, PagingList<ProblemDTO>>();
            CreateMap<PagingList<User>, PagingList<UserDTO>>();
            CreateMap<PagingList<Tag>, PagingList<TagDTO>>();
            CreateMap<PagingList<Submission>, PagingList<SubmissionDTO>>();
            CreateMap<RegisterUser, User>()
                .ForMember(dest => dest.ID, otp => otp.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.CreatedAt, otp => otp.MapFrom(src => DateTime.Now));

        }
    }
}
