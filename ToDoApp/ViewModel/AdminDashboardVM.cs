using Microsoft.AspNetCore.Identity;
using ToDoApp.Models;

namespace ToDoApp.ViewModel;

public class AdminDashboardVM
{
    public int UserCount { get; set; }
    public int BlockedUserCount { get; set; }
    public List<IdentityRole> Roles { get; set; }
    public List<UserVM> RecentUsers { get; set; }
}