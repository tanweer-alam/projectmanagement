using ProjectManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Domain.Entities
{
    public class Project : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //Relationships
        public Guid OwnerId { get; set; } //FK
        public User Owner { get; set; } = null!; //Navigation
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

        public Project() : base() { }
        public Project(Guid id, string name, string description, Guid ownerid) : base(id)
        {
            Name = name;
            Description = description;
            OwnerId = ownerid;
        }
    }
}
