using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Application.Tasks.Commands
{
    public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusCommandValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEqual(Guid.Empty)
                .WithMessage("TaskId cannot be empty.");
            RuleFor(x => x.NewStatus)
               .IsInEnum()
               .WithMessage("Invalid task status value.");
        }
    }
}
