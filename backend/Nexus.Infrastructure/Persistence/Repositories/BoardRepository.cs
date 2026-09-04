using Nexus.Domain.Boards;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Infrastructure.Persistence.Repositories;

public sealed class BoardRepository(NexusDbContext dbContext) : IBoardRepository
{
    public Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Boards.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Board>> GetByWorkspaceIdAsync(
        Guid workspaceId, CancellationToken cancellationToken = default) =>
        await dbContext.Boards.Where(b => b.WorkspaceId == workspaceId).ToListAsync(cancellationToken);

    public async Task AddAsync(Board board, CancellationToken cancellationToken = default) =>
        await dbContext.Boards.AddAsync(board, cancellationToken);

    public void Remove(Board board) => dbContext.Boards.Remove(board);
}
