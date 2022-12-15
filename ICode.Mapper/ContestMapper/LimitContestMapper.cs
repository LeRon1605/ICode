using AutoMapper;
using Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System;

namespace ICode.API.Mapper.ContestMapper
{
    public class LimitContestMapper : IContestMapper
    {
        private readonly IMapper _mapper;
        public LimitContestMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
        public ContestBase Map(Contest contest)
        {
            if (DateTime.Now >= contest.StartAt)
            {
                return _mapper.Map<Contest, ContestDTO>(contest);
            }
            else
            {
                return _mapper.Map<Contest, ContestBase>(contest);
            }
        }
    }
}
