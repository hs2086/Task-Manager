using ToDoApp.Models;
using ToDoApp.ViewModel;

namespace ToDoApp.Repository;

public interface IUserRepository
{
   Task<BigUserVM> GetUsers();
   void Block(string id);
   void Delete(string id);
   ApplicationUser GetUser(string id);
}