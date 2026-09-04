using Nexus.Domain.Common;
using FluentResults;

namespace Nexus.Domain.Workspaces;

public sealed class Workspace : Entity
{
    public WorkspaceName Name { get; private set; }
    public Guid OwnerId { get; }
    public DateTime CreatedAt { get; }

    private Workspace(Guid id, WorkspaceName name, Guid ownerId, DateTime createdAt)
        : base(id)
    {
        Name = name;
        OwnerId = ownerId;
        CreatedAt = createdAt;
    }

    public static Result<Workspace> Create(WorkspaceName name, Guid ownerId)
    {
        return Result.Ok(new Workspace(Guid.NewGuid(), name, ownerId, DateTime.UtcNow));
    }

    public void Rename(WorkspaceName name)
    {
        Name = name;
    }

    public bool IsOwnedBy(Guid userId) => OwnerId == userId;
}
