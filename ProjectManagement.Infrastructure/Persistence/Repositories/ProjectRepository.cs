using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Persistence.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectManagementDbContext _dbContext;

        public ProjectRepository(ProjectManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Project?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetAllWithTasksAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Owner)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include (p => p.Owner)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include(p => p.Tasks)
                .Where(p => p.OwnerId == ownerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
        {
            await _dbContext.Projects.AddAsync(project, cancellationToken);
        }

        public void Update(Project project)
        {
            _dbContext.Projects.Update(project);
        }

        public void Remove(Project project)
        {
            _dbContext.Projects.Remove(project);
        }
    }
}
