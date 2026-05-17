using MediatR;
using ProjectManagement.Application.Common.Caching;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Projects.Queries
{
    public record GetDashboardQuery : IQuery<DashboardDto>;

    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDashboardCache _dashboardCache;

        public GetDashboardQueryHandler(IProjectRepository projectRepository, IDashboardCache dashboardCache)
        {
            _projectRepository = projectRepository;
            _dashboardCache = dashboardCache;
        }

        public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            if (_dashboardCache.TryGet(out var cachedDashboard) && cachedDashboard is not null)
            {
                return cachedDashboard;
            }

            var projects = await _projectRepository.GetAllWithTasksAsync(cancellationToken);
            var stats = GetProgressStatistics(projects);

            var projectSummaries = projects.Select(p => new ProjectSummaryDto(
                p.Id,
                p.Name,
                p.Description,
                p.TotalTaskCount,
                p.CompletedTaskCount,
                p.CalculateProgress()
            ));

            var dashboard = new DashboardDto(
                stats.TotalProjects,
                stats.Totaltasks,
                stats.Completedtasks,
                stats.AverageProgress,
                projectSummaries.ToList()
            );

            _dashboardCache.Set(dashboard);
            return dashboard;
        }

        private ProgressStatistics GetProgressStatistics(IEnumerable<Project> projects)
        {
            ArgumentNullException.ThrowIfNull(projects, nameof(projects));

            var projectList = projects.ToList();
            var totalProjects = projectList.Count;
            var totalTasks = projectList.Sum(p => p.TotalTaskCount);
            var completedTasks = projectList.Sum(p => p.CompletedTaskCount);
            var averageProgress = CalculateAverageProgress(totalTasks, completedTasks);

            return new ProgressStatistics(totalProjects, totalTasks, completedTasks, averageProgress);
        }

        private decimal CalculateAverageProgress(int totalTasks, int completedTasks)
        {
            if (totalTasks == 0) return 0;
            return Math.Round((decimal)completedTasks / totalTasks * 100, 2);
        }
    }
}
