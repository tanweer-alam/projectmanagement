using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Application.Tasks.Queries
{
    public record GetTasksByAssigneeQuery(Guid AssigneeId) : IQuery<IReadOnlyList<TaskDto>>;

    public class GetTasksByAssigneeQueryHandler : IRequestHandler<GetTasksByAssigneeQuery, IReadOnlyList<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        public GetTasksByAssigneeQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksByAssigneeQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetByAssigneeIdAsync(request.AssigneeId, cancellationToken);
            return tasks.Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.Status.ToString(),
                t.AssigneeId,
                t.Assignee?.FullName ?? "Unknown",
                t.CreatedAt,
                t.StartedAt,
                t.CompletedAt
            )).ToList();
        }
    }
}