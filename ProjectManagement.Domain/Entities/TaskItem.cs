using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class TaskItem : Entity<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    //Relationships
    public Guid ProjectId { get; set; } //FK
    public Guid AssigneeId { get; set; } //FK
    public Project Project { get; set; } = null!; //Navigation
    public User Assignee { get; set; } = null!; //Navigation

    public TaskItem() { }

    public TaskItem(
        Guid id, 
        string title, 
        string description, 
        Guid projectId, 
        Guid assigneeId) : base(id)
    {
        Title = title;
        Description = description;
        ProjectId = projectId;
        AssigneeId = assigneeId;
        Status = TaskItemStatus.ToDo;
    }

    public static TaskItem Create(string title, string description, Guid projectId, Guid assigneeId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentNullException("Task title cannot be empty.", nameof(title));
        if (projectId == Guid.Empty)
            throw new ArgumentException("Task must be associated with a project.", nameof(projectId));
        if (assigneeId == Guid.Empty)
            throw new ArgumentException("Task must be assigned to a employee.", nameof(assigneeId));
        return new TaskItem(Guid.NewGuid(), title.Trim(), description?.Trim() ?? string.Empty, projectId, assigneeId);
    }

    public void UpdateStatus(TaskItemStatus newStatus)
    {
        if (Status == newStatus) return; // No change
        
        if(Status == TaskItemStatus.Done && newStatus != TaskItemStatus.Done)
            throw new InvalidOperationException("Cannot change status from Done to another status.");

        Status = newStatus;
        switch(newStatus)
        {
            case TaskItemStatus.InProgress:
                StartedAt ??= DateTime.UtcNow;
                break;
            case TaskItemStatus.Done:
                StartedAt ??= DateTime.UtcNow;
                CompletedAt = DateTime.UtcNow;
                break;
        }
    }
}
