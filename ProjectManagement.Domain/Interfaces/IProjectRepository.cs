using ProjectManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Project?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetAllWithTasksAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
        Task AddAsync(Project project, CancellationToken cancellationToken = default);
        void Update(Project project);
        void Remove(Project project);
    }
}
