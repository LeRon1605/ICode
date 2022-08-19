using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IService<T> where T : class
    {
        public IEnumerable<T> GetAll();
        public T FindByID(string ID);
        public Task Add(T entity);
        public Task<bool> Remove(string ID);
        public Task<bool> Update(string ID, object entity);
    }
}
