using Microsoft.AspNetCore.Identity;
using ToDoApp.Models;
using ToDoApp.ViewModel;

namespace ToDoApp.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext context;

    public RoleRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    public List<RoleDisplay> GetRoles()
    {
        List<RoleDisplay> roleDisplays = context.Roles.Select(r => new RoleDisplay { Id = r.Id, Name = r.Name }).ToList();
        return roleDisplays;
    }


    public IdentityRole GetRoleById(string id)
    {
        return context.Roles.FirstOrDefault(r => r.Id == id);
    }

    public void Update(string id, string name)
    {
        IdentityRole role = context.Roles.FirstOrDefault(r => r.Id == id);
        role.Name = name;
        context.SaveChanges();
    }

    public void DeleteRole(string id)
    {
        IdentityRole role = context.Roles.FirstOrDefault(r => r.Id == id);
        context.Roles.Remove(role);
        context.SaveChanges();
    }
}