using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Application.DTOs
{
    public record ProjectDto(
        Guid Id,
        string Name,
        string Description,
        Guid Ownerid,
        string OwnerName,
        DateTime CreatedAt,
        int TotalTaskCount,
        int CompletedTaskCount,
        decimal Progress,
        IReadOnlyList<TaskDto> Tasks
        );


    public record ProjectSummaryDto(
        Guid Id,
        string Name,
        string Description,
        int TotalTaskCount,
        int CompletedTaskCount,
        decimal Progress
        );

 
}
