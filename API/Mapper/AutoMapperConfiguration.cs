using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Models.DTO;
using System;
using System.Linq;

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
            CreateMap<Submission, SubmissionDTO>()
                .ForMember(dest => dest.Problem, opt => opt.MapFrom(src => src.SubmissionDetails.First().TestCase.Problem));
            CreateMap<Submission, SubmissionResult>();
            CreateMap<SubmissionDetail, SubmissionDetailDTO>();
            CreateMap<Problem, ProblemBase>();
            CreateMap<Problem, ProblemDTO>()
                .ForMember(dest => dest.Author, otp => otp.MapFrom(src => src.Article));

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
