using API.Models.Data;
using API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface ITagRepository: IRepository<Tag>
    {

    }    
    public class TagRepository: BaseRepository<Tag>, ITagRepository
    {
        public TagRepository(ICodeDbContext context): base(context)
        {

        }    
    }
}
