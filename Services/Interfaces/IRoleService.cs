using CodeStudy.Models;

namespace Services.Interfaces
{
    public interface IRoleService
    {
        RoleDTO FindById(string ID);
        RoleDTO FindByName(string Name);
    }
}
