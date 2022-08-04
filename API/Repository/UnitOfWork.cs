using API.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync();
    }
    public class UnitOfWork : IUnitOfWork
    {
        protected ICodeDbContext _context;
        public UnitOfWork(ICodeDbContext context)
        {
            _context = context;
        }
        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
