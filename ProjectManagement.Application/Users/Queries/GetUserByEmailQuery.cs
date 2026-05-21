using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ProjectManagement.Application.Users.Queries
{
    public record GetUserByEmailQuery(string Email) : IQuery<UserDto?>;

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;
        public GetUserByEmailQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email '{request.Email}' not found.");
            }

            return new UserDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role
            );
        }
    }
}
