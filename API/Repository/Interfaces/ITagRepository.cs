using System.Linq.Expressions;
using System;
using Data.Entity;

namespace API.Repository
{
    public interface ITagRepository : IRepository<Tag>
    {
        Tag GetTagWithProblem(Expression<Func<Tag, bool>> expression);
    }
}
