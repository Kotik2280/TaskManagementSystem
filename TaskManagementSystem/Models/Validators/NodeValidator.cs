using FluentValidation;

namespace TaskManagementSystem.Models.Validators
{
    public class NodeValidator : AbstractValidator<Node>
    {
        public NodeValidator()
        {
            RuleFor(node => node.Title)
                .NotEmpty()
                .WithMessage("Please specify a title");
            RuleFor(node => node.Title)
                .Length(2, 30)
                .WithMessage("The title should be highter 2 and lower 30 characters");
            RuleFor(node => node.Description)
                .NotEmpty()
                .WithMessage("Please specify a description");
            RuleFor(node => node.Description)
                .Length(2, 150)
                .WithMessage("The description should be highter 2 and lower 150 characters");
        }
    }
}
