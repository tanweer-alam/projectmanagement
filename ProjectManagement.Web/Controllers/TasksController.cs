using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Tasks.Commands;
using ProjectManagement.Application.Tasks.Queries;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("assignee/{assigneeId:guid}")]
        public async Task<IActionResult> GetByAssignee(Guid assigneeId, CancellationToken cancellationToken)
        {
            var tasks = await _mediator.Send(new GetTasksByAssigneeQuery(assigneeId), cancellationToken);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
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
                return CreatedAtAction(null, new {id = taskId}, new { id = taskId});
            }
            catch (Exception e)
            {
                return BadRequest($"Failed to create {e.Message}");
            }
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request, CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var command = new UpdateTaskStatusCommand(id, request.Status);
                await _mediator.Send(command, cancellationToken);
                return Ok(new { message = "Task status updated successfully" });
            }
            catch (Exception e)
            {
                return BadRequest($"Failed to update status: {e.Message}");
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
