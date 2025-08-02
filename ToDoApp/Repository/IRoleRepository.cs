using Microsoft.AspNetCore.Identity;
using ToDoApp.ViewModel;

namespace ToDoApp.Repository;

public interface IRoleRepository
{
    List<RoleDisplay> GetRoles();
    IdentityRole GetRoleById(string id);
    void Update(string id, string name);
    void DeleteRole(string id);
}