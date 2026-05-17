using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Application.Users.Queries
{
    public class GetAllAdminsQuery : IQuery<IReadOnlyList<UserDto>>
    {
    }

    public class GetAllAdminsQueryHandler : IRequestHandler<GetAllAdminsQuery, IReadOnlyList<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        public GetAllAdminsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IReadOnlyList<UserDto>> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
        {
            var employees = await _userRepository.GetByRoleAsync(UserRole.Admin ,cancellationToken);
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
