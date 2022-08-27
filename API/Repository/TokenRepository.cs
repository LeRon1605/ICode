using Data;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public class TokenRepository: BaseRepository<RefreshToken>, ITokenRepository
    {
        public TokenRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
