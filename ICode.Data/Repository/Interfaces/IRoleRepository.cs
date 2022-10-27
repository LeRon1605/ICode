using Data.Entity;

namespace Data.Repository.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Role findByName(string Name);
    }
}
