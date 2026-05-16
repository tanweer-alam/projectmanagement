using ProjectManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Application.DTOs
{
    public record TaskDto(
        Guid Id,
        string Title,
        string Description,
        TaskItemStatus Status,
        string StatusName,
        Guid AssigneeId,
        string AssigneeName,
        DateTime? CreatedAt,
        DateTime? StartedAt,
        DateTime? CompletedAt
        );
}
