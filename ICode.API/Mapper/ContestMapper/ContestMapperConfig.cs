using AutoMapper;
using Data.Entity;
using Models;
using System;

namespace ICode.Mapper.ContestMapper
{
    public class ContestMapperConfig : Profile
    {
        public ContestMapperConfig()
        {
            CreateMap<Contest, ContestBase>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StartAt <= DateTime.Now && src.EndAt >= DateTime.Now));

            CreateMap<Contest, ContestDTO>()
                .IncludeBase<Contest, ContestBase>()
                .ForMember(dest => dest.Problems, opt => opt.MapFrom(src => src.ProblemContestDetails));

        }
    }
}
