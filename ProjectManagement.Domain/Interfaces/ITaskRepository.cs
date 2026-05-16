using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem>> GetByAssigneeIdAsync(Guid assigneeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status, CancellationToken cancellationToken = default);
        Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
        void Update(TaskItem taskItem);
        void Remove(TaskItem taskItem);
    }
}
