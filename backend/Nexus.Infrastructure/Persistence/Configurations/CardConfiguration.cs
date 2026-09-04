using Nexus.Domain.Boards;
using Nexus.Domain.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nexus.Infrastructure.Persistence.Configurations;

public sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("cards");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasConversion(title => title.Value, value => CardTitle.Create(value).Value)
            .HasMaxLength(CardTitle.MaxLength)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(c => c.BoardId)
            .HasColumnName("board_id")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne<Board>()
            .WithMany()
            .HasForeignKey(c => c.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.BoardId);
    }
}
