using AutoMapper;
using Data.Entity;
using Models;

namespace ICode.API.Mapper.ContestMapper
{
    public class RunningContestMapper : IContestMapper
    {
        private readonly IMapper _mapper;
        public RunningContestMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
        public ContestBase Map(Contest contest)
        {
            return _mapper.Map<Contest, ContestDTO>(contest);
        }
    }
}
