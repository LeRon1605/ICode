using Data.Entity;

namespace API.Repository
{
    public interface IRoleRepository : IRepository<Role>
    {
        Role findByName(string Name);
    }
}
