using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models;

public class UserCreate
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [UniqueEmail]
    public string Email { get; set; }
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(3)]
    public string Name { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]    
    public string ConfirmPassword { get; set; } 
}