using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Domain.Entities
{
    public class User : Entity<Guid>
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        //Relationships
        public ICollection<Project> OwnedProjects { get; set; } = new List<Project>(); //Navigation
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>(); //Navigation

        public User() : base() { }
        public User(Guid id, string email, string firstName, string lastName, UserRole role) : base(id)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }

    }
}
