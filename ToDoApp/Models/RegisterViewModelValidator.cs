using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ToDoApp.ViewModel;

namespace ToDoApp.Models
{
    internal class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator(ApplicationDbContext context)
        {
            RuleFor(x => x.Email)
                .Must(email => !context.Users.Any(u => u.Email == email))
                .WithMessage("Email must be unique.");
        }
    }
}