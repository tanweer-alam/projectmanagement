using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Projects.Commands;
using ProjectManagement.Application.Projects.Queries;

namespace ProjectManagement.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDashboardQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var projects = await _mediator.Send(new GetAllProjectsQuery(), cancellationToken);
            return Ok(projects);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var project = await _mediator.Send(new GetProjectDetailQuery(id), cancellationToken);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        //should be for Admin only
        public async Task<IActionResult> Create([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var command = new CreateProjectCommand(request.Name, request.Description, request.OwnerId);
            var projectId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = projectId }, new { id = projectId });
        }
    }
    public record CreateProjectRequest(
        string Name,
        string Description,
        Guid OwnerId
        );
}

