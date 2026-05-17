using ProjectManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Domain.Entities
{
    public class Project : AggregateRoot<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        //FK
        public Guid OwnerId { get; private set; }
        //Navigation properties
        public User Owner { get; private set; } = null!;
        private readonly List<TaskItem> _tasks = new();
        public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

        public Project() : base() { }
        public Project(Guid id, string name, string description, Guid ownerid) : base(id)
        {
            Name = name;
            Description = description;
            OwnerId = ownerid;
        }

        public int TotalTaskCount => _tasks.Count;
        public int CompletedTaskCount => _tasks.Count(t => t.Status == Enums.TaskItemStatus.Done);

        public static Project Create(string name, string description, Guid ownerid)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Project name cannot be empty.", nameof(name));
            if (ownerid == Guid.Empty)
                throw new ArgumentException("OwnerId is required.", nameof(ownerid));

            return new Project(Guid.NewGuid(), name.Trim(), description?.Trim() ?? string.Empty, ownerid);
        }

        public TaskItem AddTask(string title, string description, Guid assigneeId)
        {
            if (assigneeId == Guid.Empty)
                throw new ArgumentException("Task must be assigned to a employee.", nameof(assigneeId));
            var task = TaskItem.Create(title, description, this.Id, assigneeId);
            _tasks.Add(task);
            return task;
        }

        public decimal CalculateProgress()
        {
            if (_tasks.Count == 0) return 0;
            return Math.Round((decimal)CompletedTaskCount / TotalTaskCount * 100, 2);
        }
    }
}
