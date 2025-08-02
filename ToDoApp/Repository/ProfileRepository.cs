using ToDoApp.Models;

namespace ToDoApp.Repository;

public class ProfileRepository : IProfileRepository
{
    private readonly ApplicationDbContext context;

    public ProfileRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    public NameAndEmail GetNameAndEmail(string id)
    {
        ApplicationUser? user = context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return new NameAndEmail() { Name = "No Name", Email = "No Email" };
        }
        NameAndEmail nameAndEmail = new NameAndEmail()
        {
            Name = user.UserName ?? "",
            Email = user.Email ?? ""
        };
        return nameAndEmail;
    }

    public ApplicationUser? GetUserByID(string id)
    {
        return context.Users.FirstOrDefault(u => u.Id == id);
    }
}