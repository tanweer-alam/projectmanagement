using MediatR;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Application.Projects.Queries
{
    public record GetDashboardQuery : IQuery<DashboardDto>;

    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
    {
        private readonly IProjectRepository _projectRepository;
        public GetDashboardQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
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
            return new DashboardDto(
                    stats.TotalProjects,
                    stats.Totaltasks,
                    stats.Completedtasks,
                    stats.AverageProgress,
                    projectSummaries.ToList()
                );
        }

        private ProgressStatistics GetProgressStatistics(IEnumerable<Project> projects)
        {
            ArgumentNullException.ThrowIfNull(projects, nameof(projects));

            var projectList = projects.ToList();

            var totalProjects = projectList.Count;
            var totalTasks = projectList.Sum(p => p.TotalTaskCount);
            var completedTasks = projectList.Sum(p => p.CompletedTaskCount);
            var averageProgress = CalculateAvergaeProgress(totalTasks, completedTasks);
            return new ProgressStatistics(totalProjects, totalTasks, completedTasks, averageProgress);
        }

        
        private decimal CalculateAvergaeProgress(int totaltasks, int completedtasks)
        {
            if (totaltasks == 0) return 0;
            return Math.Round((decimal)completedtasks / totaltasks * 100, 2);
        }
    }

}
