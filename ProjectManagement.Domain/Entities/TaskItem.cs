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
    }

}
