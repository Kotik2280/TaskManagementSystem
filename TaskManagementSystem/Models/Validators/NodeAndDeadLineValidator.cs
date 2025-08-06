using FluentValidation;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Models.Validators
{
    public class NodeAndDeadLineValidator : AbstractValidator<NodeAndDeadLine>
    {
        public NodeAndDeadLineValidator()
        {
            RuleFor(o => o.Node).SetValidator(new NodeValidator());
            RuleFor(o => o.DeadLineTime.Days)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Days count should be more or equal to 0");
            RuleFor(o => o.DeadLineTime.Hours)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Hours count should be more or equal to 0");
            RuleFor(o => o.DeadLineTime.Days)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minutes count should be more or equal to 0");
        }
    }
}
