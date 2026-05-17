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

        //Navigation properties
        private readonly List<Project> _ownedProjects = new();
        public IReadOnlyCollection<Project> OwnedProjects => _ownedProjects.AsReadOnly();
        private readonly List<TaskItem> _assignedTasks = new();
        public IReadOnlyCollection<TaskItem> AssignedTasks => _assignedTasks.AsReadOnly();

        public User() : base() { }
        public User(Guid id, string email, string firstName, string lastName, UserRole role) : base(id)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }

        public bool IsEmployee => Role == UserRole.Employee;
        public bool IsAdmin => Role == UserRole.Admin;
        public string FullName => $"{FirstName} {LastName}";

        public static User Create(string email, string firstName, string lastName, UserRole role = UserRole.Employee)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty", nameof(lastName));

            return new User(Guid.NewGuid(), email.ToLowerInvariant().Trim(), firstName.Trim(),
                lastName.Trim(), role);
        }
    }
}
