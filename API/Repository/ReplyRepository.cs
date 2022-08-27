using Data;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public class ReplyRepository: BaseRepository<Reply>, IReplyRepository
    {
        public ReplyRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
