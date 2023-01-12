using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using ICode.Common;
using Models;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Linq;

namespace ICode.Mapper
{
    public class ProblemMapperConfig : Profile
    {
        public ProblemMapperConfig()
        {
            // Request

            // Response
            CreateMap<Problem, ProblemBase>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => (src.Level == Level.Easy ? "Easy" : (src.Level == Level.Medium ? "Medium" : "Hard"))));

            CreateMap<Problem, ProblemDTO>()
                .IncludeBase<Problem, ProblemBase>()
                .ForMember(dest => dest.Author, otp => otp.MapFrom(src => src.Article))
                .ForMember(dest => dest.SuccessSubmit, otp => otp.MapFrom(src => src.Submissions.Where(x => x.State == SubmitState.Success).Count()))
                .ForMember(dest => dest.TotalSubmit, otp => otp.MapFrom(src => src.Submissions.Count()));

            CreateMap<ProblemContestDetail, ProblemContest>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Problem.ID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Problem.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Problem.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Problem.UpdatedAt))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));
        }
    }
}
