using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ProjectManagement.Application.Tasks.Commands
{
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotEqual(Guid.Empty)
                .WithMessage("ProjectId cannot be empty.");
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title cannot be empty.")
                .MaximumLength(200)
                .WithMessage("Title cannot exceed 200 characters.") ;
            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description cannot exceed 2000 characters.");
            RuleFor(x => x.AssigneeId)
                .NotEqual(Guid.Empty)
                .WithMessage("AssigneeId cannot be empty.");
        }
    }
}
