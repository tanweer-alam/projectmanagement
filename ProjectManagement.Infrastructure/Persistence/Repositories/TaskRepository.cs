using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using ProjectManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ProjectManagementDbContext _dbContext;

        public TaskRepository(ProjectManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // simple find without related entities
            var entity = await _dbContext.Tasks.FindAsync(new object[] { id }, cancellationToken);
            return entity;
        }

        public async Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Include(t => t.Assignee)
                .Where(t => t.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Where(t => t.AssigneeId == assigneeId)
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Where(t => t.Status == status)
                .Include(t => t.Project)
                .Include(t => t.Assignee)
                .ToListAsync(cancellationToken);
        }

        public void Add(TaskItem taskItem, CancellationToken cancellationToken = default)
        {
            _dbContext.Tasks.Add(taskItem);
        }

        public void Update(TaskItem taskItem)
        {
            _dbContext.Tasks.Update(taskItem);
        }

        public void Remove(TaskItem taskItem)
        {
            _dbContext.Tasks.Remove(taskItem);
        }
    }
}
