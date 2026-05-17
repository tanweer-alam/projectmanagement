using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Application.Tasks.Commands;
using ProjectManagement.Application.Tasks.Queries;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;

        public IndexModel(
            IMediator mediator,
            IUserRepository userRepository,
            ITaskRepository taskRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }

        public DashboardDto? Dashboard { get; set; }
        public IReadOnlyList<TaskDto> AssignedTasks { get; set; } = new List<TaskDto>();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            if (User.IsInRole("Admin"))
            {
                Dashboard = await _mediator.Send(new GetDashboardQuery(), cancellationToken);
                return;
            }

            var currentUser = await GetCurrentUserAsync(cancellationToken);
            if (currentUser is not null)
            {
                AssignedTasks = await _mediator.Send(
                    new GetTasksByAssigneeQuery(currentUser.Id),
                    cancellationToken);
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(
            Guid taskId,
            int newStatus,
            CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
            if (task is null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin"))
            {
                var currentUser = await GetCurrentUserAsync(cancellationToken);
                if (currentUser is null || task.AssigneeId != currentUser.Id)
                {
                    return Forbid();
                }
            }

            await _mediator.Send(
                new UpdateTaskStatusCommand(taskId, (TaskItemStatus)newStatus),
                cancellationToken);

            return RedirectToPage();
        }

        private async Task<ProjectManagement.Domain.Entities.User?> GetCurrentUserAsync(CancellationToken cancellationToken)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value;

            return string.IsNullOrWhiteSpace(email)
                ? null
                : await _userRepository.GetByEmailAsync(email.ToLowerInvariant(), cancellationToken);
        }
    }
}
