using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Projects.Queries;
using ProjectManagement.Application.Tasks.Commands;
using ProjectManagement.Application.Users.Queries;

namespace ProjectManagement.Web.Pages.Projects
{
    [Authorize(Policy = "AdminOnly")]
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;
        public DetailsModel(IMediator mediator)
        {
            _mediator = mediator;
        }
        public ProjectDto? Project { get; set; }
        public IReadOnlyList<UserDto> Employees { get; set; } = new List<UserDto>();

        public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
        {
            Project = await _mediator.Send(new GetProjectDetailQuery(id), cancellationToken);
            if (Project == null)
            {
                return Page();
            }
            Employees = await _mediator.Send(new GetAllEmployeesQuery(), cancellationToken);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
            Guid projectId,
            string title,
            string description,
            Guid assigneeId,
            CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            try
            {
                var command = new CreateTaskCommand(
                    projectId,
                    title,
                    description,
                    assigneeId);
                await _mediator.Send(command, cancellationToken);
            }
            catch (Exception)
            {

            }
            return RedirectToPage(new { id = projectId });
        }

    }
}
