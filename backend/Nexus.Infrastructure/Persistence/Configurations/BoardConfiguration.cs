using Nexus.Domain.Boards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nexus.Infrastructure.Persistence.Configurations;

public sealed class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("boards");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasConversion(name => name.Value, value => BoardName.Create(value).Value)
            .HasMaxLength(BoardName.MaxLength)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(b => b.WorkspaceId)
            .HasColumnName("workspace_id")
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne<Nexus.Domain.Workspaces.Workspace>()
            .WithMany()
            .HasForeignKey(b => b.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.WorkspaceId);
    }
}
