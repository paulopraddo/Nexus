namespace Nexus.Domain.Boards;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Board>> GetByWorkspaceIdAsync(Guid workspaceId, CancellationToken cancellationToken = default);

    Task AddAsync(Board board, CancellationToken cancellationToken = default);

    void Remove(Board board);
}
