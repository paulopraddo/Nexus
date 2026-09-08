namespace Nexus.Domain.Workspaces;

public interface IWorkspaceMemberRepository
{
    Task<bool> ExistsAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default);

    Task<WorkspaceMember?> GetAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<WorkspaceMember>> GetByWorkspaceIdAsync(
        Guid workspaceId, CancellationToken cancellationToken = default);

    Task AddAsync(WorkspaceMember member, CancellationToken cancellationToken = default);

    void Remove(WorkspaceMember member);
}
