using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class TokenRepository: BaseRepository<RefreshToken>, ITokenRepository
    {
        public TokenRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
