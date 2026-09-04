using Nexus.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Infrastructure.Persistence.Repositories;

public sealed class WorkspaceRepository(NexusDbContext dbContext) : IWorkspaceRepository
{
    public Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Workspaces.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Workspace>> GetByOwnerIdAsync(
        Guid ownerId, CancellationToken cancellationToken = default) =>
        await dbContext.Workspaces.Where(w => w.OwnerId == ownerId).ToListAsync(cancellationToken);

    public async Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default) =>
        await dbContext.Workspaces.AddAsync(workspace, cancellationToken);

    public void Remove(Workspace workspace) => dbContext.Workspaces.Remove(workspace);
}
