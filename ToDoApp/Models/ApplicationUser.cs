using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<TaskItem> Tasks { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntil { get; set; }
    }
}
