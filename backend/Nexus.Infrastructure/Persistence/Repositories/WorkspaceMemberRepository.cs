using Nexus.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Infrastructure.Persistence.Repositories;

public sealed class WorkspaceMemberRepository(NexusDbContext dbContext) : IWorkspaceMemberRepository
{
    public Task<bool> ExistsAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.WorkspaceMembers.AnyAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId, cancellationToken);

    public Task<WorkspaceMember?> GetAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.WorkspaceMembers.FirstOrDefaultAsync(
            m => m.WorkspaceId == workspaceId && m.UserId == userId, cancellationToken);

    public async Task<IReadOnlyCollection<WorkspaceMember>> GetByWorkspaceIdAsync(
        Guid workspaceId, CancellationToken cancellationToken = default) =>
        await dbContext.WorkspaceMembers.Where(m => m.WorkspaceId == workspaceId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<WorkspaceMember>> GetByUserIdAsync(
        Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.WorkspaceMembers.Where(m => m.UserId == userId).ToListAsync(cancellationToken);

    public async Task AddAsync(WorkspaceMember member, CancellationToken cancellationToken = default) =>
        await dbContext.WorkspaceMembers.AddAsync(member, cancellationToken);

    public void Remove(WorkspaceMember member) => dbContext.WorkspaceMembers.Remove(member);
}
