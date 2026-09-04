using Nexus.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nexus.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .HasConversion(name => name.Value, value => WorkspaceName.Create(value).Value)
            .HasMaxLength(WorkspaceName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(w => w.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(w => w.OwnerId);
    }
}
