using API.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IRepository<T> where T: class
    {
        T FindByID(string Id);
        IEnumerable<T> FindMulti(Expression<Func<T, bool>> expression);
        T FindSingle(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindAll();
        void Update(T entity);
        void Remove(T entity);
        void Add(T entity);
        bool isExist(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        int Count();
        Task<T> FindByIDAsync(string Id);
        Task<PagingList<T>> GetPageAsync(int page, int pageSize, Expression<Func<T, bool>> expression, params Expression<Func<T, object>> []includes);
    }
}
