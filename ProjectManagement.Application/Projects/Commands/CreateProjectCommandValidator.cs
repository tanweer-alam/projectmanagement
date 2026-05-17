using FluentValidation;

namespace ProjectManagement.Application.Projects.Commands
{
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Project name cannot be empty.")
                .MaximumLength(200)
                .WithMessage("Project name cannot exceed 200 characters.");
            RuleFor(x => x.OwnerId)
                .NotEqual(Guid.Empty)
                .WithMessage("OwnerId cannot be empty.");
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Project description cannot exceed 2000 characters.");
        }
    }
}
