namespace ToDoApp.ViewModel;

public class UserVM
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } 
    public bool IsBlocked { get; set; }
    public DateTime? BlockedUntil { get; set; }
}