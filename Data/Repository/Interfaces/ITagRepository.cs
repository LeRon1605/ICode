using System.Linq.Expressions;
using System;
using Data.Entity;

namespace Data.Repository.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Tag GetTagWithProblem(Expression<Func<Tag, bool>> expression);
    }
}
