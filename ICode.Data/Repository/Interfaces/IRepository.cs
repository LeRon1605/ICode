using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using Models.DTO;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;

namespace Data.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T FindByID(string Id);
        IEnumerable<T> FindMulti(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindMulti(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
        T FindSingle(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindAll();
        void Update(T entity);
        void Remove(T entity);
        void Add(T entity);
        bool isExist(Expression<Func<T, bool>> expression);
        int Count();
        Task AddAsync(T entity);
        Task<T> FindByIDAsync(string Id);
        Task<PagingList<T>> GetPageAsync(int page, int pageSize, Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);
        Task<PagingList<T>> GetPageAsync(int page, int pageSize, Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
    }
}
