using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.Users.Queries;

namespace ProjectManagement.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetAllEmployees(CancellationToken cancellationToken)
        {
            var employees = await _mediator.Send(new GetAllEmployeesQuery(), cancellationToken);
            return Ok(employees);
        }
    }
}
