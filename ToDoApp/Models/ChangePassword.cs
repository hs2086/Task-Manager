using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models;

public class ChangePassword
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(8)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(8)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }
    
    public string VerificationCode { get; set; }    
    public int Step { get; set; }
}