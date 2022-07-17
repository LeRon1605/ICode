using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IReplyRepository: IRepository<Reply>
    {

    }
    public class ReplyRepository: BaseRepository<Reply>, IReplyRepository
    {
        public ReplyRepository(ICodeDbContext context): base(context)
        {

        }
    }
}
