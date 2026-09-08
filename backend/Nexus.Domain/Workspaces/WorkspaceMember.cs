using Nexus.Domain.Common;
using FluentResults;

namespace Nexus.Domain.Workspaces;

public sealed class WorkspaceMember : Entity
{
    public Guid WorkspaceId { get; }
    public Guid UserId { get; }
    public DateTime CreatedAt { get; }

    private WorkspaceMember(Guid id, Guid workspaceId, Guid userId, DateTime createdAt)
        : base(id)
    {
        WorkspaceId = workspaceId;
        UserId = userId;
        CreatedAt = createdAt;
    }

    public static Result<WorkspaceMember> Create(Guid workspaceId, Guid userId)
    {
        return Result.Ok(new WorkspaceMember(Guid.NewGuid(), workspaceId, userId, DateTime.UtcNow));
    }
}
