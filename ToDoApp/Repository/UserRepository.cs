using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;
using ToDoApp.ViewModel;

namespace ToDoApp.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext context;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.context = context;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }
    public async Task<BigUserVM> GetUsers()
    {
        var big = new BigUserVM();
        var userVMs = new List<UserVM>();

        // Get all users with their basic info
        var users = await context.Users.ToListAsync();

        foreach (var user in users)
        {
            // Safely get roles (empty list if none)
            var roles = await userManager.GetRolesAsync(user) ?? new List<string>();

            userVMs.Add(new UserVM
            {
                Id = user.Id,
                Name = user.UserName ?? "N/A",  // Handle null username
                Email = user.Email ?? "N/A",    // Handle null email
                Role = roles.FirstOrDefault() ?? "User",// Take first role or default
                IsBlocked = user.IsBlocked,
                BlockedUntil = user.BlockedUntil
            });
        }

        big.Users = userVMs;
        big.Roles = context.Roles.Select(r => r.Name).ToList() ?? new List<string>();
        return big;
    }

    public void Block(string id)
    {
        ApplicationUser user = context.Users.FirstOrDefault(u => u.Id == id);

        user.IsBlocked = !user.IsBlocked;
        user.BlockedUntil = DateTime.Now.AddMonths(1);

        context.SaveChanges();
    }

    public void Delete(string id)
    {
        ApplicationUser user = context.Users.FirstOrDefault(u => u.Id == id);
        context.Users.Remove(user);
        context.SaveChanges();
    }

    public ApplicationUser GetUser(string id)
    {
        return context.Users.FirstOrDefault(u => u.Id == id) ?? new ApplicationUser();
    }






}
