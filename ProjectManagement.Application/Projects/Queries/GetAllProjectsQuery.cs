using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Projects.Queries
{
    public record GetAllProjectsQuery : IQuery<IReadOnlyList<ProjectSummaryDto>>
    {
    }

    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IReadOnlyList<ProjectSummaryDto>>
    {
        private readonly IProjectRepository _projectRepository;
        public GetAllProjectsQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<IReadOnlyList<ProjectSummaryDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.GetAllAsync(cancellationToken);
            return projects.Select(p => new ProjectSummaryDto(
                p.Id,
                p.Name,
                p.Description,
                p.TotalTaskCount,
                p.CompletedTaskCount,
                p.CalculateProgress()
            )).ToList();
        }
    }
}
