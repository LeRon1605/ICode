using API.Models.DTO;
using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected ICodeDbContext _context;
        public BaseRepository(ICodeDbContext context)
        {
            _context = context;
        }
        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public IEnumerable<T> FindAll()
        {
            return _context.Set<T>();
        }

        public IEnumerable<T> FindMulti(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }
        public T FindSingle(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefault(expression);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public bool isExist(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).Count() > 0;
        }

        public async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public Task<PagingList<T>> GetPageAsync(int page, int pageSize, Expression<Func<T, bool>> expression, params Expression<Func<T, object>> []includes)
        {
            IQueryable<T> data = _context.Set<T>();
            foreach (var prop in includes)
            {
                data = data.Include(prop);
            }
            data = data.Where(expression);
            return Task.FromResult(new PagingList<T>
            {
                Page = page,
                TotalPage = (int)(Math.Ceiling( (float)data.Count() / pageSize )),
                Data = data.Skip(pageSize * (page - 1)).Take(pageSize)
            });
        }

        public T FindByID(string Id)
        {
            return _context.Set<T>().Find(Id);
        }

        public async Task<T> FindByIDAsync(string Id)
        {
            return await _context.Set<T>().FindAsync(Id);
        }
    }
}
