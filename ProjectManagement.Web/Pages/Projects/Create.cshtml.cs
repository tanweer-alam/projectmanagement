using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Projects.Commands;
using ProjectManagement.Application.Users.Queries;

namespace ProjectManagement.Web.Pages.Projects
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        public CreateModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IReadOnlyList<UserDto> Admins { get; set; } = new List<UserDto>();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Admins = await _mediator.Send(new GetAllAdminsQuery(), cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync(
            string name,
            string description,
            Guid ownerId,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                Admins = await _mediator.Send(new GetAllAdminsQuery(), cancellationToken);
                return Page();
            }
            var command = new CreateProjectCommand(name, description, ownerId);
            var projectId = await _mediator.Send(command, cancellationToken);
            return RedirectToPage("/Projects/Details", new { id = projectId });
        }
    }
}
