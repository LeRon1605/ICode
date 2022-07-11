using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> FindMulti(Expression<Func<T, bool>> expression);
        T FindSingle(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindAll();
        void Update(T entity);
        void Remove(T entity);
        void Add(T entity);
        bool isExist(Expression<Func<T, bool>> expression);
    }
}
