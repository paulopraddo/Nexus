using Nexus.Domain.Common;
using FluentResults;

namespace Nexus.Domain.Boards;

public sealed class Board : Entity
{
    public BoardName Name { get; private set; }
    public Guid WorkspaceId { get; }
    public DateTime CreatedAt { get; }

    private Board(Guid id, BoardName name, Guid workspaceId, DateTime createdAt)
        : base(id)
    {
        Name = name;
        WorkspaceId = workspaceId;
        CreatedAt = createdAt;
    }

    public static Result<Board> Create(BoardName name, Guid workspaceId)
    {
        return Result.Ok(new Board(Guid.NewGuid(), name, workspaceId, DateTime.UtcNow));
    }

    public void Rename(BoardName name)
    {
        Name = name;
    }
}
