using System.ComponentModel.DataAnnotations;

namespace ToDoApp.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}