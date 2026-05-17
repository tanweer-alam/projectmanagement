using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Projects.Queries
{
    public record GetProjectDetailQuery(Guid ProjectId) : IQuery<ProjectDto?>;


    public class GetProjectDetailQueryHandler : IRequestHandler<GetProjectDetailQuery, ProjectDto?>
    {
        private readonly IProjectRepository _projectRepository;
        public GetProjectDetailQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<ProjectDto> Handle(GetProjectDetailQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdWithTasksAsync(request.ProjectId, cancellationToken);
            if (project == null)
            {
                throw new KeyNotFoundException($"Project with ID {request.ProjectId} not found.");
            }

            var tasks = project.Tasks.Select(t => new TaskDto(
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

            return new ProjectDto(
                project.Id,
                project.Name,
                project.Description,
                project.OwnerId,
                project.Owner?.FullName ?? "Unknown",
                project.CreatedAt,
                project.TotalTaskCount,
                project.CompletedTaskCount,
                project.CalculateProgress(),
                tasks
            );
        }
    }
}