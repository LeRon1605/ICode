using Data.Entity;
using Models;

namespace ICode.API.Mapper.ContestMapper
{
    public interface IContestMapper
    {
        public ContestBase Map(Contest contest);
    }
}
