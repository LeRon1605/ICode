using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface ITokenRepository: IRepository<RefreshToken>
    {

    }
    public class TokenRepository: BaseRepository<RefreshToken>, ITokenRepository
    {
        public TokenRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
