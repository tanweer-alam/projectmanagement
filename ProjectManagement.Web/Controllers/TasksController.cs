using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Tasks.Commands;
using ProjectManagement.Application.Tasks.Queries;
using ProjectManagement.Application.Users.Queries;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITaskRepository _taskRepository;

        public TasksController(
            IMediator mediator,
            ITaskRepository taskRepository)
        {
            _mediator = mediator;
            _taskRepository = taskRepository;
        }

        [HttpGet("assignee/{assigneeId:guid}")]
        public async Task<IActionResult> GetByAssignee(Guid assigneeId, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
            {
                var currentUser = await GetCurrentUserAsync(cancellationToken);
                if (currentUser is null || currentUser.Id != assigneeId)
                {
                    return Forbid();
                }
            }

            var tasks = await _mediator.Send(new GetTasksByAssigneeQuery(assigneeId), cancellationToken);
            return Ok(tasks);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var command = new CreateTaskCommand(
                    request.ProjectId,
                    request.Title,
                    request.Description,
                    request.AssigneeId);

                var taskId = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(null, new { id = taskId }, new { id = taskId });
            }
            catch (Exception e)
            {
                return BadRequest($"Failed to create {e.Message}");
            }
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
                if (task is null)
                {
                    return NotFound();
                }

                var currentUser = await GetCurrentUserAsync(cancellationToken);
                if (currentUser is null || task.AssigneeId != currentUser.Id)
                {
                    return Forbid();
                }

                var command = new UpdateTaskStatusCommand(id, request.Status);
                await _mediator.Send(command, cancellationToken);
                return Ok(new { message = "Task status updated successfully" });
            }
            catch (Exception e)
            {
                return BadRequest($"Failed to update status: {e.Message}");
            }
        }

        private async Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            try
            {
                var user = await _mediator.Send(new GetUserByEmailQuery(email.ToLowerInvariant()), cancellationToken);
                return user;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}

public record CreateTaskRequest(
    Guid ProjectId,
    string Title,
    string Description,
    Guid AssigneeId
    );

public record UpdateTaskStatusRequest(TaskItemStatus Status);
