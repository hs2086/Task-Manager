using ToDoApp.Models;

namespace ToDoApp.Repository;

public interface IProfileRepository
{
    NameAndEmail GetNameAndEmail(string id);
    ApplicationUser? GetUserByID(string id);
}