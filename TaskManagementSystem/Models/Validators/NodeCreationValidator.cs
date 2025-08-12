using FluentValidation;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Models.Validators
{
    public class NodeCreationValidator : AbstractValidator<NodeCreation>
    {
        private readonly NodeContext _nodeContext;
        public NodeCreationValidator(NodeContext nodeContext)
        {
            _nodeContext = nodeContext;

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
            RuleFor(o => o.Responsible)
                .Must(name => HasResponsibleNames(name))
                .WithMessage("Responsible user with this name doesn't exist");
        }
        private bool HasResponsibleNames(string name)
        {
            if (name is null)
                return true;

            return _nodeContext.Users.Any(u => u.Name == name);
        }
    }
}
