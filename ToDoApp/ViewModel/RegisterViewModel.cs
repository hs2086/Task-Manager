using System.ComponentModel.DataAnnotations;
using ToDoApp.Models;

namespace ToDoApp.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [UniqueEmail]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Verification Code is required.")]
        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; }
        public int Step { get; set; } = 1; // 1=Basic info, 2=Verification, 3=Password
    }
}

