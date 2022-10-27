using Data;
using Data.Entity;
using Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repository
{ 
    public class TagRepository: BaseRepository<Tag>, ITagRepository
    {
        public TagRepository(ICodeDbContext context): base(context)
        {

        }

        public Tag GetTagWithProblem(Expression<Func<Tag, bool>> expression)
        {
            return _context.Tags.Include(x => x.Problems).FirstOrDefault(expression);
        }
    }
}
