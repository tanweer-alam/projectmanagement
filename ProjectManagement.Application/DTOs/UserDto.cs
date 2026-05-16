using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Application.DTOs
{
    public record UserDto(Guid Id, string Email, string FirstName, string LastName, UserRole Role);
}
