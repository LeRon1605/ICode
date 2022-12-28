using AutoMapper;
using CodeStudy.Models;
using Data.Entity;
using Models;
using Models.DTO;

namespace ICode.Mapper
{
    public class ProblemMapperConfig : Profile
    {
        public ProblemMapperConfig()
        {
            CreateMap<Problem, ProblemBase>();

            CreateMap<Problem, ProblemDTO>()
                .IncludeBase<Problem, ProblemBase>()
                .ForMember(dest => dest.Author, otp => otp.MapFrom(src => src.Article));

            CreateMap<ProblemContestDetail, ProblemContest>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Problem.ID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Problem.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Problem.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Problem.UpdatedAt))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score));

            CreateMap<PagingList<Problem>, PagingList<ProblemDTO>>();
        }
    }
}
