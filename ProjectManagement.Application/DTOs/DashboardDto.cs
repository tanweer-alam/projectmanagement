using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Application.DTOs
{
    public record DashboardDto(
        int TotalProjects,
        int TotalTask,
        int Completedtasks,
        decimal AverageProgress,
        IReadOnlyList<ProjectSummaryDto> Projects
        );

    public record ProgressStatistics(
        int TotalProjects,
        int Totaltasks,
        int Completedtasks,
        decimal AverageProgress
        );
}
