using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class ReplyRepository: BaseRepository<Reply>, IReplyRepository
    {
        public ReplyRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
