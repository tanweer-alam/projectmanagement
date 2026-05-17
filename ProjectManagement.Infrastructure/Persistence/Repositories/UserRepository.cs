using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectManagementDbContext _dbContext;

        public UserRepository(ProjectManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Users.FindAsync(new object[] { id }, cancellationToken);
            return entity;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(u => u.Role == role)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
        {
            return await GetByRoleAsync(UserRole.Employee, cancellationToken);
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return _dbContext.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
        }

        public void Add(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Add(user);
        }

        public void Update(User user)
        {
            _dbContext.Users.Update(user);
        }

        public void Remove(User user)
        {
            _dbContext.Users.Remove(user);
        }

        
    }
}
