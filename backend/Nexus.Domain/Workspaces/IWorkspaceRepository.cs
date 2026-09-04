namespace Nexus.Domain.Workspaces;

public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Workspace>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default);

    void Remove(Workspace workspace);
}
