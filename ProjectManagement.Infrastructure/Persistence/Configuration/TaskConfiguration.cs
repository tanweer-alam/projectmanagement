

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configuration
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.StartedAt)
                .IsRequired(false);

            builder.Property(t => t.CompletedAt)
                .IsRequired(false);

            builder.Property(t => t.ProjectId)
                .IsRequired();

            builder.Property(t => t.AssigneeId)
                .IsRequired();

            builder.HasIndex(t => t.AssigneeId);
            builder.HasIndex(t => t.ProjectId);
            builder.HasIndex(t => t.Status);
        }
    }
}
