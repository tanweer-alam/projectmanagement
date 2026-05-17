using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Projects.Queries;

namespace ProjectManagement.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public DashboardDto? Dashboard { get; set; }
        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Dashboard = await _mediator.Send(new GetDashboardQuery(), cancellationToken);
        }
    }
}
