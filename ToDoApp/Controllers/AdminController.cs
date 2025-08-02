using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ToDoApp.Models;
using ToDoApp.Repository;
using ToDoApp.ViewModel;

namespace ToDoApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager,
                        IUserRepository userRepository,
                        IRoleRepository roleRepository,
                        ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Users()
        {
            var users = await userRepository.GetUsers();
            return View(users);
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserCreate user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userCreate = new ApplicationUser();
                userCreate.UserName = user.Name;
                userCreate.Email = user.Email;
                userCreate.PasswordHash = user.Password;

                IdentityResult result = await _userManager.CreateAsync(userCreate, user.Password);
                if (result.Succeeded)
                {
                    IdentityResult res = await _userManager.AddToRoleAsync(userCreate, "User");
                    if(!res.Succeeded)
                        foreach (var error in res.Errors)
                            ModelState.AddModelError("", error.Description);
                    return RedirectToAction("Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BlockUser(string id, string returnView = "Users")
        {
            userRepository.Block(id);
            return RedirectToAction(returnView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(string id, string returnView = "Users")
        {
            userRepository.Delete(id);
            return RedirectToAction(returnView);
        }


        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            var recentUsers = await users
                .OrderByDescending(u => u.Id)
                .Take(5)
                .ToListAsync();

            var model = new AdminDashboardVM
            {
                UserCount = await users.CountAsync(),
                BlockedUserCount = await users.CountAsync(u => u.IsBlocked),
                Roles = await _roleManager.Roles.ToListAsync(),
                RecentUsers = recentUsers.Select(u => new UserVM
                {
                    Id = u.Id,
                    Name = u.UserName ?? "",
                    Email = u.Email ?? "",
                    IsBlocked = u.IsBlocked,
                    BlockedUntil = u.BlockedUntil,
                    Role = string.Join(", ", _userManager.GetRolesAsync(u))
                }).ToList()
            };

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> AddRoleTo()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return Ok("User is already in role Admin");
                }

                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!roleResult.Succeeded)
                        return BadRequest("Failed to create Admin role");
                }

                var result = await _userManager.AddToRoleAsync(user, "Admin");

                return result.Succeeded
                    ? Ok("Successfully added to Admin role")
                    : BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole();
                role.Name = roleVM.Name;
                IdentityResult result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(roleVM);
        }

        ///////////////////////////////////////////////////////////////////////
        public IActionResult Roles()
        {
            List<RoleDisplay> roleDisplay = roleRepository.GetRoles();
            return View(roleDisplay);
        }
        [HttpGet]
        public IActionResult EditRole(string id)
        {
            RoleVM role = new RoleVM();
            role.Name = roleRepository?.GetRoleById(id)?.Name ?? "";
            TempData["id"] = id;
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRole(RoleVM role)
        {
            if (ModelState.IsValid)
            {
                string id = TempData["id"]?.ToString() ?? "";
                roleRepository.Update(id, role.Name);
                return RedirectToAction("Roles");
            }
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRole(string id)
        {
            roleRepository.DeleteRole(id);
            return RedirectToAction("Roles");
        }

        [HttpPost("Admin/AssignRoleFromUsers")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoleFromUsers(string userId, string roleName)
        {
            return await AssignRole(userId, roleName, "Users");
        }

        [HttpPost("Admin/AssignRoleFromBlocked")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoleFromBlocked(string userId, string roleName)
        {
            return await AssignRole(userId, roleName, "BlockedUsers");
        }

        // Private helper method
        private async Task<IActionResult> AssignRole(string userId, string roleName, string returnView)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction(returnView);
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                TempData["SuccessMessage"] = $"Role changed to '{roleName}' for {user.UserName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role");
                TempData["ErrorMessage"] = $"Error assigning role: {ex.Message}";
            }

            return RedirectToAction(returnView);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockedUsers()
        {
            try
            {
                var viewModel = new BigUserVM
                {
                    Users = new List<UserVM>(),
                    Roles = new List<string>()
                };

                // Get blocked users with their roles
                var blockedUsers = await _userManager.Users
                    .Where(u => u.IsBlocked)
                    .OrderByDescending(u => u.BlockedUntil)
                    .ToListAsync();

                // Convert to UserVM objects
                foreach (var user in blockedUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    viewModel.Users.Add(new UserVM
                    {
                        Id = user.Id,
                        Name = user.UserName ?? "",
                        Email = user.Email ?? "",
                        Role = roles.FirstOrDefault() ?? "",
                        IsBlocked = user.IsBlocked,
                        BlockedUntil = user.BlockedUntil
                    });
                }

                // Get all roles
                viewModel.Roles = await _roleManager.Roles
                    .Select(r => r.Name)
                    .OrderBy(r => r)
                    .ToListAsync();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading blocked users");
                TempData["ErrorMessage"] = "An error occurred while loading blocked users";
                return View(new BigUserVM 
                { 
                    Users = new List<UserVM>(),
                    Roles = new List<string>()
                });
            }
        }

    }
}