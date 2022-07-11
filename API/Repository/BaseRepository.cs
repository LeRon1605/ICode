using API.Models.Data;
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

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
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
            return !(_context.Set<T>().Where(expression) == null);
        }
    }
}
