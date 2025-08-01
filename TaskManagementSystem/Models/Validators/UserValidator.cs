using FluentValidation;

namespace TaskManagementSystem.Models.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty()
                .WithMessage("Please specify a nickname");
            RuleFor(user => user.Password)
                .NotEmpty()
                .WithMessage("Please specify a password");
            RuleFor(user => user.Password)
                .Length(6, 50)
                .WithMessage("The password should be higher 6 and lower 50 characters");
        }
    }
}
