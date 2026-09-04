using Nexus.Domain.Common;

namespace Nexus.Infrastructure.Persistence;

public sealed class UnitOfWork(NexusDbContext dbContext) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
