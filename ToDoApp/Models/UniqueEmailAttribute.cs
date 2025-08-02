using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Models
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = validationContext.GetRequiredService<ApplicationDbContext>();
            
            var email = value as string;
            if (email != null)
            {
                var emailExists = dbContext.Users.Any(u => u.Email == email);
                
                if (emailExists)
                {
                    return new ValidationResult("Email is already in use.");
                }
            }
            
            return ValidationResult.Success;
        }
    }
}