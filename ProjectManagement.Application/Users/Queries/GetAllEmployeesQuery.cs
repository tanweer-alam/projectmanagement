using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Users.Queries
{
    public class GetAllEmployeesQuery : IQuery<IReadOnlyList<UserDto>>
    {
    }

    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IReadOnlyList<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        public GetAllEmployeesQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IReadOnlyList<UserDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _userRepository.GetAllEmployeesAsync(cancellationToken);
            return employees.Select(e => new UserDto(
                e.Id,
                e.Email, 
                e.FirstName, 
                e.LastName, 
                e.Role
            )).ToList();
        }
    }
}
