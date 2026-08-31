using Nexus.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Infrastructure.Persistence;

public sealed class NexusDbContext(DbContextOptions<NexusDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NexusDbContext).Assembly);
    }
}
